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
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int w = int.Parse(inputs[0]);
        int h = int.Parse(inputs[1]);
        inputs = Console.ReadLine().Split(' ');
        int startRow = int.Parse(inputs[0]);
        int startCol = int.Parse(inputs[1]);
        int n = int.Parse(Console.ReadLine());

        PathFinder.MapHeight = h;
        PathFinder.MapWidth = w;

        var pfs = new List<PathFinder>();
        for (int i = 0; i < n; i++)
        {
            pfs.Add(new PathFinder(startCol, startRow));            
            pfs.Last().DisplayMap();
            pfs.Last().SearchTreasure();
        }

        var candidates = pfs.Where(x => x.TreasureFound);
        if(candidates.Count() == 0)
        {
            Console.WriteLine("TRAP");
        }
        else
        {
            var min = candidates.Min(x => x.PathLength);
            Console.WriteLine(pfs.FindIndex(x => x.PathLength == min));
        }
    }

    internal class PathFinder
    {
        private int _x, _y;
        private char[,] _map;

        public static int MapWidth {get;set;} = 0;
        public static int MapHeight {get;set;} = 0;
        public int PathLength {get; private set;} = 1;
        public bool TreasureFound {get; private set;} = false;

        public bool CoordinatesAreValid
        {
            get
            {
                return _x >= 0
                    && _x < MapWidth
                    && _y >= 0
                    && _y < MapHeight;
            }
        }

        public PathFinder(int x, int y)
        {
            _x = x;
            _y = y;
            LoadMap();
        }

        private void LoadMap()
        {
            _map = new char[MapWidth, MapHeight];

            for (int y = 0; y < MapHeight; y++)
            {
                string mapRow = Console.ReadLine();
                for(int x = 0; x < MapWidth; x++)
                {
                    _map[x,y] = mapRow[x];
                }
            }
        }

        public void DisplayMap()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    Console.Error.Write(_map[x,y]);
                }
                Console.Error.WriteLine();
            }
        }

        public void SearchTreasure()
        {
            var validchars = "<^>vT";

            Console.Error.WriteLine($"Starting point= ({_x},{_y})");

            List<Tuple<int,int>> path = new List<Tuple<int, int>>(); //For circular path detection

            while(!TreasureFound 
                && CoordinatesAreValid 
                && validchars.Contains(_map[_x,_y])
                && !path.Any(x => x.Item1 == _x && x.Item2 == _y))
            {
                path.Add(Tuple.Create<int, int>(_x, _y));

                if(_map[_x,_y] == '<')
                    _x--;
                else if(_map[_x,_y] == '>')
                    _x++;
                else if(_map[_x,_y] == '^')
                    _y--;
                else if(_map[_x,_y] == 'v')
                    _y++;
                else if(_map[_x,_y] == 'T')
                    TreasureFound = true;

                PathLength++;
            }

            Console.Error.WriteLine($"TreasureFound = {TreasureFound} / PathLength = {PathLength}");
        }
    }
}
