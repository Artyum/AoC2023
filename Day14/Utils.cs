using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Day14 {
    internal class Utils {
        public static List<string> Mod(List<string> data, int row, int col, char newChar) {
            if (row < data.Count) {
                string line = data[row];
                if (col < line.Length) {
                    char[] chars = line.ToCharArray();
                    chars[col] = newChar;
                    data[row] = new string(chars);
                }
            }
            return data;
        }

        public static void PrintGrig(List<string> data) {
            foreach (string line in data) Console.WriteLine(line);
            Console.WriteLine();
        }
    }
}
