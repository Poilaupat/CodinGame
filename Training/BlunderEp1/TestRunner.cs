namespace Training.BlunderEp1
{
    using Console = CodinGameEmulator.Console;

    public class Test
    {
        public static void Run()
        {
            Console.SetInputDataFromFile("Training/BlunderEp1/MultipleLoops.Input.txt");
            Solution.Main(new string[0]);
            Console.CheckOutputData("MultipleLoops", "Training/BlunderEp1/MultipleLoops.Output.txt");
        }
    }
}