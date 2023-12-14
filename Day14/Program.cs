using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Day14.Utils;

namespace MyApp {
    public class Item {
        public int count { get; set; }
        public int cycle { get; set; }
    }
    internal class Program {
        public static List<string> data = new List<string>();

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path).ToList();

            Dictionary<int, Item> dict = new Dictionary<int, Item>();

            // This number must be large enough for the grid layout to repeat itself after a specified number of cycles
            // Most likely it also must be a multiple of 1'000'000'000 and power of 10
            int cycles = 1000;

            for (int i = 1; i <= cycles; i++) {
                Tilt('N');
                if (i == 1) Console.WriteLine("RESULT 1: " + CalculateLoad());
                Tilt('W');
                Tilt('S');
                Tilt('E');
                int load = CalculateLoad();
                if (dict.ContainsKey(load)) { dict[load].count++; dict[load].cycle = i; }
                else dict[load] = new Item { count = 1, cycle = i };
            }

            foreach (var item in dict) { if (item.Value.cycle == cycles) Console.WriteLine("RESULT 2: " + item.Key); }

            // RESULT 1: 107053
            // RESULT 2: 88371
        }

        static void Tilt(char direction) {
            bool move = true;
            while (move) {
                move = false;
                if (direction == 'N') {
                    for (int c = 0; c < data[0].Length; c++) {
                        for (int r = 1; r < data.Count; r++) {
                            if (data[r][c] == 'O' && data[r - 1][c] == '.') {
                                data = Mod(data, r - 1, c, 'O');
                                data = Mod(data, r, c, '.');
                                move = true;
                            }
                        }
                    }
                }
                else if (direction == 'W') {
                    for (int r = 0; r < data.Count; r++) {
                        for (int c = 1; c < data[0].Length; c++) {
                            if (data[r][c] == 'O' && data[r][c - 1] == '.') {
                                data = Mod(data, r, c - 1, 'O');
                                data = Mod(data, r, c, '.');
                                move = true;
                            }
                        }
                    }
                }
                if (direction == 'S') {
                    for (int c = 0; c < data[0].Length; c++) {
                        for (int r = data.Count - 2; r >= 0; r--) {
                            if (data[r][c] == 'O' && data[r + 1][c] == '.') {
                                data = Mod(data, r + 1, c, 'O');
                                data = Mod(data, r, c, '.');
                                move = true;
                            }
                        }
                    }
                }
                else if (direction == 'E') {
                    for (int r = 0; r < data.Count; r++) {
                        for (int c = data[0].Length - 2; c >= 0; c--) {
                            if (data[r][c] == 'O' && data[r][c + 1] == '.') {
                                data = Mod(data, r, c + 1, 'O');
                                data = Mod(data, r, c, '.');
                                move = true;
                            }
                        }
                    }
                }
            }
        }

        static int CalculateLoad() {
            int sum = 0;
            for (int c = 0; c < data[0].Length; c++) {
                int load = data.Count;

                for (int r = 0; r < data.Count; r++) {
                    //if (data[r][c] == '#') break;
                    if (data[r][c] == 'O') sum += load - r;
                }
            }
            return sum;
        }
    }
}