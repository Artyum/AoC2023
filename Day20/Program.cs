class Broadcaster {
    public List<string> Outputs { get; set; }
    public Broadcaster() {
        Outputs = new List<string>();
    }
    public void SetOutputs(string[] dstList) {
        foreach (string dst in dstList) Outputs.Add(dst);
    }
    public void SendPulses(List<Pulse> pulseList) {
        foreach (var output in Outputs) {
            Pulse p = new Pulse();
            p.Src = "broadcaster";
            p.Dst = output;
            p.Value = 0;
            pulseList.Add(p);
            //Console.WriteLine(p.Src + " -low-> " + p.Dst);
        }
    }
}

class FlipFlop {
    public string Name { get; set; }
    public byte State { get; set; }
    public List<string> Outputs { get; set; }
    public FlipFlop(string name, string[] dstList) {
        Name = name;
        State = 0;
        Outputs = [.. dstList];
    }
    public void SetOutputs(string[] str) {
        foreach (string s in str) Outputs.Add(s);
    }
    public void ProcessPulse(Pulse pulse, List<Pulse> pulseList) {
        if (pulse.Value == 1) return;              // If a flip-flop module receives a high pulse, it is ignored and nothing happens
        if (State == 0) State = 1; else State = 0; // Flip ON/OFF
        foreach (var output in Outputs) {
            Pulse p = new Pulse();
            p.Src = Name;
            p.Dst = output;
            p.Value = State;
            pulseList.Add(p);
            //string pulseType = (State == 0) ? "-low-" : "-high-";
            //Console.WriteLine(p.Src + " " + pulseType + "> " + p.Dst);
        }
    }
    public bool IsInitialState() {
        if (State == 0) return true; else return false;
    }
}

class Conjunction {
    public string Name { get; set; }
    public List<string> Outputs { get; set; }
    public Dictionary<string, byte> Inputs { get; set; }
    public Conjunction(string name, string[] dstList) {
        Name = name;
        Inputs = new Dictionary<string, byte>();
        Outputs = [.. dstList];
    }
    public void SetInput(string name) {
        Inputs[name] = 0;
    }
    public void ProcessPulse(Pulse pulse, List<Pulse> pulseList) {
        Inputs[pulse.Src] = pulse.Value;
        byte send = 1;
        if (Inputs.Sum(x => x.Value) == Inputs.Count) send = 0; // (...)if it remembers high pulses for all inputs, it sends a low pulse
        foreach (var output in Outputs) {
            Pulse p = new Pulse();
            p.Src = Name;
            p.Dst = output;
            p.Value = send;
            pulseList.Add(p);
            //string pulseType = (send == 0) ? "-low-" : "-high-";
            //Console.WriteLine(p.Src + " " + pulseType + "> " + p.Dst);
        }
    }
    public bool IsInitialState() {
        if (Inputs.Sum(x => x.Value) == 0) return true; else return false;
    }
}

class Pulse {
    public string Src { get; set; }
    public string Dst { get; set; }
    public byte Value { get; set; }
}

internal class Program {
    static void Main(string[] args) {
        string[] data;
        Broadcaster broadcaster = new Broadcaster();
        List<Pulse> pulseList = new List<Pulse>();

        Dictionary<string, FlipFlop> flipFlops = new Dictionary<string, FlipFlop>();
        Dictionary<string, Conjunction> conjunctions = new Dictionary<string, Conjunction>();

        //string path = @"..\..\..\InputTest.txt";
        string path = @"..\..\..\InputFull.txt";
        data = File.ReadAllLines(path);

        foreach (string line in data) {
            string[] items = line.Replace(" ", "").Split("->");

            // Broadcaster
            if (line.StartsWith("broadcaster")) {
                broadcaster.SetOutputs(items[1].Split(","));
            }

            // Flip-Flop
            if (line.StartsWith("%")) {
                string name = items[0].Replace("%", "");
                FlipFlop flipFlop = new FlipFlop(name, items[1].Split(","));
                flipFlops[name] = flipFlop;
            }

            // Conjunction
            if (line.StartsWith("&")) {
                string name = items[0].Replace("&", "");
                Conjunction conjunction = new Conjunction(name, items[1].Split(","));
                conjunctions[name] = conjunction;
            }
        }

        // Set inputs for conjunctions
        foreach (var con in conjunctions) {
            foreach (string line in data) {
                string[] items = line.Replace(" ", "").Split("->");
                if (items[1].Contains(con.Key)) {
                    conjunctions[con.Key].SetInput(items[0].Replace("%", "").Replace("&", ""));
                }
            }
        }

        long btnPress = 1;
        bool end = false;

        // PART 1
        long lowCnt = 0;
        long highCnt = 0;
        long result1 = 0;

        // PART 2
        // Manually set module name that sends to rx
        string finalMod = "ll";

        // Get modules sending to finalMod
        Dictionary<string, long> modules = new Dictionary<string, long>();
        foreach (var con in conjunctions) if (con.Value.Outputs.Contains(finalMod)) modules.Add(con.Key, 0);

        while (!end) {
            PushButton(broadcaster, pulseList);

            while (pulseList.Count > 0) {

                List<Pulse> newPulseList = new List<Pulse>();
                foreach (Pulse pulse in pulseList) {
                    if (pulse.Value == 0) lowCnt++; else highCnt++;

                    if (flipFlops.ContainsKey(pulse.Dst)) flipFlops[pulse.Dst].ProcessPulse(pulse, newPulseList);
                    else if (conjunctions.ContainsKey(pulse.Dst)) conjunctions[pulse.Dst].ProcessPulse(pulse, newPulseList);
                }

                pulseList = newPulseList;

                // PART 2
                // Find the button press number when modules send a high pulse to finalMod
                foreach (Pulse pulse in pulseList) {
                    foreach (var module in modules) {
                        if (pulse.Src == module.Key && pulse.Value == 1) { modules[module.Key] = btnPress; }
                    }
                }
            }

            // TEST INPUT
            //if (CheckInitialState(flipFlops, conjunctions)) end = true;

            // PART 1
            if (btnPress == 1000) result1 = (lowCnt + btnPress) * highCnt;

            if (btnPress >= 1000) {
                // Verify if all sources have been identified
                end = true;
                foreach (var module in modules) if (module.Value == 0) end = false;
            }

            btnPress++;
        }

        // PART 2 - When all modules simultaneously send a high pulse to finalMod, then finalMod will send a low pulse to rx
        // Find least common multiple
        long result2 = 1;
        foreach (var module in modules) result2 = calcLCM(result2, module.Value);

        Console.WriteLine("RESULT 1: " + result1);
        Console.WriteLine("RESULT 2: " + result2);

        // RESULT 1: 743090292
        // RESULT 2: 241528184647003
    }

    static void PushButton(Broadcaster broadcaster, List<Pulse> pulseList) {
        //Console.WriteLine("button -low-> broadcaster");
        broadcaster.SendPulses(pulseList);
    }

    static bool CheckInitialState(Dictionary<string, FlipFlop> flipFlops, Dictionary<string, Conjunction> conjunctions) {
        foreach (var ff in flipFlops) if (!ff.Value.IsInitialState()) return false;
        foreach (var con in conjunctions) if (!con.Value.IsInitialState()) return false;
        return true;
    }

    // Least Common Multiple
    static long calcLCM(long a, long b) {
        return Math.Abs(a * b) / calcGCD(a, b);
    }

    // Greatest Common Divisor / Euklides algorithm
    static long calcGCD(long a, long b) {
        while (b != 0) {
            long c = a % b;
            a = b;
            b = c;
        }
        return a;
    }
}