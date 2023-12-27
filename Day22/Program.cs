using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace MyApp {
    class Brick {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int Z1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Z2 { get; set; }
        public bool Invisilbe { get; set; }

        public Brick Clone() {
            return (Brick)this.MemberwiseClone();
        }
    }

    internal class Program {
        public static string[] data;

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            List<Brick> brickList = new List<Brick>();

            // Add all bricks to the list
            foreach (string line in data) {
                string[] coords = line.Split("~");
                string[] c1 = coords[0].Split(",");
                string[] c2 = coords[1].Split(",");
                brickList.Add(new Brick {
                    X1 = int.Parse(c1[0]),
                    Y1 = int.Parse(c1[1]),
                    Z1 = int.Parse(c1[2]),
                    X2 = int.Parse(c2[0]),
                    Y2 = int.Parse(c2[1]),
                    Z2 = int.Parse(c2[2]),
                    Invisilbe = false
                });
            }

            // Sort by Z1
            brickList.Sort((a, b) => a.Z1.CompareTo(b.Z1));

            // Drop bricks
            foreach (Brick b in brickList) DropBrick(brickList, b);

            // PART 1
            int result1 = 0;
            foreach (Brick brick in brickList) { if (CanDisintegrate(brickList, brick)) result1++; }
            Console.WriteLine("RESULT 1: " + result1);

            // PART 2
            int result2 = 0;
            for (int i = 0; i < brickList.Count; i++) result2 += CountFalling(brickList, i);
            Console.WriteLine("RESULT 2: " + result2);

            // PART 1: 454
            // PART 2: 74287
        }

        public static bool DropBrick(List<Brick> brickList, Brick b) {
            bool dropped = false;
            while (true) {
                if (CanFall(brickList, b)) { b.Z1--; b.Z2--; dropped = true; }
                else break;
            }
            return dropped;
        }

        public static bool CanFall(List<Brick> brickList, Brick b) {
            if (b.Z1 == 1) return false;
            bool canFall = true;
            foreach (Brick c in brickList) {
                if (b.Z1 - 1 != c.Z2) continue;
                if (c.Invisilbe) continue;
                bool cx = ((b.X1 >= c.X1 && b.X1 <= c.X2) || (b.X2 >= c.X1 && b.X2 <= c.X2) || (b.X1 <= c.X1 && b.X2 >= c.X2)) ? true : false;
                bool cy = ((b.Y1 >= c.Y1 && b.Y1 <= c.Y2) || (b.Y2 >= c.Y1 && b.Y2 <= c.Y2) || (b.Y1 <= c.Y1 && b.Y2 >= c.Y2)) ? true : false;
                if (cx && cy) { canFall = false; break; }
            }
            return canFall;
        }

        public static bool CanDisintegrate(List<Brick> brickList, Brick b) {
            bool canDisintegrate = true;
            b.Invisilbe = true;
            foreach (Brick c in brickList) {
                if (c.Invisilbe) continue;
                if (CanFall(brickList, c)) { canDisintegrate = false; break; }
            }
            b.Invisilbe = false;
            return canDisintegrate;
        }

        public static int CountFalling(List<Brick> brickList, int idx) {
            // Clone list
            List<Brick> bl = new List<Brick>();
            foreach (Brick b in brickList) bl.Add(b.Clone());

            // Remove brick
            bl.RemoveAt(idx);

            // Count falling
            int droppedCnt = 0;
            foreach (Brick b in bl) if (DropBrick(bl, b)) droppedCnt++;
            return droppedCnt;
        }

        public static void PrintBricks(List<Brick> brickList) {
            foreach (Brick b in brickList) {
                Console.WriteLine(b.X1 + "," + b.Y1 + "," + b.Z1 + "~" + b.X2 + "," + b.Y2 + "," + b.Z2);
            }
        }
    }
}