﻿using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    internal class Program {
        static void Main(string[] args) {
            List<string> data = new List<string>();
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path).ToList();

            List<string> pattern = new List<string>();
            long result1 = 0;
            long result2 = 0;

            for (int i = 0; i <= data.Count; i++) {
                if (i == data.Count || data[i].Length == 0) {
                    // Convert rows and columns to integers where '.'->0 and '#'->1 making a binary number for further comparisons
                    var hashTabH = CalcHashTabH(pattern);
                    var hashTabV = CalcHashTabV(pattern);

                    // PART 1
                    int centerH1 = FindMirror(hashTabH);
                    int centerV1 = FindMirror(hashTabV);
                    if (centerH1 > 0) result1 += 100 * centerH1;
                    if (centerV1 > 0) result1 += centerV1;

                    // PART 2
                    int centerH2 = FindSmudge(hashTabH, centerH1);
                    int centerV2 = FindSmudge(hashTabV, centerV1);
                    if (centerH2 > 0) result2 += 100 * centerH2;
                    if (centerV2 > 0) result2 += centerV2;

                    pattern.Clear();
                }
                else {
                    pattern.Add(data[i]);
                }
            }

            Console.WriteLine("RESULT 1: " + result1);
            Console.WriteLine("RESULT 2: " + result2);

            // RESULT 1: 35232
            // RESULT 2: 37982
        }

        static int FindMirror(List<int> tab) {
            List<int> centerList = new List<int>();

            // Find mirror plane
            for (int i = 0; i < tab.Count - 1; i++) if (tab[i] == tab[i + 1]) centerList.Add(i);

            if (centerList.Count == 0) return -1;
            foreach (int center in centerList) {
                int a = 1;
                bool mirror = true;
                for (int i = center; i >= 0 && i + a < tab.Count; i--) {
                    if (tab[i] != tab[i + a]) { mirror = false; break; }
                    a += 2;
                }
                if (mirror) {
                    return center + 1;
                }
            }
            return -1;
        }

        static int FindSmudge(List<int> tab, int prevCenter) {
            // Checking every row
            for (int center = 0; center < tab.Count - 1; center++) {
                // Check reflection
                int a = 1;
                bool mirror = true;
                for (int i = center; i >= 0 && i + a < tab.Count; i--) {
                    long op1 = tab[i];
                    long op2 = tab[i + a];
                    long xor = op1 ^ op2;
                    long bin = xor & (xor - 1);  // If bin==0, then only one bit differs between op1 and op2

                    if ((op1 != op2) && bin != 0) { mirror = false; break; }
                    a += 2;
                }
                if (mirror) {
                    // Continue if the current center is the same as the previous one
                    if (prevCenter != center + 1) return center + 1;
                }
            }
            return -1;
        }

        static List<int> CalcHashTabH(List<string> pattern) {
            List<int> list = new List<int>();
            for (int r = 0; r < pattern.Count; r++) {
                string binaryString = pattern[r].Replace('#', '1').Replace('.', '0');
                list.Add(Convert.ToInt32(binaryString, 2));
            }
            return list;
        }

        static List<int> CalcHashTabV(List<string> pattern) {
            List<int> list = new List<int>();
            for (int c = 0; c < pattern[0].Length; c++) {
                string line = "";
                for (int r = 0; r < pattern.Count; r++) {
                    line += pattern[r][c];
                }
                string binaryString = line.Replace('#', '1').Replace('.', '0');
                list.Add(Convert.ToInt32(binaryString, 2));
            }
            return list;
        }
    }
}
