using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
 * 
 * 
 * 
 * 
 *     Warning! This solution if fast for part 1 but not so good for part 2 :)
 * 
 * 
 * 
 * 
 */

namespace MyApp {
    internal class Program {
        private static object lockObject = new object();

        static void Main(string[] args) {
            Stopwatch stopwatch = new Stopwatch();

            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            List<string> data = File.ReadAllLines(path).ToList();

            long sum = 0;
            int len = data.Count;
            int n = 1;

            //var tasks = new List<Task>();
            List<Task<long>> tasks = new List<Task<long>>();

            int extend = 5;
            int threadCnt = 10;

            stopwatch.Start();
            for (int i = 0; i < data.Count; i++) {
                while (true) {
                    int running = 0;
                    foreach (var t in tasks.ToArray()) if (!t.IsCompleted) running++;
                    if (running < threadCnt) break; else Thread.Sleep(100);
                }
                string line = data[i];
                if (line[0] == '@') continue;

                string mask = line.Split(' ')[0];
                string groups = line.Split(' ')[1];
                int num = i + 1;

                mask = Extend(mask, extend, "?");
                groups = Extend(groups, extend, ",");

                //Console.WriteLine("Start " + i);
                tasks.Add(Task.Run(() => FindArrangements(num, mask, groups)));
                //Console.WriteLine("End " + i);
            }
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            long result = 0;
            foreach (var task in tasks) result += task.Result;

            //Console.WriteLine();
            //Console.WriteLine("RESULT 1: " + sum);
            Console.WriteLine("RESULT 2: " + result);
            Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds + "ms");

            // RESULT 1: 7843
            // RESULT 2: 6787193847512 LOW
        }

        static string Extend(string text, int r, string s) {
            string t = text;
            for (int i = 1; i < r; i++) {
                t += s + text;
            }
            return Clear(t);
        }

        static string Simplify(string mask, string groups) {
            string[] gtab = groups.Split(',');
            int firstGr = int.Parse(gtab.First());
            int lastGr = int.Parse(gtab.Last());

            string fill = "";
            // Replace at the begining
            for (int i = 0; i < firstGr; i++) fill += "#";
            string p = @"^\?(" + fill + @")\?";  // ^?####?.. dla grupy 4
            string r = fill + ".";
            mask = Regex.Replace(mask, p, r);

            // Replace at the end
            fill = "";
            for (int i = 0; i < lastGr; i++) fill += "#";
            p = @"\?(" + fill + @")\?$";
            r = "." + fill;
            mask = Regex.Replace(mask, p, r);

            // First group
            if (mask.First() == '#') {
                p = "";
                for (int i = 0; i < firstGr; i++) p += '#';
                p = p + ".";
                mask = p + mask.Substring(firstGr + 1, mask.Length - firstGr - 1);
            }

            // Last group
            if (mask.Last() == '#') {
                p = "";
                for (int i = 0; i < lastGr; i++) p += '#';
                p = "." + p;
                mask = mask.Substring(0, mask.Length - lastGr - 1) + p;
            }

            return Clear(mask);
            //return mask;
        }

        static long FindArrangements(int num, string mask, string groups) {
            //Console.WriteLine(mask);
            //Console.WriteLine(groups);

            mask = Simplify(Clear(mask), groups);

            // Generacja tablicy grup w postaci string #### ## ####....
            string[] grSplit = groups.Split(',');
            string[] grTab = new string[grSplit.Length];
            int[] grTabInt = new int[grSplit.Length];
            for (int i = 0; i < grSplit.Length; i++) {
                string s = "";
                int val = int.Parse(grSplit[i]);
                for (int j = 0; j < val; j++) s += "#";
                grTab[i] = s;
                grTabInt[i] = val;
            }

            long cnt = Fit(mask, 0, grTab, 0, 0, grTabInt);
            Console.WriteLine(num + "|" + cnt);
            return cnt;
        }

        static long Fit(string mask, int pos, string[] grTab, int idx, long cnt, int[] grTabInt) {
            if (idx == grTab.Length) {
                bool chk = CheckGroups2(mask, grTab);
                if (chk) {
                    cnt += 1;
                }
                //Console.WriteLine("* END FIT: " + mask + "  chk=" + chk + "  Cnt=" + GlobalCount);
                return cnt;
            }
            //Console.WriteLine("gro=" + grTab[idx]);

            bool canFit, end;

            if (pos >= mask.Length) {
                //Console.WriteLine("pos=" + pos);
                return cnt;
            }

            //if (idx >= grTab.Length) { return; }
            //Console.WriteLine("* NEW FIT -> " + mask);

            while (true) {
                //Console.WriteLine(mask + " <- mask" + "  " + GlobalCount);
                canFit = false;

                string grStr = grTab[idx];
                int grLen = grTab[idx].Length;

                if (pos + grLen > mask.Length) break;

                string ggg = "";
                for (int i = idx; i < grTab.Length; i++) ggg += grTab[i] + " ";
                if (pos + ggg.Trim().Length > mask.Length) break;

                /*int l = 0;
                for (int i = idx; i < grTabInt.Length; i++) l += grTabInt[i] + 1;
                if (pos + l - 1 > mask.Length) break;*/

                canFit = true;
                //Console.WriteLine("end=" + end);

                string cut = mask.ToString().Substring(pos, grLen);
                if (cut.Contains('.')) canFit = false;
                if (canFit && pos > 0 && mask[pos - 1] == '#') { canFit = false; }
                if (canFit && pos + grLen < mask.Length && mask[pos + grLen] == '#') { canFit = false; }

                if (canFit) {
                    string m = mask.Substring(0, pos) + grStr;
                    if (pos + grLen < mask.Length) m = m + "." + mask.Substring(pos + 1 + grLen);
                    //Console.WriteLine(m + " <- m");
                    //Console.WriteLine("IN pos=" + pos);
                    cnt = Fit(m, pos + grTab[idx].Length, grTab, idx + 1, cnt, grTabInt);
                    //Console.WriteLine("BACK pos=" + pos);
                    //if (pos >= mask.Length - grTab[idx].Length) break ;
                }

                pos++;
            }

            //Console.WriteLine("* EXIT FIT");
            return cnt;
        }

        /*static int FindArrangements(string mask, string groups) {
            Console.WriteLine("1 = " + mask);
            mask = Simplify(Clear(mask), groups);
            Console.WriteLine("2 = " + mask + " | " + groups);
            //Console.WriteLine();

            // Count '?'
            int qm = mask.Count(c => c == '?');

            // Find positions of '?'
            List<int> pos = new List<int>();
            for (int k = 0; k < mask.Length; k++) if (mask[k] == '?') pos.Add(k);

            if (pos.Count == 0) return 1;

            // Generate binary number of length qm
            int i = 0;
            string bin = "";
            int matches = 0;
            while (true) {
                bin = Convert.ToString(i, 2).PadLeft(qm, '0');
                if (bin.Length > qm) break;
                //Console.WriteLine(bin);
                char[] m = mask.ToArray();

                // Replace '?' in mask with binary
                for (int k = 0; k < pos.Count; k++) {
                    m[pos[k]] = bin[k] == '0' ? '.' : '#';
                }
                //Console.WriteLine(i + " " + string.Join("", m));
                if (CheckGroups(m, groups)) matches++;
                i++;
            }

            return matches;
        }*/


        static bool CheckGroups2(string mask, string[] grTab) {
            //Console.WriteLine("CheckGroups2 " + GlobalCount);
            //Console.WriteLine(mask);
            string gc = Clear(mask.Replace("?", ""));
            string gt = string.Join(".", grTab);
            //Console.WriteLine("gc=" + gc);
            //Console.WriteLine("gt=" + gt);
            if (gc == gt) return true;
            return false;
        }

        static bool CheckGroups(char[] str, string groups) {
            //Console.WriteLine("CheckGroups");
            int c = 0;
            List<int> g = new List<int>();

            for (int i = 0; i < str.Length; i++) {
                if (str[i] == '.') {
                    if (c > 0) g.Add(c);
                    c = 0;
                }
                else c++;
            }
            if (c > 0) g.Add(c);

            if (string.Join(",", g) == groups) {
                //Console.WriteLine('x');
                return true;
            }

            return false;
        }

        static string Clear(string s) {
            return Regex.Replace(s, @"\.{2,}", ".").Trim('.');
        }
    }
}
