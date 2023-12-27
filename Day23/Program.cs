using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace MyApp {
    internal class Program {
        public static string[] data;

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            int nodes = data[0].Length * data.Length;
            Graph g = new Graph(nodes);

            // Build adjacency matrix
            for (int r = 0; r < data.Length; r++) {

                // Uncomment for PART 2
                //data[r] = data[r].Replace('>', '.').Replace('<', '.').Replace('v', '.').Replace('^', '.');
                // -------------------

                for (int c = 0; c < data[r].Length; c++) {
                    if (data[r][c] == '#') continue;

                    int i = r * data[r].Length + c;
                    int iu = i - data[0].Length;
                    int ir = i + 1;
                    int id = i + data[0].Length;
                    int il = i - 1;

                    // UP
                    if (r > 0) { if (data[r - 1][c] == '.' || data[r - 1][c] == '^') g.AddEdge(i, iu); }

                    // RIGHT
                    if (c < data[0].Length - 1) { if (data[r][c + 1] == '.' || data[r][c + 1] == '>') g.AddEdge(i, ir); }

                    // DOWN
                    if (r < data.Length - 1) { if (data[r + 1][c] == '.' || data[r + 1][c] == 'v') g.AddEdge(i, id); }

                    // LEFT
                    if (c > 0) { if (data[r][c - 1] == '.' || data[r][c - 1] == '<') g.AddEdge(i, il); }
                }
            }

            int start = 1;
            int end = nodes - 2;

            g.FindAllPaths(start, end);

            Console.WriteLine("Total paths:  " + g.GetPathsNumber());
            Console.WriteLine("Fewest steps: " + (g.GetShortestLength() - 1));
            Console.WriteLine("Most steps:   " + (g.GetLongestLength() - 1));

            /*
            TEST PART 1
                Total paths:  6
                Fewest steps: 74
                Most steps:   94

            TEST PART 2
                Total paths:  12
                Fewest steps: 74
                Most steps:   154
            
            FULL PART 1
                Total paths:  252
                Fewest steps: 1338
                Most steps:   2430   <---- RESULT 1

            FULL PART 2
                Total paths : 1262816
                Fewest steps: 1338
                Most steps:   6534   <---- RESULT 2
            */
        }
    }
}