namespace Training.TheLabyrinth
{
    using Console = CodinGameEmulator.Console;

    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /**
     * Auto-generated code below aims at helping you parse
     * the standard input according to the problem statement.
     **/
    class Player
    {
        public static void Main(string[] args)
        {
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            int rowCount = int.Parse(inputs[0]); // number of rows.
            int colCount = int.Parse(inputs[1]); // number of columns.
            int countdown = int.Parse(inputs[2]); // number of rounds between the time the alarm countdown is activated and the time the alarm goes off.
            bool commandCenterFound = false;

            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');
                int y = int.Parse(inputs[0]); // row where Rick is located.
                int x = int.Parse(inputs[1]); // column where Rick is located.

                var graph = BuildGraph(rowCount, colCount);
                DisplayGraph(graph, rowCount, colCount);

                var position = graph.AdjacencyList.Keys.Single(v => v.X == x && v.Y == y);
                Console.Error.WriteLine($"Position: {position.ToString()}");

                if (position.Content == EVertexContent.CommandCenter)
                    commandCenterFound = true;

                var bfs = graph.BreadthFirstSearch(position, x => x.Content == EVertexContent.Unknown);
                Console.Error.WriteLine($"BFS => Count:{bfs.Count}");

                EVertexContent? targetType = null;
                if (bfs.Any(x => x.Key.Content == EVertexContent.Unknown)) //Still undiscovered places
                {
                    targetType = EVertexContent.Unknown;
                }
                else if (!commandCenterFound)
                {
                    targetType = EVertexContent.CommandCenter;
                }
                else
                {
                    targetType = EVertexContent.StartPosition;
                }

                var target = bfs.Keys.First(v => v.Content == targetType);
                var path = graph.GetPath(bfs, position, target);
                var next = path.Skip(1).First();
                var command = GetDirection(position, next);
                Console.Error.WriteLine($"Target: {target.ToString()} /  Next: {next.ToString()}");

                Console.WriteLine(command);
            }
        }

        private static string GetDirection(Vertex start, Vertex target)
        {
            if (start.X < target.X)
                return "RIGHT";
            if (start.X > target.X)
                return "LEFT";
            if (start.Y < target.Y)
                return "DOWN";
            if (start.Y > target.Y)
                return "UP";

            throw new Exception();
        }

        private static Vertex GetNextStep(Dictionary<Vertex, Vertex> bfs, Vertex start, Vertex target)
        {
            Vertex current = target;
            while (bfs[current] != start)
            {
                current = bfs[current];
            }
            return current;
        }

        private static Graph<Vertex> BuildGraph(int rowCount, int colCount)
        {
            List<Vertex> vertices = new List<Vertex>();
            for (int y = 0; y < rowCount; y++)
            {
                var row = Console.ReadLine().ToCharArray();
                for (int x = 0; x < colCount; x++)
                {
                    vertices.Add(new Vertex(x, y, row[x]));
                }
            }

            List<Edge<Vertex>> edges = new List<Edge<Vertex>>();
            var nonWallVertices = vertices.Where(v => v.Content != EVertexContent.Wall);
            foreach (var vertex in nonWallVertices)
            {
                var northNeighbor = nonWallVertices.SingleOrDefault(v => v.X == vertex.X && v.Y == vertex.Y - 1);
                var southNeighbor = nonWallVertices.SingleOrDefault(v => v.X == vertex.X && v.Y == vertex.Y + 1);
                var eastNeighbor = nonWallVertices.SingleOrDefault(v => v.X == vertex.X + 1 && v.Y == vertex.Y);
                var westNeighbor = nonWallVertices.SingleOrDefault(v => v.X == vertex.X - 1 && v.Y == vertex.Y);

                if (northNeighbor is not null)
                    edges.Add(new Edge<Vertex>(vertex, northNeighbor, 0));

                if (southNeighbor is not null)
                    edges.Add(new Edge<Vertex>(vertex, southNeighbor, 0));

                if (eastNeighbor is not null)
                    edges.Add(new Edge<Vertex>(vertex, eastNeighbor, 0));

                if (westNeighbor is not null)
                    edges.Add(new Edge<Vertex>(vertex, westNeighbor, 0));
            }

            return new Graph<Vertex>(vertices, edges);
        }

        private static void DisplayGraph(Graph<Vertex> graph, int rowCount, int colCount)
        {
            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < colCount; x++)
                {
                    var vertex = graph.AdjacencyList.Keys.Single(v => v.X == x && v.Y == y);
                    char c = vertex.Content switch
                    {
                        EVertexContent.Empty => '.',
                        EVertexContent.CommandCenter => 'C',
                        EVertexContent.StartPosition => 'T',
                        EVertexContent.Unknown => '?',
                        EVertexContent.Wall => '#',
                        _ => throw new ArgumentException(nameof(vertex.Content)),
                    };
                    Console.Error.Write(c);
                }

                Console.Error.WriteLine();
            }
        }
    }

    interface IVertex
    {
        int X { get; set; }
        int Y { get; set; }
    }

    public enum EVertexContent
    {
        Unknown,
        Wall,
        Empty,
        StartPosition,
        CommandCenter
    }

    class Vertex : IVertex
    {
        public int X { get; set; }
        public int Y { get; set; }
        public EVertexContent Content { get; set; }

        public Vertex(int x, int y, char c)
        {
            X = x;
            Y = y;
            Content = c switch
            {
                '.' => EVertexContent.Empty,
                'T' => EVertexContent.StartPosition,
                'C' => EVertexContent.CommandCenter,
                '?' => EVertexContent.Unknown,
                '#' => EVertexContent.Wall,
                _ => throw new ArgumentException(nameof(c))
            };
        }

        public override bool Equals(object? obj)
        {
            return obj == this ||
                (obj is Vertex vertex && vertex.X == this.X && vertex.Y == this.Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"Id:({X},{Y})({Content.ToString()})";
        }
    }

    class Edge<V> where V : IVertex
    {
        public V Vertex { get; }
        public V Neighbor { get; }
        public int Cost { get; }

        public Edge(V vertex, V neighbor, int cost)
        {
            Vertex = vertex;
            Neighbor = neighbor;
            Cost = cost;
        }

        public override bool Equals(object? obj)
        {
            return this == obj ||
                (
                    obj is Edge<V> edge &&
                    this.Vertex.Equals(edge.Vertex) &&
                    this.Neighbor.Equals(edge.Neighbor)
                );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Vertex, Neighbor);
        }
    }

    class Graph<V> where V : IVertex
    {
        public Graph() { }

        public Graph(IEnumerable<V> vertices, IEnumerable<Edge<V>> edges)
        {
            foreach (var vertex in vertices)
                AddVertex(vertex);

            foreach (var edge in edges)
                AddEdge(edge);
        }

        public Dictionary<V, HashSet<Edge<V>>> AdjacencyList { get; } = new Dictionary<V, HashSet<Edge<V>>>();

        public bool ContainsVertex(V vertex)
        {
            return AdjacencyList.ContainsKey(vertex);
        }

        public Edge<V>? GetEdge(V v1, V v2)
        {
            if (ContainsVertex(v1) && ContainsVertex(v2))
            {
                return AdjacencyList[v1].SingleOrDefault(e => e.Neighbor.Equals(v2));
            }
            return null;
        }

        public void AddVertex(V vertex)
        {
            if (!ContainsVertex(vertex))
                AdjacencyList[vertex] = new HashSet<Edge<V>>();
        }

        public void RemoveVertex(V vertex)
        {
            if (ContainsVertex(vertex))
                AdjacencyList.Remove(vertex);


            foreach (var otherVertex in AdjacencyList.Where(kvp => !kvp.Key.Equals(vertex)))
            {
                var edge = otherVertex.Value.SingleOrDefault(e => e.Neighbor.Equals(vertex));
                if (edge is not null)
                    otherVertex.Value.Remove(edge);
            }
        }

        public void AddEdge(Edge<V> edge)
        {
            if (ContainsVertex(edge.Vertex) &&
                ContainsVertex(edge.Neighbor) &&
                !AdjacencyList[edge.Vertex].Any(e => e.Equals(edge)))
            {
                AdjacencyList[edge.Vertex].Add(edge);
            }
        }

        public void RemoveEdge(V v1, V v2, bool deleteTwoWays)
        {
            var edge = GetEdge(v1, v2);
            if (edge is not null)
            {
                AdjacencyList[v1].Remove(edge);
            }
            edge = GetEdge(v2, v1);
            if (deleteTwoWays && edge is not null)
            {
                AdjacencyList[v2].Remove(edge);
            }
        }

        public Graph<V> Clone()
        {
            return new Graph<V>(
                this.AdjacencyList.Keys,
                this.AdjacencyList.SelectMany(x => x.Value));
        }

        public List<V> GetPath(Dictionary<V, V> predecessors, V start, V target)
        {
            List<V> path = new List<V>() { target };

            var current = target;
            while (!predecessors[current].Equals(start))
            {
                path.Add(predecessors[current]);
                current = predecessors[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        #region Graph Traverval Algorithms

        public Dictionary<V, V> DepthFirstSearch(V start)
        {
            var visited = new HashSet<V>();
            Dictionary<V, V> predecessors = new Dictionary<V, V>();

            if (this.ContainsVertex(start))
            {
                var stack = new Stack<V>();
                stack.Push(start);

                while (stack.Count() > 0)
                {
                    var vertex = stack.Pop();
                    if (!visited.Contains(vertex))
                    {
                        visited.Add(vertex);
                        foreach (var edge in AdjacencyList[vertex])
                        {
                            predecessors[edge.Neighbor] = vertex;
                            stack.Push(edge.Neighbor);
                        }
                    }
                }
            }

            return predecessors;
        }

        public Dictionary<V, V> BreadthFirstSearch(V start, Func<V, bool>? isOnfrontier = null)
        {
            var visited = new HashSet<V>();
            Dictionary<V, V> predecessors = new Dictionary<V, V>();

            if (this.ContainsVertex(start))
            {
                var queue = new Queue<V>();
                queue.Enqueue(start);

                while (queue.Count > 0)
                {
                    var vertex = queue.Dequeue();
                    visited.Add(vertex);

                    if (isOnfrontier is null || !isOnfrontier(vertex))
                    {
                        foreach (var edge in this.AdjacencyList[vertex])
                        {
                            if (!visited.Contains(edge.Neighbor))
                            {
                                predecessors[edge.Neighbor] = vertex;
                                queue.Enqueue(edge.Neighbor);
                            }
                        }
                    }
                }
            }

            return predecessors;
        }

        public Func<V, IEnumerable<V>> ShortestPathFunction(V start)
        {
            var predecessors = new Dictionary<V, V>();

            var queue = new Queue<V>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();
                foreach (var edge in this.AdjacencyList[vertex])
                {
                    if (predecessors.ContainsKey(edge.Neighbor))
                        continue;

                    predecessors[edge.Neighbor] = vertex;
                    queue.Enqueue(edge.Neighbor);
                }
            }

            Func<V, IEnumerable<V>> shortestPath = v =>
            {
                var path = new List<V> { };

                var current = v;
                while (!current.Equals(start))
                {
                    path.Add(current);
                    current = predecessors[current];
                };

                path.Add(start);
                path.Reverse();

                return path;
            };

            return shortestPath;
        }

        public (Dictionary<V, double> costs, Dictionary<V, V> predecessors) Djikstra(V start, double initialCost = 0)
        {
            PriorityQueue<V, double> spt = new PriorityQueue<V, double>();
            Dictionary<V, double> costs = new Dictionary<V, double>();
            Dictionary<V, V> predecessors = new Dictionary<V, V>();

            spt.Enqueue(start, initialCost);
            costs[start] = initialCost;
            predecessors[start] = start;

            while (spt.Count > 0)
            {
                V vertex = spt.Dequeue();

                foreach (var edge in AdjacencyList[vertex])
                {
                    double cost = costs[vertex] + edge.Cost;
                    if (!costs.ContainsKey(edge.Neighbor) || cost < costs[edge.Neighbor])
                    {
                        costs[edge.Neighbor] = cost;
                        predecessors[edge.Neighbor] = vertex;
                        spt.Enqueue(edge.Neighbor, cost);
                    }
                }
            }

            return (costs: costs, predecessors: predecessors);
        }

        #endregion
    }
}