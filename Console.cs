using System.Text;

namespace CodinGameEmulator
{
    class Console
    {
        public static Queue<string> StandardInputData = new Queue<string>();
        public static Queue<string> StandardOutputData = new Queue<string>();

        public static StringBuilder _Buffer = new StringBuilder();

        public static Error Error { get; } = new Error();

        public static void SetInputData(string testData)
        {
            StandardInputData = new Queue<string>(testData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart()));
        }
        public static void SetInputDataFromFile(string path)
        {
            StandardInputData = new Queue<string>(File.ReadAllLines(path));
        }
        public static string ReadLine()
        {
            return StandardInputData.Dequeue();
        }
        public static void WriteLine(object? value)
        {
            StandardOutputData.Enqueue(_Buffer.ToString());

            if (value is not null)
            {
                string? sValue = value.ToString();
                if(sValue is not null)
                    StandardOutputData.Enqueue(sValue);
            }

            _Buffer.Clear();
        }

        public static void Write(object? value)
        {
            _Buffer.Append(value);
        }
    }

    class Error
    {
        public void Write(object? value = null)
        {
            System.Console.Write(value);
        }

        public void WriteLine(object? value = null)
        {
            System.Console.WriteLine(value);
        }
    }
}