class Graph {
    private int vertices;       // Number of vertices
    private List<int>[] adj;    // Adjacency list
    private int time;           // Time variable used in DFS

    public Graph(int v) {
        vertices = v;
        adj = new List<int>[v];
        for (int i = 0; i < v; ++i) adj[i] = new List<int>();
        time = 0;
    }

    // Function to add an edge
    public void AddEdge(int v, int w) {
        adj[v].Add(w);
        adj[w].Add(v);
    }

    // Function to remove an edge
    public void RemoveEdge(int v, int w) {
        adj[v].Remove(w);
        adj[w].Remove(v);
    }

    // Function to check if an edge exists
    public bool EdgeExist(int v, int w) {
        return adj[v].Contains(w) && adj[w].Contains(v);
    }

    // Function to get all edges
    public List<Tuple<int, int>> GetAllEdges() {
        List<Tuple<int, int>> allEdges = new List<Tuple<int, int>>();
        HashSet<Tuple<int, int>> addedEdges = new HashSet<Tuple<int, int>>();

        for (int i = 0; i < vertices; i++) {
            foreach (int j in adj[i]) {
                // Create an ordered edge (smaller vertex first)
                var edge = new Tuple<int, int>(Math.Min(i, j), Math.Max(i, j));

                // Add the edge if it has not been added yet
                if (!addedEdges.Contains(edge)) {
                    allEdges.Add(edge);
                    addedEdges.Add(edge); // Remember that this edge was added
                }
            }
        }

        return allEdges;
    }

    // Recursive function used by FindBridges
    private void BridgeUtil(int u, bool[] visited, int[] disc, int[] low, int?[] parent, List<Tuple<int, int>> bridges) {
        // Mark the vertex as visited
        visited[u] = true;

        // Initialize discovery time and low value
        disc[u] = low[u] = ++time;

        // Go through all vertices adjacent to this one
        foreach (int v in adj[u]) {
            if (!visited[v]) {
                parent[v] = u;
                BridgeUtil(v, visited, disc, low, parent, bridges);

                // Check if the subtree with vertex v has a connection to one of the ancestors of u
                low[u] = Math.Min(low[u], low[v]);

                // If the lowest vertex that can be visited from v is higher than the discovery time of u, then u-v is a bridge
                if (low[v] > disc[u]) bridges.Add(Tuple.Create(u, v));
            }
            // Update low value for u for parent vertices
            else if (v != parent[u]) low[u] = Math.Min(low[u], disc[v]);
        }
    }

    // Function to find bridges in the graph
    public List<Tuple<int, int>> FindBridges() {
        bool[] visited = new bool[vertices];
        int[] disc = new int[vertices];
        int[] low = new int[vertices];
        int?[] parent = new int?[vertices];
        List<Tuple<int, int>> bridges = new List<Tuple<int, int>>();

        // Initialize vertices as not visited
        for (int i = 0; i < vertices; i++) {
            parent[i] = null;
            visited[i] = false;
        }

        // Call the recursive helper function for each unvisited vertex
        for (int i = 0; i < vertices; i++) if (!visited[i]) BridgeUtil(i, visited, disc, low, parent, bridges);

        return bridges;
    }

    // Function to print the graph
    public void PrintGraph() {
        for (int v = 0; v < vertices; v++) {
            Console.Write(v + " -> ");
            foreach (var neighbor in adj[v]) Console.Write(neighbor + " ");
            Console.WriteLine();
        }
    }

    // DFSUtil for counting vertices in connected components
    private int DFSUtil(int v, bool[] visited) {
        visited[v] = true;  // Mark this vertex as visited
        int size = 1;       // Each vertex has size 1

        // Recursive call for all neighbors
        foreach (int i in adj[v]) {
            if (!visited[i]) size += DFSUtil(i, visited); // Add the size of the connected component
        }

        return size; // Return the size of this connected component
    }

    // Function to find connected components and their sizes
    public List<int> FindConnectedComponents() {
        bool[] visited = new bool[vertices];
        List<int> componentSizes = new List<int>(); // List to store sizes of connected components

        for (int v = 0; v < vertices; v++) {
            if (!visited[v]) {
                // Run DFS from an unvisited vertex and add the size of the component to the list
                componentSizes.Add(DFSUtil(v, visited));
            }
        }

        return componentSizes; // Return the list of sizes of connected components
    }
}

/*
// EXAMPLE
Graph g = new Graph(5);
g.AddEdge(1, 0);
g.AddEdge(0, 2);
g.AddEdge(2, 1);
g.AddEdge(0, 3);
g.AddEdge(3, 4);
*/