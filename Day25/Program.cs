namespace MyApp {
    internal class Program {
        public static string[] data;

        static void Main(string[] args) {
            //string path = @"..\..\..\InputTest.txt";
            string path = @"..\..\..\InputFull.txt";
            data = File.ReadAllLines(path);

            Dictionary<string, int> dict = new Dictionary<string, int>();

            // Generate IDs for verticles
            int id = 0;
            foreach (string line in data) {
                var items = line.Replace(" ", ",").Split(":,");
                var src = items[0];
                if (!dict.ContainsKey(src)) dict[src] = id++;
                var dst = items[1].Split(",");
                foreach (var d in dst) if (!dict.ContainsKey(d)) dict[d] = id++;
            }

            //foreach (var d in dict.Keys) Console.WriteLine(d + " " + dict[d]);
            Console.WriteLine("Nodes: " + dict.Count());

            Graph g = new Graph(dict.Count());
            foreach (string line in data) {
                var items = line.Replace(" ", ",").Split(":,");
                var src = items[0];
                var dst = items[1].Split(",");
                foreach (var d in dst) g.AddEdge(dict[src], dict[d]);
            }

            var edges = g.GetAllEdges();

            //foreach (var e in edges) Console.WriteLine(e);
            Console.WriteLine("Edges: " + edges.Count);

            // Remove each pair of edges and check for a single bridge in the graph
            // It may take few minutes (:
            Console.WriteLine("Searching for 3 bridges...");
            List<Tuple<int, int>> bridgeList = new List<Tuple<int, int>>();
            bool end = false;
            for (int i = 0; i < edges.Count - 2 && !end; i++) {
                g.RemoveEdge(edges[i].Item1, edges[i].Item2);
                for (int j = i + 1; j < edges.Count - 1 && !end; j++) {
                    g.RemoveEdge(edges[j].Item1, edges[j].Item2);
                    var bridges = g.FindBridges();
                    if (bridges.Count == 1) {
                        Console.WriteLine("Bridge 1: " + Node(dict, edges[i].Item1) + "--" + Node(dict, edges[i].Item2));
                        Console.WriteLine("Bridge 2: " + Node(dict, edges[j].Item1) + "--" + Node(dict, edges[j].Item2));
                        Console.WriteLine("Bridge 3: " + Node(dict, bridges[0].Item1) + "--" + Node(dict, bridges[0].Item2));
                        bridgeList.Add(Tuple.Create(edges[i].Item1, edges[i].Item2));
                        bridgeList.Add(Tuple.Create(edges[j].Item1, edges[j].Item2));
                        bridgeList.Add(Tuple.Create(bridges[0].Item1, bridges[0].Item2));
                        end = true;
                    }
                    g.AddEdge(edges[j].Item1, edges[j].Item2);
                }
                g.AddEdge(edges[i].Item1, edges[i].Item2);
            }

            // Remove 3 bridges
            foreach (var edge in bridgeList) g.RemoveEdge(edge.Item1, edge.Item2);

            var components = g.FindConnectedComponents();
            int result = 1;
            foreach (var size in components) result *= size;
            Console.WriteLine("RESULT: " + result);

            // RESULT 1: 495607
        }

        // Generate dot diagram for Graphviz
        // http://magjac.com/graphviz-visual-editor/
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

        public static string Node(Dictionary<string, int> dict, int i) {
            foreach (var item in dict) {
                if (item.Value == i) return item.Key;
            }
            return "";
        }
    }
}