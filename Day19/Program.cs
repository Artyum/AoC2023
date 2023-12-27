using System;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    class Rule {
        public string cat { get; set; }
        public char oper { get; set; }
        public int val { get; set; }
        public string dest { get; set; }
        public string final { get; set; }
    }

    class Part {
        public int x { get; set; }
        public int m { get; set; }
        public int a { get; set; }
        public int s { get; set; }
    }

    internal class Program {
        public static string[] data;
        public static Dictionary<string, List<Rule>> workflows = new Dictionary<string, List<Rule>>();
        public static List<Part> parts = new List<Part>();
        public static List<string> Accept = new List<string>();
        public static List<string> Reject = new List<string>();
        public static long result = 0;
        public static List<string> paths = new List<string>();

        static void Main(string[] args) {
            string path = @"..\..\..\InputTest.txt";
            //string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            GetParts(GetWorkflows());
            foreach (Part part in parts) ProcessPart(part, "in");

            Console.WriteLine("RESULT: " + result);
            // RESULT: 398527

            FindPaths("in", "", "");
            foreach (var p in paths) Console.WriteLine(p);
        }

        static void FindPaths(string wfname, string path, string r) {
            string p = path + " " + r;
            if (wfname == "A" || wfname == "R") {
                if (wfname == "A") paths.Add(p);
                return;
            }
            foreach (Rule rule in workflows[wfname]) {
                if (rule.final != null) FindPaths(rule.final, p, "!" + wfname);
                else FindPaths(rule.dest, p, rule.cat + rule.oper + rule.val);
            }
        }

        public static void ProcessPart(Part part, string wfname) {
            if (wfname == "A" || wfname == "R") {
                if (wfname == "A") result += part.x + part.m + part.a + part.s;
                return;
            }

            foreach (Rule rule in workflows[wfname]) {
                if (rule.final != null) ProcessPart(part, rule.final);
                else {
                    string dest = CheckRule(part, rule);
                    if (dest != "-") {
                        ProcessPart(part, dest);
                        break;
                    }
                }
            }
        }

        public static string CheckRule(Part p, Rule r) {
            string dest = "-";
            if (r.cat == "x" && r.oper == '<' && p.x < r.val) dest = r.dest;
            else if (r.cat == "x" && r.oper == '>' && p.x > r.val) dest = r.dest;
            else if (r.cat == "m" && r.oper == '<' && p.m < r.val) dest = r.dest;
            else if (r.cat == "m" && r.oper == '>' && p.m > r.val) dest = r.dest;
            else if (r.cat == "a" && r.oper == '<' && p.a < r.val) dest = r.dest;
            else if (r.cat == "a" && r.oper == '>' && p.a > r.val) dest = r.dest;
            else if (r.cat == "s" && r.oper == '<' && p.s < r.val) dest = r.dest;
            else if (r.cat == "s" && r.oper == '>' && p.s > r.val) dest = r.dest;
            return dest;
        }

        public static void GetParts(int idx) {
            for (int i = idx; i < data.Length; i++) {
                string[] itm = data[i].Replace("{", "").Replace("}", "").Replace("x=", "").Replace("m=", "").Replace("a=", "").Replace("s=", "").Split(",");
                Part p = new Part();
                p.x = int.Parse(itm[0]);
                p.m = int.Parse(itm[1]);
                p.a = int.Parse(itm[2]);
                p.s = int.Parse(itm[3]);
                parts.Add(p);
            }
        }
        public static int GetWorkflows() {
            // Get workflows
            int i;
            for (i = 0; i < data.Length; i++) {
                if (data[i].Length == 0) break;

                string[] li = data[i].Split("{");
                string name = li[0];

                workflows[name] = new List<Rule>();

                string[] rules = li[1].Split(",");
                foreach (string rul in rules) {
                    Rule rule = new Rule();
                    string[] rs = rul.Split(":");
                    if (rs.Length == 1) { rule.final = rul.Replace("}", ""); }
                    else {
                        if (rs[0].Contains(">")) rule.oper = '>'; else rule.oper = '<';
                        string[] rss = rs[0].Replace(">", "<").Split("<");
                        rule.cat = rss[0];
                        rule.val = int.Parse(rss[1]);
                        rule.dest = rs[1];
                    }
                    workflows[name].Add(rule);
                }
            }
            return i + 1;
        }
    }
}
