using System;

namespace TerminalOS_L {
    public static class Message {
        public static void Send(string message) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" * ");
            Console.ResetColor();
            Console.WriteLine($"{message}");
        }
        public static void Send_Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" * ");
            Console.ResetColor();
            Console.WriteLine($"{message}");
        }
        public static void Send_Warning(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" * ");
            Console.ResetColor();
            Console.WriteLine($"{message}");
        }
        private static int count=0;
        public static void Send_Log(string message) {
            Console.WriteLine($"[{count:D7}] - {message}");
            count++;
        }
    }
}