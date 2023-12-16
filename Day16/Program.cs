using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp {
    public class Point {
        public int row { get; set; }
        public int col { get; set; }
    }

    public class Ray {
        public Point point { get; set; }
        public int dir { get; set; } // 0-R  1-D  2-L  3-U
    }

    public class EnergyPoint {
        public Point point { get; set; }
        public int energy { get; set; }
    }

    internal class Program {
        public static string[] data;
        public static List<Ray> rayList = new List<Ray>();
        public static List<EnergyPoint> energyList = new List<EnergyPoint>();
        public static List<Point> splitterList = new List<Point>();

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            Point startPoint = new Point();
            int maxEnergy = 0;

            // Directions: 0-R  1-D  2-L  3-U

            // PART 1
            startPoint.row = 0;
            startPoint.col = 0;
            rayList.Add(new Ray { point = startPoint, dir = 0 });
            while (rayList.Count > 0) RayTracing();
            int energy = energyList.Count;
            splitterList.Clear();
            energyList.Clear();

            // PART 2
            Console.WriteLine("Top row down");
            for (int i = 0; i < data[0].Length; i++) {
                startPoint.row = 0;
                startPoint.col = i;
                rayList.Add(new Ray { point = startPoint, dir = 1 });
                while (rayList.Count > 0) RayTracing();
                if (maxEnergy < energyList.Count) maxEnergy = energyList.Count;
                Console.WriteLine("i=" + i + "  Energy: " + energyList.Count);
                splitterList.Clear();
                energyList.Clear();
            }

            // Bottom row up
            Console.WriteLine("Bottom row up");
            for (int i = 0; i < data[0].Length; i++) {
                startPoint.row = data.Length - 1;
                startPoint.col = i;
                rayList.Add(new Ray { point = startPoint, dir = 3 });
                while (rayList.Count > 0) RayTracing();
                if (maxEnergy < energyList.Count) maxEnergy = energyList.Count;
                Console.WriteLine("i=" + i + "  Energy: " + energyList.Count);
                splitterList.Clear();
                energyList.Clear();
            }

            // Left col to the right
            Console.WriteLine("Left col to the right");
            for (int i = 0; i < data.Length; i++) {
                startPoint.row = i;
                startPoint.col = 0;
                rayList.Add(new Ray { point = startPoint, dir = 0 });
                while (rayList.Count > 0) RayTracing();
                if (maxEnergy < energyList.Count) maxEnergy = energyList.Count;
                Console.WriteLine("i=" + i + "  Energy: " + energyList.Count);
                splitterList.Clear();
                energyList.Clear();
            }

            // Right col to the left
            Console.WriteLine("Right col to the left");
            for (int i = 0; i < data.Length; i++) {
                startPoint.row = 0;
                startPoint.col = data[0].Length - 1;
                rayList.Add(new Ray { point = startPoint, dir = 2 });
                while (rayList.Count > 0) RayTracing();
                if (maxEnergy < energyList.Count) maxEnergy = energyList.Count;
                Console.WriteLine("i=" + i + "  Energy: " + energyList.Count);
                splitterList.Clear();
                energyList.Clear();
            }

            Console.WriteLine("RESULT 1: " + energy);
            Console.WriteLine("RESULT 2: " + maxEnergy);

            // RESULT 1: 8034
            // RESULT 2: 8225
        }

        static void RayTracing() {
            for (int idx = 0; idx < rayList.Count; idx++) {
                var ray = rayList[idx];
                var p = ray.point;

                // Directions: 0-R  1-D  2-L  3-U

                bool endRay = false;
                AddEnergy(p.row, p.col);

                // Change of direction
                if (data[p.row][p.col] == '\\') {
                    if (ray.dir == 0) ray.dir = 1;       // R->D
                    else if (ray.dir == 1) ray.dir = 0;  // D->R
                    else if (ray.dir == 2) ray.dir = 3;  // L->U
                    else if (ray.dir == 3) ray.dir = 2;  // U->L
                }
                else if (data[p.row][p.col] == '/') {
                    if (ray.dir == 0) ray.dir = 3;       // R->U
                    else if (ray.dir == 1) ray.dir = 2;  // D->L
                    else if (ray.dir == 2) ray.dir = 1;  // L->D
                    else if (ray.dir == 3) ray.dir = 0;  // U->R
                }
                // If the beam hits the splitter, it goes out and the splitter emits two new beams
                else if (data[p.row][p.col] == '|') {
                    if (ray.dir == 0 || ray.dir == 2) {  // R->L | L->R
                        if (!splitterList.Any(n => n.row == p.row && n.col == p.col)) {
                            splitterList.Add(new Point { row = p.row, col = p.col });
                            if (p.row > 0) rayList.Add(new Ray() { point = new Point { row = p.row, col = p.col }, dir = 3 });                  // ->U
                            if (p.row < data.Length - 1) rayList.Add(new Ray() { point = new Point { row = p.row, col = p.col }, dir = 1 });    // ->D
                        }
                        endRay = true;
                    }
                }
                else if (data[p.row][p.col] == '-') {
                    if (ray.dir == 1 || ray.dir == 3) {  // U->D | D->U
                        if (!splitterList.Any(n => n.row == p.row && n.col == p.col)) {
                            splitterList.Add(new Point { row = p.row, col = p.col });
                            if (p.col > 0) rayList.Add(new Ray() { point = new Point { row = p.row, col = p.col }, dir = 2 });                  // ->L
                            if (p.col < data[0].Length - 1) rayList.Add(new Ray() { point = new Point { row = p.row, col = p.col }, dir = 0 }); // ->R
                        }
                        endRay = true;
                    }
                }

                // Move ray
                if (!endRay) {
                    if (ray.dir == 0) { if (p.col < data[0].Length - 1) p.col++; else endRay = true; }
                    else if (ray.dir == 1) { if (p.row < data.Length - 1) p.row++; else endRay = true; }
                    else if (ray.dir == 2) { if (p.col > 0) p.col--; else endRay = true; }
                    else if (ray.dir == 3) { if (p.row > 0) p.row--; else endRay = true; }
                }

                if (endRay) rayList.Remove(ray);
            }
        }

        static void AddEnergy(int row, int col) {
            var p = energyList.LastOrDefault(n => n.point.row == row && n.point.col == col);
            if (p == null) energyList.Add(new EnergyPoint { point = new Point { row = row, col = col }, energy = 1 });
            else p.energy++;
        }

        static void PrintGrid() {
            for (int r = 0; r < data.Length; r++) {
                for (int c = 0; c < data[r].Length; c++) {
                    //if (energyList.Any(n => n.Row == r && n.Col == c) && data[r][c] == '.') Console.Write('#');
                    if (energyList.Any(n => n.point.row == r && n.point.col == c)) Console.Write('#');
                    else Console.Write(data[r][c]);
                }
                Console.WriteLine();
            }
        }
    }
}