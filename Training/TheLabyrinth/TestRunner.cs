namespace Training.TheLabyrinth
{
    using Console = CodinGameEmulator.Console;

    public class Test
    {
        public static void Run()
        {
            var testnames = new string[] { "Test01" };

            foreach (var testname in testnames)
            {                
                Console.SetInputDataFromFile($"Training/TheLabyrinth/TestCases/{testname}.Input.txt");
                Player.Main(new string[0]);                
            }
        }
    }
}