internal class Program {
    private class Directions {
        public string Left { get; set; }
        public string Right { get; set; }
    }

    static string path;
    static Dictionary<string, Directions> map = new Dictionary<string, Directions>();

    static void Main(string[] args) {
        string file;
        //file = @"..\..\..\InputTest.txt";
        file = @"..\..\..\InputFull.txt";
        string[] data = File.ReadAllLines(file);
        path = data[0];

        // Parse input
        data = data.Skip(2).ToArray();
        for (int i = 0; i < data.Length; i++) {
            string line = data[i].Replace("(", "").Replace(")", "").Replace(" ", "");
            string node = line.Split("=")[0];
            string[] dir = line.Split("=")[1].Split(",");
            map[node] = new Directions { Left = dir[0], Right = dir[1] };
        }

        // PART 1
        Console.WriteLine("RESULT 1: " + calcSteps("AAA", "ZZZ"));

        // PART 2
        // Find steps required to reach position Z from all starting nodes
        List<long> stepsList = new List<long>();
        foreach (var pair in map) if (pair.Key.EndsWith('A')) stepsList.Add(calcSteps(pair.Key, "Z"));

        // Find least common multiple
        long lcm = stepsList[0];
        for (int i = 1; i < stepsList.Count(); i++) lcm = calcLCM(lcm, stepsList[i]);
        Console.WriteLine("RESULT 2: " + lcm);

        // RESULT 1: 24253
        // RESULT 2: 12357789728873
    }

    static long calcSteps(string start, string endWith) {
        int i = 0;
        long steps = 0;
        string pos = start;
        while (!pos.EndsWith(endWith)) {
            if (path[i] == 'L') pos = map[pos].Left; else pos = map[pos].Right;
            i = (i + 1) % path.Length;
            steps++;
        }
        return steps;
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