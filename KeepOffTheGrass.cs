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
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);

        Board board = new Board(width, height);

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int myMatter = int.Parse(inputs[0]);
            int oppMatter = int.Parse(inputs[1]);

            board.Reset(myMatter, oppMatter);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int scrapAmount = int.Parse(inputs[0]);
                    int owner = int.Parse(inputs[1]); // 1 = me, 0 = foe, -1 = neutral
                    int units = int.Parse(inputs[2]);
                    int recycler = int.Parse(inputs[3]);
                    int canBuild = int.Parse(inputs[4]);
                    int canSpawn = int.Parse(inputs[5]);
                    int inRangeOfRecycler = int.Parse(inputs[6]);

                    Tile tile = new Tile(j, i, scrapAmount, owner, units, recycler, canBuild, canSpawn, inRangeOfRecycler);
                    board.AddTile(tile);
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine("WAIT");
        }
    }
}

#region Enums

enum EOwner
{
    NoOne = -1,
    Opponent = 0,
    Me = 1,
}

enum EActionType
{
    Wait,
    Move,
    Build,
    Spawn,
    Message,
}

#endregion

class Tile 
{
    public int X { get; set; }
    public int Y  { get; set; }
    public int ScrapAmount { get; set; }
    public int  Units { get; set; }
    public EOwner Owner { get; set; }
    public bool IsRecycler{ get; set; }
    public bool CanBuild{ get; set; }
    public bool CanSpawn { get; set; }
    public bool InRangeOfRecycler{ get; set; }

    public bool MarkedForBuild { get; set; } = false;
    public bool MarkedForSpawn { get; set; } = false;

    public Tile(int x, int y, int scrapAmount, int owner, int units, int recycler, int canBuild, int canSpawn, int inRangeOfRecycler) 
    {
        this.X = x;
        this.Y = y;
        this.ScrapAmount = scrapAmount;
        this.Owner = (EOwner)owner;
        this.Units = units;
        this.IsRecycler = recycler == 1 ? true : false;
        this.CanBuild = canBuild == 1 ? true : false;;
        this.CanSpawn = canSpawn == 1 ? true : false;;
        this.InRangeOfRecycler = inRangeOfRecycler == 1 ? true : false;;
    }

    public int ApproximateDistance(Tile tile)
    {
        return Math.Abs(this.X - tile.X) + Math.Abs(this.Y - tile.Y);
    }
}

class BalanceOfPower
{
    public Tile MyTile { get; private set; }
    public Tile OppTile { get; private set; }

    public int Distance { get { return MyTile.ApproximateDistance(OppTile); } }
    public int UnitBalance { get { return MyTile.Units - OppTile.Units; } }

    public BalanceOfPower(Tile myTile, Tile oppTile)
    {
        MyTile= myTile;
        OppTile = oppTile;
    }    
}

class Board
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public int MyMatter { get; private set; }
    public int OpponentMatter { get; private set; }

    public List<Tile> Tiles { get; } = new List<Tile>();

    public Board(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int RecyclerCount
    {
        get { return Tiles.Count(t => t.Owner == EOwner.Me && t.IsRecycler); }
    }

    public IEnumerable<Tile> MyUnits
    {
       get { return Tiles.Where(t => t.Owner == EOwner.Me && t.Units > 0); } 
    }

    public int MyUnitCount
    {
        get { return MyUnits.Sum(t => t.Units); }
    }

    public IEnumerable<Tile> OppUnits
    {
       get { return Tiles.Where(t => t.Owner == EOwner.Opponent && t.Units > 0); } 
    }

    public int OppUnitCount
    {
        get { return OppUnits.Sum(t => t.Units); }
    }

    public IEnumerable<BalanceOfPower> MyUnitsAgainstOppUnits
    {
        get { return MyUnits.SelectMany(u => OppUnits, (x, y) => new BalanceOfPower(x, y)); }
    }

    public void Reset(int myMatter, int oppMatter)
    {
        Tiles.Clear();
        MyMatter = myMatter;
        OpponentMatter = oppMatter;
    }

    public void AddTile(Tile tile)
    {
        if(!Tiles.Any(t => t.X == tile.X && t.Y == tile.Y))
            Tiles.Add(tile);
    }

    public ICollection<Tile> GetSurroundingTiles(Tile tile)
    {
        var around = new List<Tile>();

        for(int i = Math.Max(0, tile.X - 1); i <= Math.Min(Width - 1, tile.X + 1); i++)
        for(int j = Math.Max(0, tile.Y - 1); j <= Math.Min(Height - 1, tile.Y + 1); j++)
        {
            if(tile.X != i && tile.Y != j)
            {
                var other = Tiles.Single(t => t.X == i && t.Y == j);
                around.Add(other);
            }
        }

        return around;
    }

    public float MeanDistanceFromOpponents(Tile tile)
    {
        return OppUnits.Sum(u => u.ApproximateDistance(tile)) / OppUnitCount;
    }
}

#region Action Management


class Computer
{
    private static readonly float RATIO_UNIT_RECYCLER = 10f;
    private static readonly int RECYCLER_MALUS_OPP_PROXIMITY = 1;
    private static readonly int RECYCLER_MALUS_RECYCLER_PROXIMITY = 2;

    Board _board;

    public Computer(Board board)
    {
        _board = board;
    }

    public ActionCollection ComputeActions()
    {
        ActionCollection actions = new ActionCollection();

        //Création d'un recycleur si besoin
        var recyclerLocation = FindBestRecyclerLocation();
        if(recyclerLocation is not null)
        {
            recyclerLocation.MarkedForBuild = true;
            BuildAction action = new BuildAction(recyclerLocation.X, recyclerLocation.Y);
            actions.Add(action);
        }

        //Spawning sur les points à risque
        var hotpoint = FindCheapestHotpoint();
        while(hotpoint is not null && _board.MyMatter >= 10)
        {        
            hotpoint.MyTile.MarkedForSpawn = true;
            SpawnAction action = new SpawnAction(
                Math.Abs(hotpoint.UnitBalance) + 1, 
                hotpoint.MyTile.X, 
                hotpoint.MyTile.Y);
            actions.Add(action);
            hotpoint = FindCheapestHotpoint();
        }

        //Spawning pour harvest plus rapidement
        var harvestPoint = FindBestHarvestingLocation();
        if(harvestPoint is not null && _board.MyMatter >= 10)
        {
            harvestPoint.MarkedForSpawn = true;
            SpawnAction action = new SpawnAction(
                Math.Abs(hotpoint.UnitBalance) + 1, 
                harvestPoint.X, 
                harvestPoint.Y);
            actions.Add(action);
        }

        foreach(var unit in _board.MyUnits.Where(u => !u.MarkedForSpawn))
        {
            var moveActions = GetMoves(unit);
            actions.AddRange(moveActions);
        }

        return actions;
    }

    public BalanceOfPower FindCheapestHotpoint()
    {
        var candidates = _board
                .MyUnitsAgainstOppUnits
                .Where(x => 
                    x.Distance == 1 //unités amies juste à coté d'unité ennemies
                    && x.UnitBalance <= 0 //où le rapport de force m'est défavorable ou juste égal
                    && Math.Abs(x.UnitBalance) * 10 < _board.MyMatter //et où la quantité de minerai dispo peut changer qqchose
                );

        return candidates
            .OrderBy(x => x.UnitBalance) //Moindre coût d'abord
            .FirstOrDefault();
    }

    public Tile FindBestRecyclerLocation()
    {
        Tile bestTileSoFar = null;
        int bestScoreSoFar = 0;

        if(_board.RecyclerCount == 0 
            || _board.MyUnitCount / _board.RecyclerCount > RATIO_UNIT_RECYCLER)
        {
            foreach(var tile in _board.Tiles.Where(t => t.CanBuild))
            {
                int score = tile.ScrapAmount;
                foreach(var n1Tile in _board.GetSurroundingTiles(tile))
                {
                    score += n1Tile.ScrapAmount;

                    if(n1Tile.Owner == EOwner.Opponent) score -= RECYCLER_MALUS_OPP_PROXIMITY;
                    if(n1Tile.InRangeOfRecycler) score -= RECYCLER_MALUS_RECYCLER_PROXIMITY;
                }

                if(score > bestScoreSoFar)
                {
                    bestScoreSoFar = score;
                    bestTileSoFar = tile;
                }
            }
        }

        return bestTileSoFar;
    }

    public Tile FindBestHarvestingLocation()
    {
        Tile bestTileSoFar = null;
        int bestScoreSoFar = 0;

        foreach(var tile in  _board.MyUnits.Where(t => t.CanSpawn && t.Units == 0))
        {
            int score = tile.ScrapAmount;
            foreach(var n1Tile in _board.GetSurroundingTiles(tile))
            {
                score += n1Tile.ScrapAmount;
                if(n1Tile.Owner == EOwner.Opponent)
                {
                    if(n1Tile.Units > 0) //Tile à coté d'unité ennemies
                        score -= 10;
                    else
                        score += 10; //Tile ennemie non défendue;
                }
            }
            if(score > bestScoreSoFar)
            {
                bestScoreSoFar = score;
                bestTileSoFar = tile;
            }
        }

        return bestTileSoFar;
    }

    public IEnumerable<MoveAction> GetMoves(Tile unitTile)
    {
        List<MoveAction> actions = new List<MoveAction>();

        if(!unitTile.MarkedForSpawn && unitTile.Units > 0)
        {
            var surroundings = _board.GetSurroundingTiles(unitTile);
            var winsOrExchanges = surroundings.Where(s => s.Owner == EOwner.Opponent && s.Units <= unitTile.Units);

            
        }

        return actions;
    }
}



class ActionCollection
{
    private List<Action> _actions = new List<Action>();

    public void Add(Action action)
    {
        _actions.Add(action);
    }

    public void AddRange(IEnumerable<Action> actions)
    {
        _actions.AddRange(actions);
    }

    public string GetCommands()
    {
        StringBuilder commands = new StringBuilder();

        if(_actions.Count == 0)
            _actions.Add(new WaitAction());

        foreach(var action in _actions.OrderByDescending(a => a.Priority))
        {
            commands.Append($"{action.GetCommand()};");
        }
        return commands.ToString();
    }
}


abstract class Action
{
    public EActionType Type { get; protected set; }
    public int Priority { get; protected set; } = 1;

    public abstract string  GetCommand();
}

class WaitAction : Action
{
    public WaitAction()
    {
        Type = EActionType.Wait;
    }

    public override string GetCommand()
    {
        return $"WAIT";
    }    
}

class MessageAction : Action
{
    private string _message;

    public MessageAction(string message)
    {
        Type = EActionType.Message;
        _message = message;
    }

    public override string GetCommand()
    {
        return $"MESSAGE {_message}";
    }
}

class MoveAction : Action
{
    public int _amount, _fromX, _fromY, _toX, _toY;

    public MoveAction(int amount, int fromX, int fromY, int toX, int toY)
    {
        _amount = amount;
        _fromX = fromX;
        _fromY = fromY;
        _toX = toX;
        _toY = toY;
    }

    public override string GetCommand()
    {
        return $"MOVE {_amount} {_fromX} {_fromY} {_toX} {_toY}";
    }
}

class BuildAction : Action 
{
    public int _x, _y;

    public BuildAction(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public override string GetCommand()
    {
        return $"BUILD {_x} {_y}";
    }
}

class SpawnAction : Action 
{
    public int _amount, _x, _y;

    public SpawnAction(int amount, int x, int y)
    {
        _amount = amount;
        _x = x;
        _y = y;
    }

    public override string GetCommand()
    {
        return $"SPAWN {_amount} {_x} {_y}";
    }
}

#endregion
