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
        string[] inputs = Console.ReadLine().Split(' ');
        int height = int.Parse(inputs[0]);
        int width = int.Parse(inputs[1]);

        char[,] map = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            var row = Console.ReadLine().ToCharArray();
            for(int x = 0; x < width; x++)
            {
                map[x,y] = row[x];
            }
        }

        var bender = new Bender();
        //bender.DisplayMap(map);
        var moves = bender.TraceRoute(map);
        //bender.DisplayMap(map);

        if(moves.Any())
        {
            foreach(var move in moves)
            {
                Console.WriteLine(move.ToString().ToUpper());
            }
        }
        else
        {
            Console.WriteLine("LOOP");
        }
    }
}

enum EDirection
{
    South,
    East,
    North,
    West,
}

class Bender
{
    public Point Position {get;set;}
    public EDirection Direction {get;set;} = EDirection.South;
    public bool IsBerserk {get;set;}
    public bool IsMagnetized {get;set;}
    public List<Visit> Visited {get;} = new List<Visit>();

    public List<EDirection> TraceRoute(char[,] map)
    {
        List<EDirection> movements = new List<EDirection>();
        Position = GetInitialPosition(map);

        while(map[Position.X, Position.Y] != '$')
        {
            Visited.Add(new Visit(Position, Direction));
            MarkMap(map);
            DisplayMap(map);

            //Loop detection
            if(IsInLoop(map))
            {
                movements.Clear();
                break;
            }
            
            foreach(var direction in GetDirectionOrder())
            {
                var nextDirection = direction;
                var nextPosition = GetNextPosition(map, Position, direction);

                if(nextPosition is null)
                    continue;

                var canMove = true;
                switch(map[nextPosition.X, nextPosition.Y])
                {
                    case 'S':
                        nextDirection = EDirection.South;
                        break;
                    case 'W':
                        nextDirection = EDirection.West;
                        break;
                    case 'N':
                        nextDirection = EDirection.North;
                        break;
                    case 'E':
                        nextDirection = EDirection.East;
                        break;
                    case 'I':
                        IsMagnetized = !IsMagnetized;
                        break;
                    case 'B':
                        IsBerserk = !IsBerserk;
                        break;
                    case '#':
                        canMove = false;
                        break;
                    case 'X':
                        if(!IsBerserk)
                        {
                            canMove = false;
                            Visited.Last().SetBreakable(nextDirection);
                        }
                        else
                            map[nextPosition.X, nextPosition.Y] = ' ';
                        break;
                    case 'T':
                        nextPosition = FindTwinTeleporter(map, nextPosition);
                        break;
                    default:
                        break;                    
                }

                if(canMove)
                {
                    movements.Add(direction);
                    Direction = nextDirection;
                    Position = nextPosition;
                    break;
                }
            }
        }
        return movements;
    }

    private Point GetInitialPosition(char[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                if(map[x,y].Equals('@'))
                    return new Point(x, y);
            }
        }

        return null;
    }

    private List<EDirection> GetDirectionOrder()
    {
        List<EDirection> directions = new List<EDirection>()
        {
            EDirection.South, 
            EDirection.East, 
            EDirection.North, 
            EDirection.West
        };

        if(IsMagnetized)
            directions.Reverse();

        directions.Remove(Direction);
        directions.Insert(0, Direction);

        return directions;
    }

    private Point GetNextPosition(char[,] map, Point position, EDirection direction)
    {
        Point nextPosition = null;
        
        switch(direction)
        {
            case EDirection.South:
                if(position.Y < map.GetLength(1)-1)
                    nextPosition = new Point(position.X, position.Y+1);
                break;
            case EDirection.West:
                  if(position.X > 0)
                    nextPosition = new Point(position.X-1, position.Y);
                break;          
            case EDirection.North:
                if(position.Y > 0)
                    nextPosition = new Point(position.X, position.Y-1);
                break;
            case EDirection.East:
                if(position.X < map.GetLength(0)-1)
                    nextPosition = new Point(position.X+1, position.Y);
                break; 
        }

        return nextPosition;
    }

    private Point FindTwinTeleporter(char[,] map, Point firstTeleporter)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                Point p = new Point(x, y);
                if(map[x,y].Equals('T') && !p.Equals(firstTeleporter))
                    return p;
            }
        }
        return firstTeleporter; // My twin is nowhere to be found ;(
    }

    private bool IsInLoop(char[,] map)
    {     
        var last = Visited.Last();
        var previous = Visited
            .Take(Visited.Count() - 1)
            .LastOrDefault(x => x.Position.Equals(last.Position) && x.Direction == last.Direction);
                
        if(previous is not null) // Bender know this place... but did anything changed since last visit ?
        {
            var loop = Visited.TakeLast(Visited.Count() - Visited.IndexOf(previous)).ToList();
            foreach(var breakable in loop.Where(x => x.FaceBreakableWall))
            {
                var nextPosition = GetNextPosition(map, breakable.Position, breakable.Direction);
                if(map[nextPosition.X, nextPosition.Y] == ' ' || map[nextPosition.X, nextPosition.Y] == '.') //A wall was broken since the last visit
                {
                    return false;
                }
                else
                {
                    var beerCount = loop.TakeWhile(x => x != breakable).Count(x => map[x.Position.X, x.Position.Y] == 'B');
                    if(!last.Position.Equals(breakable.Position) && map[breakable.Position.X, breakable.Position.Y] == 'B')
                        beerCount++;
                    if((IsBerserk && beerCount % 2 == 0) || (!IsBerserk && beerCount % 2 == 1)) //Wall is still here, but correct bier amount on the way to break it
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MarkMap(char[,] map)
    {
        char[] overridable = new char[]{'<','>','^','v','.',' '};
        var lasts = Visited.TakeLast(2);

        char mark = ' ';
        switch(lasts.Last().Direction)
        {
            case EDirection.South:
                mark = 'v';
                break;
            case EDirection.West:
                mark = '<';
                break;
            case EDirection.North:
                mark = '^';
                break;
            case EDirection.East:
                mark = '>';
                break;
        }
        if(overridable.Contains(map[lasts.Last().Position.X, lasts.Last().Position.Y]))
        {
            map[lasts.Last().Position.X, lasts.Last().Position.Y] = mark;
        }

        if(lasts.Count() == 2 && overridable.Contains(map[lasts.First().Position.X, lasts.First().Position.Y]))
        {
            map[lasts.First().Position.X, lasts.First().Position.Y] = '.';
        }
    }

    public void DisplayMap(char[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                Console.Error.Write(map[x,y]);
            }
            Console.Error.WriteLine();
        }
    }
}

class Point
{
    public int X {get;}
    public int Y {get;}

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        return obj == this ||
            (obj is Point point && point.X == this.X && point.Y == this.Y);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.X, this.Y);
    }

    public override string ToString()
    {
        return $"{{{X},{Y}}}";
    }
}

class Visit
{
    public Point Position {get;}
    public EDirection Direction {get;}
    public bool FaceBreakableWall {get; private set;}

    public Visit(Point position, EDirection direction)
    {
        Position = position;
        Direction = direction;
    }

    public void SetBreakable(EDirection direction)
    {
        if(direction == this.Direction)
        {
            FaceBreakableWall = true;
        }
    }
}

