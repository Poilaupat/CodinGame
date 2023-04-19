namespace Training.NetworkWiring
{
    using Console = CodinGameEmulator.Console;
    using System.Diagnostics;

    public class Test
    {
        public static void Run()
        {
            var watch = new Stopwatch();
            var testnames = new string[] { "Example1","Example2","Example3","Example4","Example5","Example6","Example7","Example8","Example9" };
            //var testnames = new string[] { "Example3"};

            foreach (var testname in testnames)
            {                
                Console.SetInputDataFromFile($"Training/{nameof(NetworkWiring)}/TestCases/{testname}.Input.txt");
                watch.Start();
                Solution.Main(new string[0]);
                watch.Stop();
                Console.CheckOutputData($"{testname}", $"Training/{nameof(NetworkWiring)}/TestCases/{testname}.Output.txt", watch.ElapsedMilliseconds);
                watch.Reset();
            }
        }
    }
}