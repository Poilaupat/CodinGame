namespace Training.Unary
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
        static void Main(string[] args)
        {
            StringBuilder builder = new StringBuilder();

            int last = -1;
            foreach (char c in Console.ReadLine())
            {
                for (int i = 6; i >= 0; i--) // 7 bits ascii chars written in reverse
                {
                    int bit = (byte)c >> i & 1;
                    if (bit == last)
                        builder.Append("0");
                    else if (bit == 0)
                        builder.Append(" 00 0");
                    else
                        builder.Append(" 0 0");

                    last = bit;
                }
            }

            Console.WriteLine(builder.ToString().Trim());
        }
    }
}