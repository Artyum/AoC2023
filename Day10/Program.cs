using System;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    internal class Program {
        public class Pos {
            public int r { get; set; }
            public int c { get; set; }
        }
        public static List<string> map = new List<string>();
        public static List<string> mapOrg = new List<string>();
        public static List<Pos> loopList = new List<Pos>();
        public static List<Pos> outList = new List<Pos>();

        static void Main(string[] args) {
            string path;

            //path = @"..\..\..\InputTest.txt";
            path = @"..\..\..\InputFull.txt";
            map = File.ReadAllLines(path).ToList();
            mapOrg = File.ReadAllLines(path).ToList();

            Pos pos = new Pos();
            // Find S
            for (int r = 0; r < map.Count; r++) {
                for (int c = 0; c < map[r].Length; c++) {
                    if (map[r][c].ToString().ToUpper() == "S") { pos.r = r; pos.c = c; break; }
                }
                if (pos.r != 0) break;
            }

            // !!!
            // Assuming that S is the F tile
            // !!!

            Console.WriteLine("RESULT 1: " + FindLoop(pos.r, pos.c) / 2);

            // Save original loop length
            int loopLen = loopList.Count;

            // Enlarge the map 2x and add an additional 1 unit thick frame, ensuring that every
            // internal area is accessible via a path that is at least 1 unit wide
            ExtendMap2x();

            // Find the loop on extended map
            FindLoop(pos.r * 2 + 1, pos.c * 2 + 1); // +1 because of frame

            // Mark loop tiles on the map as '#'
            AddLoopToGrid();

            // Starting at point 0,0 recursively find all adjacent tiles, that will be the 'outside the loop' tiles
            FindOutsideTiles(0, 0);

            // Mark outside tiles on the map as 'O'
            AddOutsideTilesToGrid();

            SaveMap();

            // Count tiles on the original map that ARE NOT marked with 'O' on the extended map, at their corresponding position
            int cnt = CountInternalTiles();

            //Console.WriteLine("Internal = " + cnt);
            //Console.WriteLine("boundaryLen = " + boundaryLen);
            Console.WriteLine("RESULT 2: " + (cnt - loopLen));

            // RESULT 1: 7097
            // RESULT 2: 355
        }

        static int FindLoop(int srow, int scol) {
            Pos cp = new Pos();  // Current position
            Pos pp = new Pos();  // Previous position
            cp.r = srow;
            cp.c = scol;

            // If S is F:
            pp.r = srow + 1;
            pp.c = scol;

            loopList.Clear();
            int steps = 0;
            do {
                char type = map[cp.r][cp.c];
                if (type.ToString().ToUpper() == "S") type = 'F';

                // Add to loop list
                loopList.Add(new Pos() { r = cp.r, c = cp.c });

                if (type == '-') if (pp.c == cp.c - 1) { pp.c = cp.c; cp.c++; } else { pp.c = cp.c; cp.c--; }
                else if (type == '|') if (pp.r == cp.r - 1) { pp.r = cp.r; cp.r++; } else { pp.r = cp.r; cp.r--; }
                else if (type == 'F') if (pp.r == cp.r + 1) { pp.r = cp.r; pp.c = cp.c; cp.c++; } else { pp.r = cp.r; pp.c = cp.c; cp.r++; }
                else if (type == 'J') if (pp.c == cp.c - 1) { pp.r = cp.r; pp.c = cp.c; cp.r--; } else { pp.r = cp.r; pp.c = cp.c; cp.c--; }
                else if (type == 'L') if (pp.r == cp.r - 1) { pp.r = cp.r; pp.c = cp.c; cp.c++; } else { pp.r = cp.r; pp.c = cp.c; cp.r--; }
                else if (type == '7') if (pp.c == cp.c - 1) { pp.r = cp.r; pp.c = cp.c; cp.r++; } else { pp.r = cp.r; pp.c = cp.c; cp.c--; }

                steps++;
            } while (cp.r != srow || cp.c != scol);
            return steps;
        }

        static void ExtendMap2x() {
            // Extend horizontally
            for (int i = 0; i < map.Count; i++) {
                string l = map[i];
                string lh = "";
                for (int j = 0; j < l.Length; j++) {
                    if (l[j] == '.') lh += "..";
                    if (l[j] == '-') lh += "--";
                    if (l[j] == '|') lh += "|.";
                    if (l[j] == 'S') lh += "F-";
                    if (l[j] == 'F') lh += "F-";
                    if (l[j] == '7') lh += "7.";
                    if (l[j] == 'J') lh += "J.";
                    if (l[j] == 'L') lh += "L-";
                }
                map[i] = lh;
            }

            // Extend vertically
            int cnt = map.Count * 2;
            for (int i = 0; i < cnt; i += 2) {
                string l = map[i];
                string lv = "";
                for (int j = 0; j < l.Length; j++) {
                    if (l[j] == '.') lv += ".";
                    if (l[j] == '-') lv += ".";
                    if (l[j] == '|') lv += "|";
                    if (l[j] == 'S') lv += "|";
                    if (l[j] == 'F') lv += "|";
                    if (l[j] == '7') lv += "|";
                    if (l[j] == 'J') lv += ".";
                    if (l[j] == 'L') lv += ".";
                }
                map.Insert(i + 1, lv);
            }

            // Add frame around the grid
            string s = "";
            for (int i = 0; i < map[0].Length; i++) s += ".";
            map.Insert(0, s);
            map.Insert(map.Count, s);
            for (int i = 0; i < map.Count; i++) map[i] = "." + map[i] + ".";
        }

        static void FindOutsideTiles(int r, int c) {
            if (map[r][c] == '#' || IsInOutList(r, c)) return;
            outList.Add(new Pos { r = r, c = c });
            if (r - 1 >= 0) FindOutsideTiles(r - 1, c);             // Go Up
            if (c + 1 < map[0].Length) FindOutsideTiles(r, c + 1);  // Go Right
            if (r + 1 < map.Count) FindOutsideTiles(r + 1, c);      // Go Down
            if (c - 1 >= 0) FindOutsideTiles(r, c - 1);             // Go Left
        }

        static void PrintGrid() {
            Console.Write("  ");
            for (int i = 0; i < map[0].Length; i++) Console.Write(i % 10);
            Console.WriteLine();
            for (int i = 0; i < map.Count; i++) {
                string line = map[i];
                Console.Write(i % 10 + " ");
                Console.WriteLine(line);
            }
        }

        static void PrintGridOrg() {
            foreach (string line in mapOrg) Console.WriteLine(line);
        }

        static void AddOutsideTilesToGrid() {
            for (int i = 0; i < map.Count; i++) {
                StringBuilder l = new StringBuilder(map[i]);
                for (int j = 0; j < l.Length; j++) {
                    if (IsInOutList(i, j)) l[j] = 'O';
                }
                map[i] = l.ToString();
            }
        }

        static void AddLoopToGrid() {
            for (int i = 0; i < map.Count; i++) {
                StringBuilder l = new StringBuilder(map[i]);
                for (int j = 0; j < l.Length; j++) {
                    if (IsOnLoop(i, j)) l[j] = '#';
                }
                map[i] = l.ToString();
            }
        }

        static int CountInternalTiles() {
            int sum = 0;
            for (int r = 0; r < mapOrg.Count; r++) {
                for (int c = 0; c < mapOrg[r].Length; c++) {
                    if (map[r * 2 + 1][c * 2 + 1] != 'O') sum++;  // 2x + frame
                }
            }
            return sum;
        }

        static bool IsOnLoop(int row, int col) {
            return loopList.Any(pos => pos.r == row && pos.c == col);
        }

        static bool IsInOutList(int r, int c) {
            return outList.Any(pos => pos.r == r && pos.c == c);
        }

        static void SaveMap() {
            string filePath = @"..\..\..\Map.txt";
            File.WriteAllLines(filePath, map);
        }
    }
}
