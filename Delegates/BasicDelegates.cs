using System;

namespace BasicDelegates
{
    class Program
    {
        delegate void LogDel(string text);

        static void Main(string[] args)
        {
            LogDel logDel = new LogDel(LogTextToScreen);

            Console.WriteLine("Please enter your name");
            var name = Console.ReadLine();

            logDel(name);

            Console.ReadKey();
        }

        static void LogTextToScreen(string text)
        {
            Console.WriteLine($"{DateTime.Now}: {text}");
        }
    }
}