using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    public class Lens {
        public string id { get; set; }
        public int focal { get; set; }
    }

    internal class Program {
        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            var data = File.ReadAllLines(path);
            string line = data[0];
            string[] strings = line.Split(',');

            int result1 = 0; foreach (string str in strings) result1 += CalcHash(str);

            Dictionary<byte, List<Lens>> boxList = new Dictionary<byte, List<Lens>>();

            foreach (string str in strings) {
                int oper = str.Contains('-') ? 0 : 1;  // '-' Remove, '=' Add/Replace
                string[] lensParams = str.Replace('-', '=').Split('=');
                string lensID = lensParams[0];
                byte boxID = CalcHash(lensID);

                if (!boxList.ContainsKey(boxID)) boxList[boxID] = new List<Lens>();
                var lenses = boxList[boxID];

                if (oper == 0) {
                    // Remove
                    var lensToRemove = lenses.FirstOrDefault(lens => lens.id == lensID);
                    if (lensToRemove != null) lenses.Remove(lensToRemove);
                }
                if (oper == 1) {
                    // Add / Replace
                    int lensFocal = int.Parse(lensParams[1]);
                    var lens = lenses.FirstOrDefault(lens => lens.id == lensID);
                    if (lens == null) lenses.Add(new Lens() { id = lensID, focal = lensFocal });
                    else lens.focal = lensFocal;
                }
            }

            //PrintDictionary(boxList);

            // Calculate score
            int result2 = 0;
            foreach (var box in boxList) {
                var lenses = box.Value;
                for (int i = 0; i < lenses.Count; i++) {
                    result2 += (box.Key + 1) * (i + 1) * lenses[i].focal;
                }
            }

            Console.WriteLine("RESULT 1: " + result1);
            Console.WriteLine("RESULT 2: " + result2);

            // RESULT 1: 512950
            // RESULT 2: 247153
        }

        public static byte CalcHash(string sequence) {
            byte hash = 0;
            foreach (char chr in sequence) hash = (byte)(((hash + (int)chr) * 17) % 256);
            return hash;
        }

        public static void PrintDictionary(Dictionary<byte, List<Lens>> boxList) {
            foreach (var box in boxList) {
                if (box.Value.Count > 0) {
                    Console.Write("Box " + box.Key + ":");
                    foreach (var lens in box.Value) Console.Write(" [" + lens.id + " " + lens.focal + "]");
                    Console.WriteLine("");
                }
            }
        }
    }
}