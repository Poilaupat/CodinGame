namespace Training.HorseRace
{
    using System.Diagnostics;
    using Console = CodinGameEmulator.Console;

    public class Test
    {
        public static void Run()
        {
            var watch = new Stopwatch();
            var testnames = new string[] { "SimpleCase", "DisorderedHorses", "ManyHorses" };

            foreach (var testname in testnames)
            {
                Console.SetInputDataFromFile($"Training/HorseRace/TestCases/{testname}.Input.txt");
                watch.Start();
                Solution.Main(new string[0]);
                watch.Stop();
                Console.CheckOutputData($"{testname}", $"Training/HorseRace/TestCases/{testname}.Output.txt", watch.ElapsedMilliseconds);
                watch.Reset();
            }
        }
    }
}