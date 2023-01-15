namespace Training.ThereIsNoSpoonEp1
{
    using Console = CodinGameEmulator.Console;

    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /**
     * Don't let the machines win. You are humanity's last hope...
     **/
    class Player
    {
        static void Main(string[] args)
        {
            int width = int.Parse(Console.ReadLine()); // the number of cells on the X axis
            int height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis
            Console.Error.WriteLine($"Width={width} / Height={height}");

            //Loads data into an array
            char[,] grid = new char[height, width];
            for (int y = 0; y < height; y++)
            {
                string line = Console.ReadLine(); // width characters, each either 0 or .
                for (int x = 0; x < width; x++)
                {
                    grid[y, x] = line[x];
                }
            }

            // Three coordinates: a node, its right neighbor, its bottom neighbor
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int xrn = -1, yrn = -1, xbn = -1, ybn = -1;

                    if (grid[y, x] == '0') //Node at current position
                    {
                        //Finding first neighbour to the right
                        for (int x2 = x + 1; x2 < width; x2++)
                        {
                            if (grid[y, x2] == '0')
                            {
                                xrn = x2;
                                yrn = y;
                                break;
                            }
                        }

                        //Finding first neighbour below
                        for (int y2 = y + 1; y2 < height; y2++)
                        {
                            if (grid[y2, x] == '0')
                            {
                                xbn = x;
                                ybn = y2;
                                break;
                            }
                        }

                        Console.WriteLine($"{x} {y} {xrn} {yrn} {xbn} {ybn}");
                    }
                }
            }
        }
    }
}