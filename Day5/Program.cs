using System.Diagnostics;

namespace MyApp {
    internal class Program {
        private static object lockObject = new object();
        private static long minLoc;
        private static List<long> listSeeds = new List<long>();
        private static List<long[]>[] listMaps = new List<long[]>[7];

        static void Main(string[] args) {
            Stopwatch stopwatch = new Stopwatch();
            string path;
            string[] data;

            //path = @"..\..\..\InputTest.txt";
            path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            for (int i = 0; i < listMaps.Length; i++) listMaps[i] = new List<long[]>();

            // Parse input
            int sel = 0; // Selector
            for (int i = 0; i < data.Length; i++) {
                string line = data[i];

                if (line.StartsWith("seed-to-soil")) sel = 0;
                if (line.StartsWith("soil-to-fertilizer")) sel = 1;
                if (line.StartsWith("fertilizer-to-water")) sel = 2;
                if (line.StartsWith("water-to-light")) sel = 3;
                if (line.StartsWith("light-to-temperature")) sel = 4;
                if (line.StartsWith("temperature-to-humidity")) sel = 5;
                if (line.StartsWith("humidity-to-location")) sel = 6;

                if (i == 0) {
                    string[] n = line.Split(":")[1].Split(" ");
                    foreach (string s in n) if (s != "") listSeeds.Add(long.Parse(s));
                }
                else if (line != "" && Char.IsDigit(line[0])) {
                    string[] num = line.Split(" ");
                    long[] tab = new long[3];
                    for (int j = 0; j < 3; j++) tab[j] = long.Parse(num[j]);
                    listMaps[sel].Add(tab);
                }
            }

            // Part 1
            minLoc = long.MaxValue;
            foreach (long seed in listSeeds) {
                long l = GetLocation(seed, listMaps);
                if (l < minLoc) minLoc = l;
            }
            Console.WriteLine("RESULT 1: " + minLoc);

            // Part 2 - Full scan - Parallel threads
            stopwatch.Start();
            minLoc = long.MaxValue;
            var tasks = new List<Task>();
            int threadCnt = 6;
            for (int i = 0; i < listSeeds.Count; i += 2) {
                while (true) {
                    int running = 0;
                    foreach (var t in tasks.ToArray()) if (!t.IsCompleted) running++;
                    if (running < threadCnt) break; else Thread.Sleep(10);
                }
                int id = i / 2 + 1;
                long seed = listSeeds[i];
                long range = listSeeds[i + 1];
                tasks.Add(Task.Run(() => StartThread(id, seed, range, listMaps)));
            }
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            Console.WriteLine("RESULT 2: " + minLoc);
            Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds + "ms");

            // RESULT 1: 825516882
            // RESULT 2: 136096660
            // Total seeds: 1624044411

            // Time @ Intel i7-10750H
            // 1 thread   162327ms
            // 2 threads   86058ms
            // 3 threads   64808ms
            // 4 threads   54184ms
            // 5 threads   55976ms
            // 6 threads   49375ms
            // 7 threads   46776ms
            // 8 threads   46914ms
            // 9 threads   46805ms
            // 10 threads  48834ms
        }

        static void StartThread(int id, long seed, long range, List<long[]>[] tab) {
            Console.WriteLine("Start " + id + "\tSeed=" + seed + "\trange=" + range);
            long ml = long.MaxValue;
            for (long s = seed; s < seed + range; s++) {
                long l = GetLocation(s, tab);
                if (l < ml) ml = l;
            }
            lock (lockObject) {
                if (ml < minLoc) minLoc = ml;
            }
            Console.WriteLine("End " + id);
        }

        static long GetLocation(long seed, List<long[]>[] tab) {
            long soil = Map(seed, tab[0]);
            long fertilizer = Map(soil, tab[1]);
            long water = Map(fertilizer, tab[2]);
            long light = Map(water, tab[3]);
            long temperature = Map(light, tab[4]);
            long humidity = Map(temperature, tab[5]);
            long location = Map(humidity, tab[6]);
            return location;
        }

        static long Map(long source, List<long[]> tab) {
            for (int i = 0; i < tab.Count; i++) {
                long dest = tab[i][0];
                long start = tab[i][1];
                long range = tab[i][2];
                if ((source >= start) && (source <= start + range)) return (source - start + dest);
            }
            return source;
        }
    }
}