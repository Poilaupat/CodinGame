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
        int width = int.Parse(Console.ReadLine());
        int height = int.Parse(Console.ReadLine());
        string text = Console.ReadLine().ToUpper();
        List<char> alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();
        
        for (int i = 0; i < height; i++)
        {
            string row = Console.ReadLine();
            StringBuilder output = new StringBuilder();
            foreach(char c in text)
            {
                int index = alphabet.Contains(c) ? alphabet.FindIndex(x => x == c) : 26;
                //Console.Error.WriteLine($"Letter : {c} / Index {index}");
                string slice = string.Concat(row.Skip(index * width).Take(width));
                //Console.Error.WriteLine(slice);
                output.Append(slice);
            }
            Console.WriteLine(output.ToString());
        }
    }
}
