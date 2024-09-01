using System;
using System.Drawing;
using TerminalOS_L.FrameBuffer;

namespace TerminalOS_L {
    public static class Message {
        public static void Send(string message) {
            FrConsole.ForegroundColor = Color.Green;
            FrConsole.Write(" * ");
            FrConsole.ResetColor();
            FrConsole.WriteLine($"{message}");
        }
        public static void Send_Error(string message) {
            FrConsole.ForegroundColor = Color.Red;
            FrConsole.Write(" * ");
            FrConsole.ResetColor();
            FrConsole.WriteLine($"{message}");
        }
        public static void Send_Warning(string message) {
            FrConsole.ForegroundColor = Color.Yellow;
            FrConsole.Write(" * ");
            FrConsole.ResetColor();
            FrConsole.WriteLine($"{message}");
        }
        private static int count=0;
        public static void Send_Log(string message) {
            FrConsole.WriteLine($"[{count:D7}] - {message}");
            count++;
        }
    }
}