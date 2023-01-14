namespace DeathFirstSearchEp1
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
        static void Main(string[] args)
        {
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            int verticeCount = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
            int edgeCount = int.Parse(inputs[1]); // the number of links
            int gatewayCount = int.Parse(inputs[2]); // the number of exit gateways

            HashSet<Vertice> vertices = new HashSet<Vertice>();
            HashSet<Tuple<Vertice, Vertice>> edges = new HashSet<Tuple<Vertice, Vertice>>();
            for (int i = 0; i < edgeCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int id1 = int.Parse(inputs[0]);
                int id2 = int.Parse(inputs[1]);

                Vertice? v1 = vertices.SingleOrDefault(x => x.Id == id1);
                if (v1 is null)
                {
                    v1 = new Vertice(id1);
                    vertices.Add(v1);
                }

                Vertice? v2 = vertices.SingleOrDefault(x => x.Id == id2);
                if (v2 is null)
                {
                    v2 = new Vertice(id2);
                    vertices.Add(v2);
                }

                edges.Add(new Tuple<Vertice, Vertice>(v1, v2));
            }

            for (int i = 0; i < gatewayCount; i++)
            {
                int idg = int.Parse(Console.ReadLine()); // the index of a gateway node
                vertices.Single(v => v.Id == idg).IsGateway = true;
            }

            Graph<Vertice> graph = new Graph<Vertice>(vertices, edges);


            // game loop
            while (true)
            {
                // The node on which the Bobnet agent is positioned this turn
                int agentId = int.Parse(Console.ReadLine());
                Vertice agent = graph
                    .AdjacencyList
                    .Keys
                    .Single(v => v.Id == agentId);

                var gateways = graph
                    .AdjacencyList
                    .Keys
                    .Where(v => v.IsGateway);


                //Finding the edge to cut...
                Vertice? verticeToCut = null;

                // Searching all accessibles positions from the agent current position
                var accessibles = GraphTraversalAlgorithms.BreadthFirst(graph, agent);
                Console.Error.WriteLine($"Accessible nodes: {accessibles.Count()} Accessibles Gateways: {accessibles.Count(x => x.IsGateway)}");

                // 1.Computing shortest path to all accessibles gateways

                //Setting a function that calculates the shortest path from agent to given vertice
                var shortestPathFunc = GraphTraversalAlgorithms.ShortestPathFunction(graph, agent);

                //Computing paths
                HashSet<Path> paths = new HashSet<Path>();
                foreach (var gateway in gateways)
                {
                    if (accessibles.Contains(gateway))
                    {
                        var path = shortestPathFunc(gateway);
                        paths.Add(new Path { Origin = agent, Target = gateway, Steps = path });
                    }
                }

                //Selecting shortest path
                int minPath = paths.Min(x => x.Length);
                var shortestPath = paths.First(x => x.Length == minPath);
                Console.Error.WriteLine($"Nearest gateway: {shortestPath.Target.Id}, Path Length: {shortestPath.Length}");

                // //2. Two strategies A or B
                // if(shortestPath.Length == 2) // A : On next move, bot will reach a gateway => we cut that edge
                // {
                //Path alway starts by agent itself, so skip 1, then cut the edge with the first node on the way
                verticeToCut = shortestPath.Steps.Skip(1).First();
                // }
                // else // B : No urgency, so we try to find the edge that will reduce the most the bot's movement capacity
                // {
                //     var neighbors = graph
                //         .AdjacencyList
                //         .Where(kvp => kvp.Value.Select(v => v.Id).Contains(agent.Id))
                //         .Select(kvp => kvp.Key);

                //     int scoreMin = int.MaxValue;                
                //     foreach(var neighbor in neighbors)
                //     {
                //         if(accessibles.Contains(neighbor))
                //         {
                //             var gClone = graph.Clone();
                //             gClone.RemoveEdge(agent, neighbor);
                //             var score = GraphTraversalAlgorithms.BreadthFirst(gClone, agent).Count();

                //             if(score < scoreMin)
                //             {
                //                 scoreMin = score;
                //                 verticeToCut = neighbor;
                //             }
                //         }
                //     }
                // }

                // 3. Cutting selected edge    
                graph.RemoveEdge(agent, verticeToCut);
                Console.WriteLine($"{agent.Id} {verticeToCut.Id}");
            }
        }
    }

    class Graph<T> where T : class
    {
        public Graph() { }
        public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges)
        {
            foreach (var vertex in vertices)
                AddVertex(vertex);

            foreach (var edge in edges)
                AddEdge(edge.Item1, edge.Item2);
        }

        public Dictionary<T, HashSet<T>> AdjacencyList { get; } = new Dictionary<T, HashSet<T>>();

        public void AddVertex(T vertex)
        {
            if (!AdjacencyList.Keys.Contains(vertex))
                AdjacencyList[vertex] = new HashSet<T>();
        }

        public void AddEdge(T v1, T v2)
        {
            if (AdjacencyList.ContainsKey(v1) && AdjacencyList.ContainsKey(v2))
            {
                if (!AdjacencyList[v1].Contains(v2))
                    AdjacencyList[v1].Add(v2);
                if (!AdjacencyList[v2].Contains(v1))
                    AdjacencyList[v2].Add(v1);
            }
        }

        public void RemoveEdge(T v1, T v2)
        {
            if (AdjacencyList.ContainsKey(v1) && AdjacencyList.ContainsKey(v2))
            {
                if (AdjacencyList[v1].Contains(v2))
                    AdjacencyList[v1].Remove(v2);

                if (AdjacencyList[v2].Contains(v1))
                    AdjacencyList[v2].Remove(v1);
            }
        }

        public Graph<T> Clone()
        {
            return new Graph<T>(
                this.AdjacencyList.Keys,
                this.AdjacencyList.SelectMany(x => x.Value.Select(y => new Tuple<T, T>(x.Key, y)))
            );
        }
    }

    public class Vertice
    {
        public int Id { get; }
        public bool IsGateway { get; set; }

        public Vertice(int id)
        {
            Id = id;
            IsGateway = false;
        }

        public override string ToString()
        {
            return $"{Id}{(IsGateway ? "G" : "")}";
        }

        public override bool Equals(object? obj)
        {
            return obj == this || (obj is Vertice vertice && vertice.Id == this.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    class GraphTraversalAlgorithms
    {
        public static HashSet<T> BreadthFirst<T>(Graph<T> graph, T start) where T : class
        {
            var visited = new HashSet<T>();

            if (!graph.AdjacencyList.ContainsKey(start))
                return visited;

            var queue = new Queue<T>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                if (visited.Contains(vertex))
                    continue;

                visited.Add(vertex);

                foreach (var neighbor in graph.AdjacencyList[vertex])
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return visited;
        }

        public static Func<T, IEnumerable<T>> ShortestPathFunction<T>(Graph<T> graph, T start) where T : class
        {
            var previous = new Dictionary<T, T>();

            var queue = new Queue<T>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();
                foreach (var neighbor in graph.AdjacencyList[vertex])
                {
                    if (previous.ContainsKey(neighbor))
                        continue;

                    previous[neighbor] = vertex;
                    queue.Enqueue(neighbor);
                }
            }

            Func<T, IEnumerable<T>> shortestPath = v =>
            {
                var path = new List<T> { };

                var current = v;
                while (!current.Equals(start))
                {
                    path.Add(current);
                    current = previous[current];
                };

                path.Add(start);
                path.Reverse();

                return path;
            };

            return shortestPath;
        }
    }

    public class Path
    {
        public Vertice? Origin { get; set; }
        public Vertice? Target { get; set; }
        public IEnumerable<Vertice>? Steps { get; set; }
        public int Length { get { return Steps?.Count() ?? 0; } }
    }
}