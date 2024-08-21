using System;
using System.Text;
using Cosmos.HAL;
using TerminalOS_L.FileSystemR.Microsoft;
using TerminalOS_L.System;
using Sys = Cosmos.System;

namespace TerminalOS_L
{
    public class Kernel: Sys.Kernel
    {
        public static FileIO f;
        public static CommandManager cm;
        public static string file;

        public static MBR.MBR_t mbr_;

        public static string DOW(int day) {
            switch (day)
            {
                case 0:
                    return "Sunday";
                case 1:
                    return "Monday";
                case 2:
                    return "Tuesday";
                case 3:
                    return "Wednesday";
                case 4:
                    return "Thursday";
                case 5:
                    return "Friday";
                case 6:
                    return "Saturday";
                default:
                    break;
            }
            return "Unknown";
        }

        protected override void OnBoot()
        {
            //Disable IDE Cosmos Driver and focus on creating own.
            Sys.Global.Init(GetTextScreen(),true,true,true,false);
        }

        public static void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2},", b);
            }
            sb.Append('}');
            Console.WriteLine(sb.ToString());
        }

        public static bool SVGASupport;
        private static string Username;
        private static string Password;

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("TerminalOS_L (TerminalOS reborn Linux version)");
            Console.WriteLine($"Boot date: {DOW(RTC.DayOfTheWeek)} - {RTC.DayOfTheMonth}/{RTC.Month}/{RTC.Century}{RTC.Year} {RTC.Hour:D2}:{RTC.Minute:D2}:{RTC.Second:D2}");
            cm = new CommandManager();
            PCIDevice dev = PCI.GetDevice(VendorID.VMWare,DeviceID.SVGAIIAdapter);
            if (dev.VendorID == (ushort)VendorID.VMWare && dev.DeviceID == (ushort)DeviceID.SVGAIIAdapter) {
                Message.Send("Detected VMWare SVGA-II Device, Replacing VGA method...");
                SVGASupport=true;
            }
            Console.WriteLine("\nThis is root from Terminal OS.");
            Console.Write("root login: ");
            Username = Console.ReadLine();
            string pass = string.Empty;
            ConsoleKey k;

            Console.Write("Password: ");
            do {
                var info = Console.ReadKey(true);
                k = info.Key;
                if (k == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(" ");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(info.KeyChar))
                {
                    Console.Write("*");
                    pass += info.KeyChar;
                }
            }while(k != ConsoleKey.Enter);
            Password=pass;
            Console.WriteLine();
        }

        public static StringBuilder v = new();
        ///<summary>
        /// The ReadLine function used to resemble the linux shell.
        ///</summary>
        private static string ReadLine() {
            Console.Write($"{Username}$ {v}");
            string res = null;
            ConsoleKeyInfo info = Console.ReadKey(true);
            while(info.Key != ConsoleKey.Enter) {
                if ((info.Modifiers & ConsoleModifiers.Control) != 0) {
                    switch(info.Key) {
                        case ConsoleKey.L:
                            Console.Clear();
                            Console.Write($"{Username}$ {v}");
                            break;
                        case ConsoleKey.C:
                            Console.Write("^C");
                            Console.WriteLine();
                            Console.Write($"{Username}$ ");
                            v.Clear();
                            break;
                        case ConsoleKey.B:
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            break;
                        case ConsoleKey.F:
                            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                            break;
                        case ConsoleKey.A:
                            Console.SetCursorPosition(0, Console.CursorTop);
                            break;
                        case ConsoleKey.E:
                            Console.SetCursorPosition(v.Length - 1, Console.CursorTop);
                            break;

                    }
                }
                else {
                    switch (info.Key) {
                        case ConsoleKey.Backspace:
                            if (v.Length > 0) {
                                if (v[^1] == '\t') { // check for tab key press
                                    for (int i=0;i<4;i++) {
                                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                        Console.Write(" ");
                                    }
                                    Console.SetCursorPosition(Console.CursorLeft - 4, Console.CursorTop);
                                    v = v.Remove(v.Length - 1, 1);
                                } else {
                                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                    Console.Write(" ");
                                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                    v = v.Remove(v.Length - 1, 1);
                                }
                            }
                            break;
                        case ConsoleKey.Tab:
                            Console.SetCursorPosition(Console.CursorLeft + 4, Console.CursorTop);
                            v.Append('\t');
                            break;
                        case ConsoleKey.Enter:
                            Console.CursorLeft = 0;
                            Console.CursorTop++;
                            break;
                        case ConsoleKey.Spacebar:
                            Console.Write(" ");
                            v.Append(' ');
                            break;
                        case ConsoleKey.LeftArrow:
                            if (Console.CursorLeft > 4+v.Length)
                                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            break;
                        case ConsoleKey.RightArrow:
                            if (Console.CursorLeft < v.Length+4)
                                Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                            break;
                        default:
                            Console.Write(info.KeyChar);
                            v.Append(info.KeyChar);
                            break;
                    }
                }
                info = Console.ReadKey(true);
            }
            if (info.Key == ConsoleKey.Enter) {
                Console.WriteLine();
                res = v.ToString();
            }
            return res;
        }
        
        protected override void Run()
        {
            var input = ReadLine();
            cm.Input(input);
            v=new();
        }

        protected override void AfterRun()
        {
            base.AfterRun();
        }
    }
}
