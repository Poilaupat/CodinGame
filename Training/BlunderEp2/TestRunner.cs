namespace Training.BlunderEp2
{
    using Console = CodinGameEmulator.Console;

    public class Test
    {
        public static void Run()
        {
            Console.SetInputDataFromFile("Training/BlunderEp2/FiftyFiveRooms.Input.txt");
            Solution.Main(new string[0]);
            Console.CheckOutputData("FifteenRoomsLowDifference", "Training/BlunderEp2/FiftyFiveRooms.Output.txt");
        }
    }
}