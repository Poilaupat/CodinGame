namespace Training.ShadowsOfTheKnightEp2
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
            int width = int.Parse(inputs[0]); // width of the building.
            int height = int.Parse(inputs[1]); // height of the building.
            int n = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
            inputs = Console.ReadLine().Split(' ');
            int x0 = int.Parse(inputs[0]);
            int y0 = int.Parse(inputs[1]);

            var edges = new List<Edge>
            {
                new Edge(new Point(3,4), new Point(2,12)),
                new Edge(new Point(8,18), new Point(15,19)),
                new Edge(new Point(19,11), new Point(16,1)),
                new Edge(new Point(15,19), new Point(19,11)),
                new Edge(new Point(2,12), new Point(8,18)),
                new Edge(new Point(16,1), new Point(6,3)),
                new Edge(new Point(6,3), new Point(3,4))
            };

            var p = new Polygon(edges);
            var e = new Edge(new Point(18, 17), new Point(10, 0));

            var ps = p.Split(e);

        }
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && (this == obj
                || (obj is Point other && this.X == other.X && this.Y == other.Y));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public double DistanceTo(Point other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2d) + Math.Pow(Y - other.Y, 2d));
        }
    }

    class Edge
    {
        public Point A { get; set; }
        public Point B { get; set; }

        public Edge(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && (this == obj
                || (obj is Edge other && this.A.Equals(other.A) && this.B.Equals(other.B)));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B);
        }

        public Point? Intersection(Edge other)
        {
            float d = ((B.X - A.X) * (other.B.Y - other.A.Y)) - ((B.Y - A.Y) * (other.B.X - other.A.X));

            //  Segment are parallel 
            if (d == 0)
                return null;

            float n = ((A.Y - other.A.Y) * (other.B.X - other.A.X)) - ((A.X - other.A.X) * (other.B.Y - other.A.Y));
            float r = n / d;

            float n2 = ((A.Y - other.A.Y) * (B.X - A.X)) - ((A.X - other.A.X) * (B.Y - A.Y));
            float s = n2 / d;

            // Segment do not cut
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return null;

            // Find intersection point
            return new Point()
            {
                X = (int)Math.Round(A.X + (r * (B.X - A.X))),
                Y = (int)Math.Round(A.Y + (r * (B.Y - A.Y)))
            };
        }
    }

    class Polygon : IEnumerable<Edge>
    {
        private IList<Edge> _edges { get; set; }

        public IEnumerable<Point> Vertices => _edges.Select(e => e.A);

        public Polygon(IList<Edge> edges)
        {
            if (edges is null ||
                edges.Count < 3 ||
                !edges.All(e1 => edges.Count(e2 => e1.A.Equals(e2.B)) == 1 && edges.Count(e2 => e1.B.Equals(e2.A)) == 1))
                throw new ArgumentException("Not a polygon");

            _edges = edges;
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            return _edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_edges).GetEnumerator();
        }

        public Edge Next(Edge edge)
        {
            return _edges.Single(e => e.A.Equals(edge.B));
        }

        public Edge Previous(Edge edge)
        {
            return _edges.Single(e => e.B.Equals(edge.A));
        }

        public IEnumerable<Polygon> Split(Edge segment)
        {
            Dictionary<Edge, Point> intersections = new Dictionary<Edge, Point>();
            foreach (var edge in _edges)
            {
                var intersection = edge.Intersection(segment);
                if (intersection is not null)
                {
                    intersections.Add(edge, intersection);
                }
            }

            if (intersections.Count == 2) // the segment splits the polygon in half
            {
                List<Edge>[] edges = new List<Edge>[2] { new List<Edge>(), new List<Edge>() };

                var edge = _edges.First();
                var startPoint = edge.A;
                do
                {
                    if (intersections.ContainsKey(edge))
                    {
                        var edge2 = intersections.Single(i => !i.Key.Equals(edge)).Key;
                        Edge cut1 = new Edge(edge.A, intersections[edge]);
                        Edge cut2 = new Edge(intersections[edge], intersections[edge2]);
                        Edge cut3 = new Edge(intersections[edge2], edge2.B);

                        edges[0].AddRange(new Edge[] { cut1, cut2, cut3 });
                        edge = Next(edge2);
                    }
                    else
                    {
                        edges[0].Add(edge);
                        edge = Next(edge);
                    }
                } while (!edge.A.Equals(startPoint));

                edge = intersections.First().Key;
                startPoint = edge.A;
                do
                {
                    if (intersections.ContainsKey(edge))
                    {
                        var edge2 = intersections.Single(i => !i.Key.Equals(edge)).Key;
                        Edge cut1 = new Edge(edge.B, intersections[edge]);
                        Edge cut2 = new Edge(intersections[edge], intersections[edge2]);
                        Edge cut3 = new Edge(intersections[edge2], edge2.A);

                        edges[1].AddRange(new Edge[] { cut1, cut2, cut3 });
                        edge = Previous(edge2);
                    }
                    else
                    {
                        edges[1].Add(new Edge(edge.B, edge.A));
                        edge = Previous(edge);
                    }
                } while (!edge.A.Equals(startPoint));

                return new Polygon[] { new Polygon(edges[0]), new Polygon(edges[1]) };
            }
            else
            {
                return new Polygon[] { this };
            }
        }

    }
}