namespace Training.War
{
    using Console = CodinGameEmulator.Console;
    using System.Diagnostics;

    public class Test
    {
        public static void Run()
        {
            var watch = new Stopwatch();
            var testnames = new string[] { "3Cards", "26Cards", "26CardsMean", "War", "1Game1War", "2WarInARow", "LongGame", "Pat", "PatAnother" };
            //var testnames = new string[] { "Example" };

            foreach (var testname in testnames)
            {                
                Console.SetInputDataFromFile($"Training/{nameof(War)}/TestCases/{testname}.Input.txt");
                watch.Start();
                Solution.Main(new string[0]);
                watch.Stop();
                Console.CheckOutputData($"{testname}", $"Training/{nameof(War)}/TestCases/{testname}.Output.txt", watch.ElapsedMilliseconds);
                watch.Reset();
            }
        }
    }
}