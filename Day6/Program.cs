using System.Diagnostics;
using System.Runtime.Serialization.Formatters;

namespace MyApp {
    internal class Program {
        static void Main(string[] args) {
            string path;
            string[] data;
            List<long> time = new List<long>();
            List<long> dist = new List<long>();

            //path = @"..\..\..\InputTest.txt";
            path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            // PART 1
            //string[] l0 = data[0].Split(':')[1].Split(' ');
            //foreach (string x in l0) if (x != "") time.Add(int.Parse(x.Trim()));
            //string[] l1 = data[1].Split(':')[1].Split(' ');
            //foreach (string x in l1) if (x != "") dist.Add(int.Parse(x.Trim()));

            // PART 2
            time.Add(long.Parse(data[0].Split(':')[1].Replace(" ", "")));
            dist.Add(long.Parse(data[1].Split(':')[1].Replace(" ", "")));

            long result = 1;
            for (int i = 0; i < time.Count; i++) {
                long cnt = 0;
                long spd = 0;
                for (long t = 1; t <= time[i]; t++) {
                    spd++;
                    long d = spd * (time[i] - t);
                    if (d > dist[i]) cnt++;
                }
                if (cnt > 0) result *= cnt;
            }

            Console.WriteLine("RESULT:" + result);

            // RESULT 1: 293046
            // RESULT 2: 35150181
        }
    }
}