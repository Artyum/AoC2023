using System;
using System.Data;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    class Position {
        public int x { get; set; }
        public int y { get; set; }
    }

    internal class Program {
        public static List<string> data = new List<string>();
        public static List<Position> starList = new List<Position>();
        public static List<int> emptyRowList = new List<int>();
        public static List<int> emptyColList = new List<int>();

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path).ToList();

            FindStars();
            FindEmptyRows();
            FindEmptyCols();

            long sum1 = 0;
            long sum2 = 0;
            for (int i = 0; i < starList.Count; i++) {
                for (int j = i + 1; j < starList.Count; j++) {
                    sum1 += CalcDist(2, starList[i], starList[j]);
                    sum2 += CalcDist(1000000, starList[i], starList[j]);
                }
            }
            Console.WriteLine("RESULT 1: " + sum1);
            Console.WriteLine("RESULT 2: " + sum2);

            // RESULT 1: 9681886
            // RESULT 2: 791134099634
        }

        static void FindStars() {
            for (int r = 0; r < data.Count; r++) {
                for (int c = 0; c < data[r].Length; c++) {
                    if (data[r][c] == '#') starList.Add(new Position { x = c, y = r });
                }
            }
        }

        static void FindEmptyRows() {
            for (int i = 0; i < data.Count; i++) if (!data[i].Contains("#")) emptyRowList.Add(i);
        }

        static void FindEmptyCols() {
            for (int c = 0; c < data[0].Length - 1; c++) {
                bool empty = true;
                foreach (string l in data) if (l[c] == '#') { empty = false; break; }
                if (empty) emptyColList.Add(c);
            }
        }

        static int CalcDist(int expand, Position a, Position b) {
            int emptyColCnt = 0;
            int emptyRowCnt = 0;
            expand--;

            // Count empty columns
            int p = Math.Min(a.x, b.x) + 1;
            int m = Math.Max(a.x, b.x) - 1;
            while (p <= m) if (emptyColList.Contains(p++)) emptyColCnt++;

            // Count empty rows
            p = Math.Min(a.y, b.y) + 1;
            m = Math.Max(a.y, b.y) - 1;
            while (p <= m) if (emptyRowList.Contains(p++)) emptyRowCnt++;

            // Manhattan distance
            return (Math.Abs(a.x - b.x) + expand * emptyColCnt) + (Math.Abs(a.y - b.y) + expand * emptyRowCnt);
        }
    }
}
