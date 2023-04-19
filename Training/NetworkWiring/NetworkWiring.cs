namespace Training.NetworkWiring
{
    using Console = CodinGameEmulator.Console;


    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    class Solution
    {
        public static void Main(string[] args)
        {

            int n = int.Parse(Console.ReadLine());
            var houses = new HouseCollection(n);

            for (int i = 0; i < n; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);

                houses[i] = new House { X = x, Y = y };
            }

            var game = new Game(houses);
            game.Run();

            Console.WriteLine(game.WireLength);
        }
    }

    internal class House
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal class HouseCollection : IEnumerable<House>
    {
        private House[] _houses;

        public HouseCollection(int size)
        {
            _houses = new House[size];
        }

        public House this[int index]
        {
            get => _houses[index];
            set => _houses[index] = value;
        }

        public IEnumerator<House> GetEnumerator()
        {
            return _houses
                .AsEnumerable()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _houses
                .GetEnumerator();
        }

        public int XMin { get => _houses.Min(h => h.X); }
        public int XMax { get => _houses.Max(h => h.X); }
        public int YMin { get => _houses.Min(h => h.Y); }
        public int YMax { get => _houses.Max(h => h.Y); }
    }

    internal class Game
    {
        public long WireLength {get;set;}

        private HouseCollection _houses;

        public Game(HouseCollection houses)
        {
            _houses = houses;
        }

        public void Run()
        {
            var yBackBone = _houses
                .OrderBy(h => h.Y)
                .ElementAt((int)Math.Ceiling(_houses.Count() / 2d) - 1)
                .Y;
        
            WireLength = _houses.XMax - _houses.XMin; //BackBone length

            foreach(var house in _houses)
            {
                WireLength += Math.Abs(house.Y - yBackBone);
            }
        }
    }
}