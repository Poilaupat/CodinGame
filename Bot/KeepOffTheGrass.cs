namespace KeepOffTheGrass
{
    using Console = CodinGameEmulator.Console;

    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /*
     *
     * This code reached Silver league on Bot Battle Challenge fall 2022.
     * For it has not much room for improvements, I decided to explore totally different approche
     */


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

                Computer hal = new Computer(board);
                hal.ComputeActions();
                Console.WriteLine(hal.GetCommands());
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
        public int Y { get; set; }
        public int ScrapAmount { get; set; }
        public int Units { get; set; }
        public EOwner Owner { get; set; }
        public bool IsRecycler { get; set; }
        public bool CanBuild { get; set; }
        public bool CanSpawn { get; set; }
        public bool InRangeOfRecycler { get; set; }

        public bool IsGrass { get { return ScrapAmount == 0; } }

        public bool MarkedForBuild { get; set; } = false;
        public bool MarkedForSpawn { get; set; } = false;
        public int UnitsToLeave { get; set; } = 0;
        public int UnitsToCome { get; set; } = 0;

        public override bool Equals(object obj)
        {
            return obj == this ||
                (obj is Tile tile && tile.X == this.X && tile.Y == this.Y);
        }

        public override int GetHashCode() //Not mine. This is Jon Skeet's solution for an appropriate coordinates GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public Tile(int x, int y, int scrapAmount, int owner, int units, int recycler, int canBuild, int canSpawn, int inRangeOfRecycler)
        {
            this.X = x;
            this.Y = y;
            this.ScrapAmount = scrapAmount;
            this.Owner = (EOwner)owner;
            this.Units = units;
            this.IsRecycler = recycler == 1 ? true : false;
            this.CanBuild = canBuild == 1 ? true : false; ;
            this.CanSpawn = canSpawn == 1 ? true : false; ;
            this.InRangeOfRecycler = inRangeOfRecycler == 1 ? true : false; ;
        }

        public int ApproximateDistance(Tile tile)
        {
            return Math.Abs(this.X - tile.X) + Math.Abs(this.Y - tile.Y);
        }

        public override string ToString()
        {
            return $"{{{X},{Y}}}";
        }
    }

    class TileCollection : HashSet<Tile>
    {
        private static Random _rg;

        public TileCollection() : base()
        {

        }
        public TileCollection(Tile tile) : this()
        {
            AddTile(tile);
        }
        public TileCollection(IEnumerable<Tile> tiles) : this()
        {
            AddTiles(tiles);
        }

        public void AddTile(Tile tile)
        {
            if (tile is not null)
            {
                this.Add(tile);
            }
        }

        public void AddTiles(IEnumerable<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                Add(tile);
            }
        }

        public Tile GetTile(int x, int y)
        {
            return this.FirstOrDefault(t => t.X == x && t.Y == y);
        }

        public Tile GetRandomTile()
        {
            if (_rg is null)
            {
                _rg = new Random(DateTime.Now.Millisecond);
            }

            int n = _rg.Next(this.Count() - 1);
            return this.ElementAt(n);
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var y in this.GroupBy(t => t.Y).OrderBy(g => g.Key))
            {
                str.AppendJoin(" ", y.OrderBy(t => t.X));
                str.Append("\n");
            }
            return str.ToString();
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
            MyTile = myTile;
            OppTile = oppTile;
        }
    }

    class Board
    {
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int MyMatter { get; private set; }
        public int OpponentMatter { get; private set; }

        public TileCollection Tiles { get; } = new TileCollection();

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int RecyclerCount
        {
            get { return Tiles.Count(t => t.Owner == EOwner.Me && t.IsRecycler); }
        }

        public IEnumerable<Tile> MyTilesWithUnits
        {
            get { return Tiles.Where(t => t.Owner == EOwner.Me && t.Units > 0); }
        }

        public int MyUnitCount
        {
            get { return MyTilesWithUnits.Sum(t => t.Units); }
        }

        public IEnumerable<Tile> OppTilesWithUnits
        {
            get { return Tiles.Where(t => t.Owner == EOwner.Opponent && t.Units > 0); }
        }

        public int OppUnitCount
        {
            get { return OppTilesWithUnits.Sum(t => t.Units); }
        }

        public IEnumerable<BalanceOfPower> MyUnitsAgainstOppUnits
        {
            get { return MyTilesWithUnits.SelectMany(u => OppTilesWithUnits, (x, y) => new BalanceOfPower(x, y)); }
        }

        public void Reset(int myMatter, int oppMatter)
        {
            Tiles.Clear();
            MyMatter = myMatter;
            OpponentMatter = oppMatter;
        }

        public void AddTile(Tile tile)
        {
            if (tile is not null)
                Tiles.Add(tile);
        }

        public TileCollection GetSurroundingTiles(Tile tile, int radius = 1)
        {
            var around = new TileCollection();

            int xMin = Math.Max(0, tile.X - radius);
            int xMax = Math.Min(Width - 1, tile.X + radius);
            int yMin = Math.Max(0, tile.Y - radius);
            int yMax = Math.Min(Height - 1, tile.Y + radius);

            for (int x = xMin; x <= xMax; x++)
            {
                var upperTile = Tiles.GetTile(x, yMin);
                if (!upperTile.Equals(tile)) around.Add(upperTile);

                var lowerTile = Tiles.GetTile(x, yMax);
                if (!lowerTile.Equals(tile)) around.Add(lowerTile);
            }

            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                var leftTile = Tiles.GetTile(xMin, y);
                if (!leftTile.Equals(tile)) around.Add(leftTile);

                var rightTile = Tiles.GetTile(xMax, y);
                if (!rightTile.Equals(tile)) around.Add(rightTile);
            }

            // Console.Error.WriteLine($"Tile : {tile.ToString()} Radius : {radius}");
            // Console.Error.WriteLine(around.ToString());
            return around;
        }

        public TileCollection GetFreeTiles()
        {
            return null;
        }

        public float MeanDistanceFromOpponents(Tile tile)
        {
            return OppTilesWithUnits.Sum(u => u.ApproximateDistance(tile)) / OppUnitCount;
        }
    }

    #region Action Management


    class Computer
    {
        private static readonly int RECYCLER_MATTER_THRESHOLD = 100;
        private static readonly int RECYCLER_MALUS_OPP_PROXIMITY = 1;
        private static readonly int RECYCLER_MALUS_RECYCLER_PROXIMITY = 2;

        Board _board;
        ActionCollection _actions = new ActionCollection();

        public Computer(Board board)
        {
            _board = board;
        }

        public string GetCommands()
        {
            return _actions.GetCommands();
        }

        public void ComputeActions()
        {
            //Création d'un recycleur si besoin
            if (_board.RecyclerCount == 0 && _board.MyMatter < RECYCLER_MATTER_THRESHOLD)
            {
                var recyclerLocation = FindBestRecyclerLocation();
                if (recyclerLocation is not null)
                {
                    BuildAction action = new BuildAction(recyclerLocation);
                    _actions.Add(action);
                }
            }

            //Spawning sur les points à risque
            var hotpoint = FindCheapestHotpoint();
            while (hotpoint is not null && _board.MyMatter >= 10)
            {
                SpawnAction action = new SpawnAction(
                    Math.Abs(hotpoint.UnitBalance) + 1,
                    hotpoint.MyTile);
                _actions.Add(action);
                hotpoint = FindCheapestHotpoint();
            }

            //Spawning pour harvest plus rapidement 1x par tour
            var harvestPoint = FindBestHarvestingLocation();
            if (harvestPoint is not null && _board.MyMatter >= 10)
            {
                _actions.Add(new MessageAction($"Found harvesting place: {{ {harvestPoint.X}, {harvestPoint.Y} }}"));
                SpawnAction action = new SpawnAction(
                    1,
                    harvestPoint);
                _actions.Add(action);
            }

            foreach (var unit in _board.MyTilesWithUnits.Where(u => !u.MarkedForSpawn))
            {
                GetMoves(unit);
            }
        }

        public BalanceOfPower FindCheapestHotpoint()
        {
            var candidates = _board
                    .MyUnitsAgainstOppUnits
                    .Where(x =>
                          !x.MyTile.MarkedForSpawn
                        && x.Distance == 1 //unités amies juste à coté d'unité ennemies
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

            foreach (var tile in _board.Tiles.Where(t => t.CanBuild))
            {
                int score = tile.ScrapAmount;
                foreach (var n1Tile in _board.GetSurroundingTiles(tile))
                {
                    score += n1Tile.ScrapAmount;

                    if (n1Tile.Owner == EOwner.Opponent) score -= RECYCLER_MALUS_OPP_PROXIMITY;
                    if (n1Tile.InRangeOfRecycler) score -= RECYCLER_MALUS_RECYCLER_PROXIMITY;
                }

                if (score > bestScoreSoFar)
                {
                    bestScoreSoFar = score;
                    bestTileSoFar = tile;
                }
            }

            return bestTileSoFar;
        }

        public Tile FindBestHarvestingLocation()
        {
            Tile bestTileSoFar = null;
            int bestScoreSoFar = 0;

            foreach (var tile in _board.Tiles.Where(t =>
                    t.Owner == EOwner.Me
                && t.Units == 0 //Free place
                && t.CanSpawn))
            {
                int score = tile.ScrapAmount;
                foreach (var n1Tile in _board.GetSurroundingTiles(tile))
                {
                    score += n1Tile.ScrapAmount;
                    if (n1Tile.Owner == EOwner.Opponent)
                    {
                        if (n1Tile.Units > 0) //Tile à coté d'unité ennemies
                            score -= 10;
                        else
                            score += 10; //Tile ennemie non défendue;
                    }
                }
                if (score > bestScoreSoFar)
                {
                    bestScoreSoFar = score;
                    bestTileSoFar = tile;
                }
            }
            Console.Error.WriteLine($"Harvest spawn location : {bestTileSoFar?.X}, {bestTileSoFar?.Y}");
            return bestTileSoFar;
        }

        public void GetMoves(Tile myTile)
        {
            Console.Error.WriteLine($"Planning moves for tile {myTile.ToString()}");
            if (!myTile.MarkedForSpawn && myTile.Units > 0)
            {
                var surroundings = _board.GetSurroundingTiles(myTile);

                //Mouvements dont issue est une probable bataille gagnée
                var winTiles = surroundings.Where(s =>
                        s.Owner == EOwner.Opponent
                    && s.Units > 0
                    && s.Units < myTile.Units);
                if (winTiles.Any())
                {
                    var winTile = winTiles.Where(u => u.Units == winTiles.Max(u => u.Units)).First();
                    _actions.Add(new MoveAction(winTile.Units + 1, myTile, winTile));
                    Console.Error.WriteLine($"Moving for the win {winTile.ToString()}");
                    return;
                }

                //Mouvements dont issue est un probable échange d'unités
                var tradeoffTiles = surroundings.Where(s =>
                        s.Owner == EOwner.Opponent
                    && s.Units == myTile.Units);
                if (tradeoffTiles.Any())
                {
                    var tradeoffTile = tradeoffTiles.First();
                    _actions.Add(new MoveAction(tradeoffTile.Units, myTile, tradeoffTile));
                    Console.Error.WriteLine($"Moving for tradeoff {tradeoffTiles.ToString()}");
                    return;
                }

                //Danger immédiat ?
                var dangerTiles = surroundings.Where(s =>
                           (s.Owner == EOwner.Opponent && s.Units > myTile.Units) //Tile next to a too powerful opponent squad
                        || (s.ScrapAmount == 1) //Tile will be grass on next turn
                        );
                if (dangerTiles.Any())
                {
                    var safeTiles = surroundings
                        .Except(dangerTiles)
                        .Where(t => !t.IsRecycler);
                    if (safeTiles.Any())
                    {
                        var safeTile = safeTiles.First();
                        _actions.Add(new MoveAction(myTile.Units, myTile, safeTile));
                        Console.Error.WriteLine($"Escaping danger to {safeTile.ToString()}");
                        return;
                    }
                    else //on serre les fesses en espérant que l'ennemi ne nous voie pas :)
                    {
                        Console.Error.WriteLine($"Nowhere to escape. Praying the good lord.");
                        return;
                    }
                }
                else //Pas de danger immédiat
                {
                    Console.Error.WriteLine($"No battling move available. Just hanging around.");
                    bool leave = false;
                    int radius = 1;
                    while (myTile.UnitsToLeave < myTile.Units && !leave && radius < Math.Max(_board.Height, _board.Width))
                    {
                        var potentialMoves = _board
                            .GetSurroundingTiles(myTile, radius)
                            .Where(t => !t.IsRecycler && !t.IsGrass);

                        Console.Error.WriteLine($"Potential moves with radius = {radius} : {potentialMoves.Count()}");

                        switch (potentialMoves.Count())
                        {
                            case 0:
                                Console.Error.WriteLine("There's no way out of here.");
                                leave = true;
                                break;
                            case 1:
                                Console.Error.WriteLine("Only one way");
                                leave = true;
                                _actions.Add(new MoveAction(1, myTile, potentialMoves.Single()));
                                break;
                            default:
                                var goodMoves = potentialMoves.Where(t => t.Owner != EOwner.Me);
                                if (!goodMoves.Any())
                                {
                                    Console.Error.WriteLine("No good move. Searching further");
                                    radius++;
                                }
                                else
                                {
                                    Console.Error.WriteLine("Some good moves");
                                    foreach (var target in goodMoves)
                                    {
                                        _actions.Add(new MoveAction(1, myTile, target));
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            return;
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

            if (_actions.Count == 0)
                _actions.Add(new WaitAction());

            foreach (var action in _actions.OrderByDescending(a => a.Priority))
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

        public abstract string GetCommand();
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
        private Tile _fromTile;
        private Tile _toTile;
        public int _amount;

        public MoveAction(int amount, Tile fromTile, Tile toTile)
        {
            _amount = amount;
            _fromTile = fromTile;
            _toTile = toTile;
            _fromTile.UnitsToLeave += _amount;
            _toTile.UnitsToCome += _amount;
        }

        public override string GetCommand()
        {
            return $"MOVE {_amount} {_fromTile.X} {_fromTile.Y} {_toTile.X} {_toTile.Y}";
        }
    }

    class BuildAction : Action
    {
        public int _x, _y;
        private Tile _tile;

        public BuildAction(Tile tile)
        {
            _tile = tile;
            _tile.MarkedForBuild = true;
        }

        public override string GetCommand()
        {
            return $"BUILD {_tile.X} {_tile.Y}";
        }
    }

    class SpawnAction : Action
    {
        private int _amount;
        private Tile _tile;

        public SpawnAction(int amount, Tile tile)
        {
            _amount = amount;
            _tile = tile;
            _tile.MarkedForSpawn = true;
        }

        public override string GetCommand()
        {
            return $"SPAWN {_amount} {_tile.X} {_tile.Y}";
        }
    }

    #endregion
}