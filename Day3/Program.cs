using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Numerics;

namespace MyApp {
    internal class Program {
        static void Main(string[] args) {
            string path;
            string[] data;
            int sum1 = 0;
            int sum2 = 0;
            string empty;

            //path = @"..\..\..\InputTest.txt";
            //empty = "..........";
            path = @"..\..\..\InputFull.txt";
            empty = "............................................................................................................................................";

            data = File.ReadAllLines(path);
            int dataLen = data.Length;

            string[] lines = new string[3];
            List<string> gears = new List<string>();

            for (int i = 0; i < dataLen; i++) {
                // Prepare set of 3 lines
                if (i > 0) lines[0] = data[i - 1]; else lines[0] = empty;
                lines[1] = data[i];
                if (i < dataLen - 1) lines[2] = data[i + 1]; else lines[2] = empty;
                lines[0] = "." + lines[0] + ".";
                lines[1] = "." + lines[1] + ".";
                lines[2] = "." + lines[2] + ".";

                sum1 += SearchForParts(lines);
                SearchForGears(lines, gears, i);
            }

            // gears => x,y;val
            gears.Sort();
            int j = 0;
            while (j < gears.Count - 1) {
                string[] spl1 = gears[j].Split(';');
                string[] spl2 = gears[j + 1].Split(';');
                if (spl1[0] == spl2[0]) {
                    sum2 += int.Parse(spl1[1]) * int.Parse(spl2[1]);
                    j++;  // skip next gear
                }
                j++;
            }

            Console.WriteLine("RESULT 1: " + sum1);
            Console.WriteLine("RESULT 2: " + sum2);

            // PART 1: 530849
            // PART 2: 84900879
        }

        static int SearchForParts(string[] lines) {
            int sum = 0;

            // Find numbers
            Regex regex = new Regex(@"\d+");
            MatchCollection numbers = regex.Matches(lines[1]);

            // Foreach number
            foreach (Match num in numbers) {
                //Console.WriteLine("Number: " + num.Value);
                int pos = num.Index;
                int val = int.Parse(num.Value);
                int len = num.Value.Length;

                // Get surrouding chars
                int s = pos - 1;
                int d = len + 2;

                string part = "";
                part += lines[0].Substring(s, d);
                part += lines[1][pos - 1];
                part += lines[1][pos + len];
                part += lines[2].Substring(s, d);
                //Console.WriteLine("Part: " + part);

                // Find symbol in part
                Match match = Regex.Match(part, @"[^0-9\.]");
                if (match.Success) sum += val;
            }
            return sum;
        }

        static void SearchForGears(string[] lines, List<string> gears, int row) {
            // Find numbers
            Regex regex = new Regex(@"\d+");
            MatchCollection numbers = regex.Matches(lines[1]);

            // Foreach number
            foreach (Match num in numbers) {
                //Console.WriteLine("Number: " + num.Value);
                int pos = num.Index;
                int val = int.Parse(num.Value);
                int len = num.Value.Length;
                string p;
                int col;

                // Get surrouding chars
                int s = pos - 1;
                int d = len + 2;
                if (s < 0) { s = 0; d--; }
                else if (pos + len == lines[1].Length) d--;

                // Check on top
                p = lines[0].Substring(s, d);
                col = p.IndexOf("*");
                if (col >= 0) {
                    col += pos - 1;
                    gears.Add((row - 1).ToString() + "," + col + ";" + val);
                }

                // Check on left
                if (lines[1][pos - 1] == '*') {
                    col = pos - 1;
                    gears.Add(row.ToString() + "," + col + ";" + val);
                }

                // Check on right
                if (lines[1][pos + len] == '*') {
                    col = pos + len;
                    gears.Add(row.ToString() + "," + col + ";" + val);
                }

                // Check on bottom
                p = lines[2].Substring(s, d);
                col = p.IndexOf("*");
                if (col >= 0) {
                    col += pos - 1;
                    gears.Add((row + 1).ToString() + "," + col + ";" + val);
                }
            }
        }
    }
}