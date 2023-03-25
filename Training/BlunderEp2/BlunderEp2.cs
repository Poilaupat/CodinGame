namespace Training.BlunderEp2
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
    class Solution
    {
        public static void Main(string[] args)
        {
            var graph = BuildGraph();

            //Displays graph
            // foreach (var node in graph.AdjacencyList)
            // {
            //     Console.Error.WriteLine($"{node.Key.Id} {node.Key.Weight} {node.Value.FirstOrDefault()?.Neighbor.Id} {node.Value.LastOrDefault()?.Neighbor.Id}");
            // }

            var start = graph.AdjacencyList.Keys.First();
            //var res = graph.DjikstraInverse(start, start.Money);
            var res = graph.DjikstraInverse2(start, start.Money);

            Console.WriteLine(res.costs.SingleOrDefault(x => x.Key.Id == -1).Value);
        }

        static Graph<Room> BuildGraph()
        {
            int roomCount = int.Parse(Console.ReadLine());

            List<string[]> roomData = new List<string[]>();
            for (int i = 0; i < roomCount; i++)
            {
                roomData.Add(Console.ReadLine().Split(' '));
            }

            List<Room> vertices = roomData.Select(x => new Room(Convert.ToInt32(x[0]), Convert.ToInt32(x[1]))).ToList();
            var exit = new Room(-1, 0);
            vertices.Add(exit);

            List<Edge<Room>> edges = new List<Edge<Room>>();

            edges.AddRange(roomData
                .Select(x => new
                {
                    Vertex = vertices.Single(y => y.Id == Convert.ToInt32(x[0])),
                    Neighbor = x[2] == "E" ? exit : vertices.Single(y => y.Id == Convert.ToInt32(x[2]))
                })
                .Select(x => new Edge<Room>(x.Vertex, x.Neighbor, x.Neighbor.Money))
            );

            edges.AddRange(roomData
                .Select(x => new
                {
                    Vertex = vertices.Single(y => y.Id == Convert.ToInt32(x[0])),
                    Neighbor = x[3] == "E" ? exit : vertices.Single(y => y.Id == Convert.ToInt32(x[3]))
                })
                .Select(x => new Edge<Room>(x.Vertex, x.Neighbor, x.Neighbor.Money))
            );

            return new Graph<Room>(vertices, edges);
        }
    }

    class Room : IVertex
    {
        public int Id { get; set; }
        public int Money { get; set; }

        public Room(int id, int money)
        {
            Id = id;
            Money = money;
        }

        public override bool Equals(object? obj)
        {
            return obj == this ||
                (obj is Room room && room.Id == this.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Id:{Id}({Money})";
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

        public Dictionary<V, V> BreadthFirstSearch(V start)
        {
            var visited = new HashSet<V>();
            Dictionary<V, V> predecessors = new Dictionary<V, V>();

            if (this.ContainsVertex(start))
            {
                var queue = new Queue<V>();
                queue.Enqueue(start);
                visited.Add(start);

                while (queue.Count > 0)
                {
                    var vertex = queue.Dequeue();

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

        public (Dictionary<V, double> costs, Dictionary<V, V> predecessors) DjikstraInverse(V start, double initialCost = 0)
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
                    if (!costs.ContainsKey(edge.Neighbor) || cost > costs[edge.Neighbor])
                    {
                        costs[edge.Neighbor] = cost;
                        predecessors[edge.Neighbor] = vertex;
                        spt.Enqueue(edge.Neighbor, cost);
                    }
                }
            }

            return (costs: costs, predecessors: predecessors);
        }

        public (Dictionary<V, int> costs, Dictionary<V, V> predecessors) DjikstraInverse2(V start, int initialCost = 0)
        {
            SpeedQueue<V> spt = new SpeedQueue<V>();
            Dictionary<V, int> costs = new Dictionary<V, int>();
            Dictionary<V, V> predecessors = new Dictionary<V, V>();

            spt.Enqueue(start, initialCost);
            costs[start] = initialCost;
            predecessors[start] = start;

            while (spt.Count > 0)
            {
                V vertex = spt.Dequeue();

                foreach (var edge in AdjacencyList[vertex])
                {
                    int cost = costs[vertex] + edge.Cost;
                    if (!costs.ContainsKey(edge.Neighbor))
                    {
                        costs[edge.Neighbor] = cost;
                        predecessors[edge.Neighbor] = vertex;
                        spt.Enqueue(edge.Neighbor, cost);
                    }
                    else if(cost > costs[edge.Neighbor])
                    {
                        spt.Requeue(edge.Neighbor, costs[edge.Neighbor], cost);
                        costs[edge.Neighbor] = cost;
                        predecessors[edge.Neighbor] = vertex;
                    }
                }
            }

            return (costs: costs, predecessors: predecessors);
        }

        #endregion
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

    interface IVertex
    {
        int Id { get; set; }
    }

    class SpeedQueue<V> where V : IVertex
    {
        public int Count { get; private set; } = 0;
        private int _minDistance = 0;
        private int _maxDistance = 0;
        private HashSet<V>[] vertices = new HashSet<V>[System.Int16.MaxValue];

        public void Enqueue(V vertex, int distance)
        {
            if(vertices[distance] is null)
                vertices[distance] = new HashSet<V>();

            vertices[distance].Add(vertex);

            if(distance > _maxDistance)
                _maxDistance = distance;

            Count++;
        }

        public V Dequeue()
        {
            if (Count > 0)
            {
                while (_minDistance <= _maxDistance)
                {
                    if (vertices[_minDistance] is not null && vertices[_minDistance].Count > 0)
                    {
                        var vertex = vertices[_minDistance].First();
                        vertices[_minDistance].Remove(vertex);
                        Count--;
                        return vertex;
                    }
                    _minDistance++;
                }
            }

            throw new IndexOutOfRangeException();
        }

        public void Requeue(V vertex, int oldDistance, int newDistance)
        {
            if(vertices[oldDistance] is not null)
            {
                vertices[oldDistance].Remove(vertex);
                Count--;
            }
            Enqueue(vertex, newDistance);
        }
    }
}