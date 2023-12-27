using System;
using System.Data;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    public class Point {
        public long x { get; set; }
        public long y { get; set; }
    }

    internal class Program {
        public static string[] data;
        public static List<Point> vList = new List<Point>();

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            // The area is calculated using the Shoelace formula, plus half of the perimeter's area.
            // Corners are counted as three-quarters of a right turn and one-quarter of a left turn.

            var (frameLen, rightTurns, leftTurns) = GenerateVerticleList();
            long area = ShoelaceFormula(vList);
            area = (long)(area + (frameLen - vList.Count) / 2.0 + rightTurns * 3.0 / 4.0 + leftTurns / 4.0);

            Console.WriteLine("RESULT: " + area);

            // PART 1: 39194
            // PART 2: 78242031808225
        }

        static (long, long, long) GenerateVerticleList() {
            long x = 0;
            long y = 0;
            long rightTurns = 0;
            long leftTurns = 0;
            long frameLen = 0;

            for (int i = 0; i < data.Length; i++) {
                string[] item = data[i].Split(' ');
                string[] next = data[(i + 1) % data.Length].Split(' ');

                // PART 1
                //int dir = Cdir(item[0][0]);
                //int ndir = Cdir(next[0][0]);
                //long len = int.Parse(item[1]);
                //----------------

                // PART 2
                string hex = item[2].Replace("(#", "").Replace(")", "");
                string nhex = next[2].Replace("(#", "").Replace(")", "");
                int dir = int.Parse(hex.Last().ToString());
                int ndir = int.Parse(nhex.Last().ToString());
                long len = Convert.ToInt64(hex.Substring(0, hex.Length - 1), 16);
                //----------------
                
                if (dir == 0) x += len;
                else if (dir == 1) y += len;
                else if (dir == 2) x -= len;
                else if (dir == 3) y -= len;

                vList.Add(new Point { x = x, y = y });

                if (dir == 0 && ndir == 1) rightTurns++;
                if (dir == 0 && ndir == 3) leftTurns++;
                if (dir == 1 && ndir == 2) rightTurns++;
                if (dir == 1 && ndir == 0) leftTurns++;
                if (dir == 2 && ndir == 3) rightTurns++;
                if (dir == 2 && ndir == 1) leftTurns++;
                if (dir == 3 && ndir == 0) rightTurns++;
                if (dir == 3 && ndir == 2) leftTurns++;

                frameLen += len;
            }

            return (frameLen, rightTurns, leftTurns);
        }

        public static long ShoelaceFormula(List<Point> points) {
            long area = 0;
            for (int i = 0; i < points.Count - 1; i++) area += (points[i].y + points[i + 1].y) * (points[i].x - points[i + 1].x);
            area += (points[points.Count - 1].y + points[0].y) * (points[points.Count - 1].x - points[0].x);
            if (area < 0) area = -area;
            return area / 2;
        }

        static int Cdir(char dir) {
            // 0-R, 1-D, 2-L, 3-U
            if (dir == 'R') return 0;
            else if (dir == 'D') return 1;
            else if (dir == 'L') return 2;
            else if (dir == 'U') return 3;
            else return -1;
        }
    }
}