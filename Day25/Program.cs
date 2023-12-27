namespace MyApp {
    internal class Program {
        public static string[] data;

        static void Main(string[] args) {
            string path = @"..\..\..\InputTest.txt";
            //string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            Dictionary<string, int> dict = new Dictionary<string, int>();

            // Generate IDs for verticles
            int id = 0;
            foreach (string line in data) {
                var items = line.Replace(" ", ",").Split(":,");
                var src = items[0];
                if (!dict.ContainsKey(src)) dict[src] = id++;
                var dst = items[1].Split(",");
                foreach (var d in dst) {
                    if (!dict.ContainsKey(d)) dict[d] = id++;
                }
            }

            //foreach (var d in dict.Keys) Console.WriteLine(d + " " + dict[d]);
            //Console.WriteLine("Count: " + dict.Count());
            Graph g = new Graph(dict.Count());
            foreach (string line in data) {
                var items = line.Replace(" ", ",").Split(":,");
                var src = items[0];
                var dst = items[1].Split(",");
                foreach (var d in dst) {
                    g.AddEdge(dict[src], dict[d]);
                }
            }

            var edges = g.GetAllEdges();
            g.PrintGraph();
            Console.WriteLine();

            foreach (var e in edges) { 
                Console.WriteLine(e.ToString());
            }

            /*// Usuwanie 2 krawędzi
            for (int i = 0; i < dict.Count - 2; i++) {
                for (int j = i + 1; j < dict.Count - 1; j++) {
                    Console.WriteLine("remove " + i + "-" + j);
                    
                    g.RemoveEdge(i, j);
                    g.RemoveEdge(i, j + 1);
                    
                    var bridges = g.FindBridges();
                    if (bridges.Count > 0) foreach (var bridge in bridges) Console.WriteLine(bridge.Item1 + " -- " + bridge.Item2);
                    
                    g.AddEdge(i, j);
                    g.AddEdge(i, j + 1);

                    g.PrintGraph();
                    Console.WriteLine();
                }
            }*/
        }

        public static void GenerateDot(string[] data) {
            List<string> graph = new List<string>();
            graph.Add("digraph {");
            foreach (string line in data) {
                var items = line.Replace(" ", ",").Split(":,");
                var src = items[0];
                var dst = items[1].Split(",");
                foreach (var d in dst) {
                    graph.Add(src + "->" + d);
                }
            }
            graph.Add("}");
            PrintGraph(graph);
        }

        public static void PrintGraph(List<string> graph) {
            foreach (var item in graph) {
                Console.WriteLine(item);
            }
        }
    }
}