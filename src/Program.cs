// See https://aka.ms/new-console-template for more information
using System.Text;
using lox;

internal class Lox
{
    static bool hadError = false;

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        if (args.Length > 1)
        {
            Console.WriteLine("Usage jlox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }        
    }

    static void RunFile(string path)
        {
            var text = File.ReadAllText(path);
            Run(text);

            if(hadError)
            {
                Environment.Exit(65);
            }
        }

        static void RunPrompt()
        {
            for (; ; )
            {
                Console.WriteLine("> ");
                string? line = Console.ReadLine();

                if (line == null)
                {
                    break;
                }

                Run(line);
                hadError = false;
            }
        }

        static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message) => Report(line, "", message);

        static void Report(int line, string where, string mesage)
        {
            Console.WriteLine($"Line {line} Error {where}: message");
            hadError = true;
        }
}