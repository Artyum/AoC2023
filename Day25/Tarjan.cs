using System;
using System.Collections.Generic;

class Graph {
    private int vertices;       // Liczba wierzchołków
    private List<int>[] adj;    // Lista sąsiedztwa
    private int time;           // Zmienna czasowa używana w DFS

    public Graph(int v) {
        vertices = v;
        adj = new List<int>[v];
        for (int i = 0; i < v; ++i)
            adj[i] = new List<int>();
        time = 0;
    }

    // Funkcja do dodawania krawędzi
    public void AddEdge(int v, int w) {
        adj[v].Add(w);
        adj[w].Add(v);
    }

    public void RemoveEdge(int v, int w) {
        adj[v].Remove(w);
        adj[w].Remove(v);
    }

    public bool EdgeExist(int v, int w) {
        return adj[v].Contains(w) && adj[w].Contains(v);
    }

    public List<Tuple<int, int>> GetAllEdges() {
        List<Tuple<int, int>> allEdges = new List<Tuple<int, int>>();
        HashSet<Tuple<int, int>> addedEdges = new HashSet<Tuple<int, int>>();

        for (int i = 0; i < vertices; i++) {
            foreach (int j in adj[i]) {
                // Tworzenie uporządkowanej krawędzi (mniejszy wierzchołek pierwszy)
                var edge = new Tuple<int, int>(Math.Min(i, j), Math.Max(i, j));

                // Dodanie krawędzi, jeśli nie została jeszcze dodana
                if (!addedEdges.Contains(edge)) {
                    allEdges.Add(edge);
                    addedEdges.Add(edge); // Zapamiętaj, że ta krawędź została dodana
                }
            }
        }

        return allEdges;
    }

    // Rekurencyjna funkcja używana przez FindBridges
    private void BridgeUtil(int u, bool[] visited, int[] disc, int[] low, int?[] parent, List<Tuple<int, int>> bridges) {
        // Oznacz wierzchołek jako odwiedzony
        visited[u] = true;

        // Inicjalizacja czasu odkrycia i low
        disc[u] = low[u] = ++time;

        // Przejście po wszystkich wierzchołkach przyległych do tego wierzchołka
        foreach (int v in adj[u]) {
            if (!visited[v]) {
                parent[v] = u;
                BridgeUtil(v, visited, disc, low, parent, bridges);

                // Sprawdzenie, czy poddrzewo z wierzchołkiem v ma połączenie z jednym z przodków u
                low[u] = Math.Min(low[u], low[v]);

                // Jeżeli najniższy wierzchołek, który może być odwiedzony z v
                // jest wyższy niż czas odkrycia u, to u-v jest mostem
                if (low[v] > disc[u]) {
                    bridges.Add(Tuple.Create(u, v));
                }
            }
            // Aktualizacja low wartości dla u dla wierzchołków rodzica
            else if (v != parent[u]) {
                low[u] = Math.Min(low[u], disc[v]);
            }
        }
    }

    // Funkcja do znalezienia mostów w grafie
    // Wykorzystuje rekurencyjną funkcję BridgeUtil
    public List<Tuple<int, int>> FindBridges() {
        bool[] visited = new bool[vertices];
        int[] disc = new int[vertices];
        int[] low = new int[vertices];
        int?[] parent = new int?[vertices];
        List<Tuple<int, int>> bridges = new List<Tuple<int, int>>();

        // Inicjalizacja wierzchołków jako nieodwiedzonych
        for (int i = 0; i < vertices; i++) {
            parent[i] = null;
            visited[i] = false;
        }

        // Wywołanie rekurencyjnej funkcji pomocniczej
        // dla każdego nieodwiedzonego wierzchołka
        for (int i = 0; i < vertices; i++)
            if (!visited[i])
                BridgeUtil(i, visited, disc, low, parent, bridges);

        return bridges;
    }

    public void PrintGraph() {
        for (int v = 0; v < vertices; v++) {
            Console.Write(v + " -> ");
            foreach (var neighbor in adj[v]) {
                Console.Write(neighbor + " ");
            }
            Console.WriteLine();
        }
    }
}
/*
class Program {
    static void Main() {
        Console.WriteLine("Mosty w grafie:");

        // Tworzenie grafu przykładowego
        Graph g = new Graph(5);
        g.AddEdge(1, 0);
        g.AddEdge(0, 2);
        g.AddEdge(2, 1);
        g.AddEdge(0, 3);
        g.AddEdge(3, 4);

        // Wyszukiwanie mostów
        var bridges = g.FindBridges();
        foreach (var bridge in bridges) {
            Console.WriteLine(bridge.Item1 + " -- " + bridge.Item2);
        }
    }
}
*/