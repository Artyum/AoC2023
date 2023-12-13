using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

namespace MyApp {
    internal class Program {
        static void Main(string[] args) {
            string[] data;
            int sum1 = 0;
            int sum2 = 0;
            int id = 1; //Game ID

            const int maxRed = 12;
            const int maxGreen = 13;
            const int maxBlue = 14;

            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            foreach (string line in data) {
                if (line[0] == '#') continue;
                string[] record = line.Split(":");
                string[] games = record[1].Split(";");
                int gamesCnt = games.Length;

                bool setOK = true;
                int mr = 0;  // max red
                int mg = 0;  // max green
                int mb = 0;  // max blue
                foreach (string set in games) {
                    string[] play = set.Split(",");

                    foreach (string token in play) {
                        string[] parts = token.Trim().Split(' ');
                        int number = int.Parse(parts[0].Trim());
                        string color = parts[1].Trim();

                        if (color == "red") {
                            if (number > maxRed) setOK = false;
                            if (number > mr) mr = number;
                        }
                        if (color == "green") {
                            if (number > maxGreen) setOK = false;
                            if (number > mg) mg = number;
                        }
                        if (color == "blue") {
                            if (number > maxBlue) setOK = false;
                            if (number > mb) mb = number;
                        }
                        //if (!setOK) break;    // <- disabled for part 2
                    }
                    //if (!setOK) break;        // <- disabled for part 2
                }
                if (setOK) sum1 += id;

                int m = mr * mg * mb;
                sum2 += m;
                id++;
            }
            Console.WriteLine("RESULT 1: " + sum1);
            Console.WriteLine("RESULT 2: " + sum2);

            // PART 1: 2528
            // PART 2: 67363
        }
    }
}