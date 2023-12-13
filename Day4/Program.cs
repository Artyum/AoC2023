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
            int i;
            int sum1 = 0;
            List<string> cards = new List<string>();

            //path = @"..\..\..\InputTest.txt";
            path = @"..\..\..\InputFull.txt";

            data = File.ReadAllLines(path);
            int[] pts = new int[data.Length];

            for (i = 0; i < data.Length; i++) {
                string line = data[i].Replace("  ", " ");
                string[] sets = line.Split('|');
                string str1 = (sets[0].Split(':'))[1];
                string str2 = sets[1];
                string[] numsWin = str1.Split(' ');
                string[] nums = str2.Split(' ');

                int m = 0;
                foreach (string n in nums) {
                    if (n != "" && numsWin.Contains(n)) m++;
                }

                double score = 0;
                if (m != 0) score = 1 * Math.Pow(2, m - 1);
                cards.Add(line);
                pts[i] = m;
                sum1 += (int)score;
            }

            i = 0;
            while (true) {
                int cn = int.Parse(cards[i].Split(':')[0].Replace("Card", "").Replace(" ", ""));
                int copycnt = pts[cn - 1];

                // Add card copies
                for (int j = cn + 1; j < cn + 1 + copycnt; j++) cards.Add(data[j - 1]);

                i++;
                if (i == cards.Count) break;
            }

            Console.WriteLine("RESULT 1: " + sum1);
            Console.WriteLine("RESULT 2: " + cards.Count);

            // PART 1: 21105
            // PART 2: 5329815
        }
    }
}