namespace Training.BlunderEp2
{
    using Console = CodinGameEmulator.Console;
    using System.Diagnostics;

    public class Test
    {
        public static void Run()
        {
            var watch = new Stopwatch();
            var testnames = new string[] { "OneRoom", "3Rooms", "15RoomsLowDifference", "55Rooms", "1275Rooms", "5050Rooms", "9870Rooms", "SquareBuilding", "ManyEntries"};

            foreach (var testname in testnames)
            {                
                Console.SetInputDataFromFile($"Training/BlunderEp2/TestCases/{testname}.Input.txt");
                watch.Start();
                Solution.Main(new string[0]);
                watch.Stop();
                Console.CheckOutputData($"{testname}", $"Training/BlunderEp2/TestCases/{testname}.Output.txt", watch.ElapsedMilliseconds);
                watch.Reset();
            }
        }
    }
}