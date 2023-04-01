namespace Training.HorseRace
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
            int N = int.Parse(Console.ReadLine());
            int maxDiff = 10000000;
            int smallestdiff = maxDiff;

            // Following code is functionnal, but fails last test for performance issues
            // int[] horses = new int[N];
            // for (int i = 0; i < N; i++)
            // {
            //     horses[i] = int.Parse(Console.ReadLine());

            //     for(int j = 0; j < i; j++)
            //     {
            //         var diff = Math.Abs(horses[i] - horses[j]);
            //         smallestdiff = Math.Min(diff, smallestdiff);
            //     }
            // }

            List<int> horses = new List<int>();
            for (int i = 0; i < N; i++)
            {
                horses.Add(int.Parse(Console.ReadLine()));
            }

            horses = horses.Order().ToList();

            for(int i = 0; i < N - 1; i++)
            {
                var diff = Math.Abs(horses[i] - horses[i + 1]);
                if(diff < smallestdiff)
                    smallestdiff = diff;
            }

            Console.WriteLine(smallestdiff);
        }
    }
}