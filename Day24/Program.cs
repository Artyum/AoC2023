using static VectorUtils;

namespace MyApp {
    internal class Program {
        public static string[] data;
        
        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            List<Vector> vectorList = new List<Vector>();

            foreach (string line in data) {
                string[] item = line.Replace(" ", "").Split("@");
                string[] pnt = item[0].Split(",");
                string[] vel = item[1].Split(",");
                Vector v = new Vector() {
                    X = long.Parse(pnt[0]),
                    Y = long.Parse(pnt[1]),
                    Z = 0,
                    //Z = long.Parse(pnt[2]),

                    Vx = long.Parse(vel[0]),
                    Vy = long.Parse(vel[1]),
                    Vz = 0
                    //Vz = long.Parse(vel[2])

                };
                vectorList.Add(v);
            }

            //long boxMin = 7;
            //long boxMax = 27;
            long boxMin = 200000000000000;
            long boxMax = 400000000000000;

            long sum = 0;
            for (int i = 0; i < vectorList.Count - 1; i++) {
                for (int j = i + 1; j < vectorList.Count; j++) {
                    var (intersects, x, y, z, t, s) = IntersectionPoint(vectorList[i], vectorList[j]);
                    if (intersects) {
                        if (x >= boxMin && x <= boxMax &&
                            y >= boxMin && y <= boxMax &&
                            t > 0 && s > 0) {
                            sum++;
                        }
                        //Console.WriteLine(i + "->" + j + " (" + x + ";" + y + ";" + z + ") t=" + t + " s=" + s);
                    }
                }
            }
            Console.WriteLine("RESULT 1: " + sum);

            //RESULT 1: 16502
        }
    }
}