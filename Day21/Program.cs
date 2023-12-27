namespace MyApp {
    class Point {
        public int r { get; set; }
        public int c { get; set; }
        public override bool Equals(object obj) {
            Point p = (Point)obj;
            return (r == p.r) && (c == p.c);
        }

        public override int GetHashCode() {
            return r ^ c;
        }
    }

    internal class Program {
        public static string[] data;
        public static HashSet<Point> points = new HashSet<Point>();

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);
            data = ExpandGrid(5);

            int r1 = (data.Length - 1) / 2;
            int c1 = (data[0].Length - 1) / 2;

            int steps = 327;

            // Pattern repeats every 131
            int count65 = 0;   // (131-1) / 2 = 65
            int count196 = 0;  // 65 + 131 = 196
            int count327 = 0;  // 196 + 131 = 327

            points.Add(new Point { r = r1, c = c1 });
            for (int i = 1; i <= steps; i++) {
                MovePoints();
                if (i == 64) Console.WriteLine("RESULT 1: " + points.Count);
                if (i == 65) count65 = points.Count;
                if (i == 196) count196 = points.Count;
                if (i == 327) count327 = points.Count;
            }

            //Console.WriteLine("count65  = " + count65);
            //Console.WriteLine("count196 = " + count196);
            //Console.WriteLine("count327 = " + count327);

            // count65  = 3874
            // count196 = 34549
            // count327 = 95798

            // Equation of a parabola passing through 3 points found using: https://www.mathway.com/Algebra
            double x = 26501365.0;
            double y = 15287.0 * x * x / 17161.0 + 28518.0 * x / 17161.0 + 40469.0 / 17161.0;
            Console.WriteLine("RESULT 2: " + y);

            // PART 1: 3773
            // PART 2: 625628021226274
        }

        static void MovePoints() {
            HashSet<Point> tmp = new HashSet<Point>();

            foreach (Point p in points) {
                if (p.r > 0 && data[p.r - 1][p.c] != '#') tmp.Add(new Point { r = p.r - 1, c = p.c });                   // UP
                if (p.r < data.Length - 1 && data[p.r + 1][p.c] != '#') tmp.Add(new Point { r = p.r + 1, c = p.c });     // DOWN
                if (p.c > 0 && data[p.r][p.c - 1] != '#') tmp.Add(new Point { r = p.r, c = p.c - 1 });                   // LEFT
                if (p.c < data[0].Length - 1 && data[p.r][p.c + 1] != '#') tmp.Add(new Point { r = p.r, c = p.c + 1 });  // RIGHT
            }

            points.Clear();
            points = tmp;
        }

        static bool PointExists(HashSet<Point> list, int r, int c) {
            return list.Any(n => n.r == r && n.c == c);
        }

        static void PrintGrid() {
            for (int r = 0; r < data.Length; r++) {
                for (int c = 0; c < data[r].Length; c++) {
                    if (PointExists(points, r, c)) Console.Write("O");
                    else Console.Write(data[r][c]);
                }
                Console.WriteLine();
            }
        }

        static void SaveGrid() {
            string fileName = @"..\..\..\Grid.txt";
            string[] strings = new string[data.Length];
            for (int r = 0; r < data.Length; r++) {
                string line = data[r];
                for (int c = 0; (c < line.Length); c++) {
                    if (PointExists(points, r, c)) line = line.Substring(0, c) + 'O' + line.Substring(c + 1);
                }
                strings[r] = line;
            }
            File.WriteAllLines(fileName, strings);
        }

        static string[] ExpandGrid(int n) {
            int len = data.Length;
            string[] result = new string[len * n];
            for (int r = 0; r < len; r++) {
                string line = "";
                for (int i = 0; i < n; i++) line += data[r];
                result[r] = line;
                for (int x = 1; x < n; x++) result[r + len * x] = result[r];
            }
            return result;
        }
    }
}