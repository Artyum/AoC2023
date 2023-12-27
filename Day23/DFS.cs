class Graph {
    private int vertices;       // Number of vertices
    private List<int>[] adj;    // Adjacency list for each vertex
    private List<List<int>> allPaths = new List<List<int>>(); // List to store all paths found

    public Graph(int v) {
        vertices = v;
        adj = new List<int>[v];
        for (int i = 0; i < v; ++i)
            adj[i] = new List<int>();
    }

    // Function to add an edge into the graph
    public void AddEdge(int v, int w) {
        adj[v].Add(w); // Add w to v's list.
    }

    // A recursive function used by DFS
    private void DFSUtil(int v, bool[] visited, List<int> path, int d) {
        // Mark the current node as visited and add it to path
        visited[v] = true;
        path.Add(v);

        // If current vertex is the destination, add the path to allPaths
        if (v == d) {
            allPaths.Add(new List<int>(path));
        }
        else {
            // Recur for all the vertices adjacent to this vertex
            List<int> iList = adj[v];
            foreach (var n in iList) if (!visited[n]) DFSUtil(n, visited, path, d);
        }

        // Remove current vertex from path and mark it as unvisited
        path.RemoveAt(path.Count - 1);
        visited[v] = false;
    }

    // Calls DFSUtil() to find all paths from 's' to 'd'
    public void FindAllPaths(int s, int d) {
        bool[] visited = new bool[vertices];
        List<int> pathList = new List<int>();

        // Call the recursive helper function to print all paths
        DFSUtil(s, visited, pathList, d);
    }

    // Function to print all paths stored in allPaths
    public void PrintPaths() {
        foreach (var path in allPaths) {
            foreach (var v in path) Console.Write(v + "->");
        }
    }

    public int GetPathsNumber() {
        return allPaths.Count;
    }

    public int GetShortestLength() {
        int l = int.MaxValue;
        foreach (var path in allPaths) if (path.Count < l) l = path.Count;
        return l;
    }

    public int GetLongestLength() {
        int l = -1;
        foreach (var path in allPaths) if (path.Count > l) l = path.Count;
        return l;
    }
}