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
        int W = int.Parse(inputs[0]); // width of the building.
        int H = int.Parse(inputs[1]); // height of the building.
        int N = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        inputs = Console.ReadLine().Split(' ');
        int X0 = int.Parse(inputs[0]);
        int Y0 = int.Parse(inputs[1]);
        
        BatCoordinate x = new BatCoordinate(W, X0);
        BatCoordinate y = new BatCoordinate(H, Y0);

        // game loop
        while (true)
        {
            string bombDir = Console.ReadLine(); // the direction of the bombs from batman's current location (U, UR, R, DR, D, DL, L or UL)
            
            Console.Error.WriteLine($"H={H} W={W}");
            Console.Error.WriteLine($"X Position={x.Value} Y Position ={y.Value} ");
            Console.Error.WriteLine($"X LowerBound={x.LowerBound} X HigherBound={x.HigherBound} ");
            Console.Error.WriteLine($"Y LowerBound={y.LowerBound} y HigherBound={y.HigherBound} ");
            Console.Error.WriteLine($"Dir={bombDir}");
            
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            if(bombDir.Contains("R"))
                x.Increase();
            if(bombDir.Contains("L"))
                x.Decrease();
            if(bombDir.Contains("D"))
                y.Increase();
            if(bombDir.Contains("U"))
                y.Decrease();

            Console.Error.WriteLine($"Response X={x.Value} Y={y.Value}");

            // the location of the next window Batman should jump to.
            Console.WriteLine($"{x.Value} {y.Value}");
        }
    }
}

class BatCoordinate
{
    public int Value {get;set;}
    public int LowerBound {get;set;}
    public int HigherBound {get;set;}

    public BatCoordinate(int size, int value)
    {
        Value = value;
        LowerBound = 0;
        HigherBound = size - 1;
    }

    public void Decrease()
    {
        HigherBound = Value - 1;
        Value = Value - (int)((HigherBound - LowerBound) / 2f) - 1;
    }

    public void Increase()
    {
        LowerBound = Value + 1;
        Value = Value + (int)((HigherBound - LowerBound) / 2f) + 1;
    }
}
