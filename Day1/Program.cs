﻿using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;

class Program {
    static void Main() {
        string[] dane;
        string path = "";

        //path = @"..\..\..\InputTest.txt";
        path = @"..\..\..\InputFull.txt";
        
        dane = File.ReadAllLines(path);

        int sum = 0;

        foreach (string line in dane) {
            //Console.WriteLine(line);
            string l = line.ToLower();

            l = prepareLine(line);  // <- Enable for part 2

            string strDigits = new string(l.Where(char.IsDigit).ToArray());
            //Console.WriteLine(strDigits);

            int number = getNumber(strDigits);
            sum += number;

            //Console.WriteLine(number.ToString());
        }

        Console.WriteLine("RESULT: " + sum.ToString());

        // PART 1: 53080
        // PART 2: 53268
    }

    static int getNumber(string str) {
        string strNum;
        if (str.Length == 1) strNum = str + str;
        else strNum = str.First().ToString() + str.Last().ToString();
        return int.Parse(strNum);
    }
    static string prepareLine(string line) {
        //Console.WriteLine(line);

        if (line.Length == 1) { return line; }

        string[] strDigits = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        string[] intDigits = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        // Search from left
        string l = "";
        bool done = false;
        for (int i = 0; i < line.Length; i++) {
            if (char.IsDigit(line[i])) break;
            l += line[i];
            for (int j = 0; j < strDigits.Length; j++) {
                if (l.Contains(strDigits[j])) {
                    line = intDigits[j] + line;
                    done = true;
                    break;
                }
            }
            if (done) break;
        }

        // Search from right
        l = "";
        done = false;
        for (int i = line.Length - 1; i >= 0; i--) {
            if (char.IsDigit(line[i])) break;
            l = line[i] + l;
            for (int j = 0; j < strDigits.Length; j++) {
                if (l.Contains(strDigits[j])) {
                    line = line + intDigits[j];
                    done = true;
                    break;
                }
            }
            if (done) break;
        }

        //Console.WriteLine(line);
        return line;
    }
}
