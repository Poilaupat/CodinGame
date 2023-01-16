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
            foreach (var node in graph.AdjacencyList)
            {
                Console.Error.WriteLine($"{node.Key.Id} {node.Key.Weight} {node.Value.First().Id} {node.Value.Last().Id}");
            }

            // Write an answer using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine("answer");
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

            List<Tuple<Room, Room>> edges = new List<Tuple<Room, Room>>();

            edges.AddRange(roomData
                .Select(x => new Tuple<Room, Room>
                (
                    vertices.Single(y => y.Id == Convert.ToInt32(x[0])),
                    x[2] == "E" ? exit : vertices.Single(y => y.Id == Convert.ToInt32(x[2]))
                ))
            );

            edges.AddRange(roomData
                .Select(x => new Tuple<Room, Room>
                (
                    vertices.Single(y => y.Id == Convert.ToInt32(x[0])),
                    x[3] == "E" ? exit : vertices.Single(y => y.Id == Convert.ToInt32(x[3]))
                ))
            );

            return new Graph<Room>(vertices, edges);
        }
    }

    class Room
    {
        public int Id { get; set; }
        public int Weight { get; set; }

        public Room(int id, int weight)
        {
            Id = id;
            Weight = weight;
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
            return $"Id:{Id}({Weight})";
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

        public void RemoveVertex(T vertex)
        {
            if (AdjacencyList.Keys.Contains(vertex))
                AdjacencyList.Remove(vertex);

            foreach (var edgeVertices in AdjacencyList.Values)
                foreach (var edgeVertex in edgeVertices)
                {
                    if (edgeVertex.Equals(vertex))
                    {
                        edgeVertices.Remove(edgeVertex);
                        break;
                    }
                }
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

        #region Graph Traverval Algorithms

        public HashSet<T> BreadthFirst(T start) 
        {
            var visited = new HashSet<T>();

            if (!this.AdjacencyList.ContainsKey(start))
                return visited;

            var queue = new Queue<T>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                if (visited.Contains(vertex))
                    continue;

                visited.Add(vertex);

                foreach (var neighbor in this.AdjacencyList[vertex])
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return visited;
        }

        public Func<T, IEnumerable<T>> ShortestPathFunction(T start)
        {
            var previous = new Dictionary<T, T>();

            var queue = new Queue<T>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();
                foreach (var neighbor in this.AdjacencyList[vertex])
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

    #endregion
}

