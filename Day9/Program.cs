namespace MyApp {
    internal class Program {
        static void Main(string[] args) {
            string path;
            string[] data;

            //path = @"..\..\..\InputTest.txt";
            path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            long sum1 = 0, sum2 = 0;

            foreach (string line in data) {
                List<long> list = new List<long>();
                foreach (string s in line.Split(" ")) list.Add(long.Parse(s));

                sum1 += list.Last() + FindNext(list);
                sum2 += list.First() - FindPrev(list);
            }

            Console.WriteLine("RESULT 1: " + sum1);
            Console.WriteLine("RESULT 2: " + sum2);

            // RESULT 1: 1930746032
            // RESULT 2: 1154
        }

        static long FindNext(List<long> list) {
            if (list.Sum() == 0) return 0;
            List<long> diffs = new List<long>();
            for (int i = 1; i < list.Count; i++) diffs.Add(list[i] - list[i - 1]);
            return diffs.Last() + FindNext(diffs);
        }

        static long FindPrev(List<long> list) {
            if (list.Sum() == 0) return 0;
            List<long> diffs = new List<long>();
            for (int i = 1; i < list.Count; i++) diffs.Add(list[i] - list[i - 1]);
            return diffs.First() - FindPrev(diffs);
        }
    }
}