using System.Text;

namespace CodinGameEmulator
{
    class Console
    {
        public static Queue<string> _StandardInputData = new Queue<string>();
        public static Queue<string> _StandardOutputData = new Queue<string>();
        public static StringBuilder _Buffer = new StringBuilder();

        public static Error Error { get; } = new Error();

        public static void SetInputData(string testData)
        {
            _StandardInputData = new Queue<string>(testData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart()));
        }
        public static void SetInputDataFromFile(string path)
        {
            _StandardInputData = new Queue<string>(File.ReadAllLines(path));
        }
        public static void CheckOutputData(string testname, string path)
        {
            FlushBuffer();
            
            foreach (var fileLine in File.ReadAllLines(path))
            {
                var outputLine = _StandardOutputData.Dequeue();
                if (outputLine is null || !outputLine.Equals(fileLine))
                {
                    WriteSystemConsole($"Test <{testname}> failed. Expected <{fileLine}> but was <{outputLine ?? "null"}>", System.ConsoleColor.Red);
                    return;
                }
            }
            WriteSystemConsole($"Test <{testname}> ok", System.ConsoleColor.Green);
        }
        public static string ReadLine()
        {
            return _StandardInputData.Dequeue();
        }
        public static void WriteLine(object? value)
        {
            FlushBuffer();

            if (value is not null)
            {
                string? sValue = value.ToString();
                if (sValue is not null)
                    _StandardOutputData.Enqueue(sValue);
            }
        }

        public static void Write(object? value)
        {
            _Buffer.Append(value);
        }

        private static void FlushBuffer()
        {
            var str = _Buffer.ToString();
            if(!string.IsNullOrWhiteSpace(str))
                _StandardOutputData.Enqueue(str);
            _Buffer.Clear();
        }

        private static void WriteSystemConsole(string message, System.ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
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