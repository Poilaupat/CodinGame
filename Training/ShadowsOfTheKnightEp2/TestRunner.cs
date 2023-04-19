namespace Training.ShadowsOfTheKnightEp2
{
    using System.Diagnostics;
    using Console = CodinGameEmulator.Console;

    public class Test
    {
        public static void Run()
        {
            var watch = new Stopwatch();
            //var testnames = new string[] {  };
            var testnames = new string[] { "Debug" };

            foreach (var testname in testnames)
            {                
                Console.SetInputDataFromFile($"Training/{nameof(ShadowsOfTheKnightEp2)}/TestCases/{testname}.Input.txt");
                watch.Start();
                Player.Main(new string[0]);
                watch.Stop();
                Console.CheckOutputData($"{testname}", $"Training/{nameof(ShadowsOfTheKnightEp2)}/TestCases/{testname}.Output.txt", watch.ElapsedMilliseconds);
                watch.Reset();
            }
        }
    }
}