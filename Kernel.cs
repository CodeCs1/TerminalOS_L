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
            Console.ReadLine();
            string pass = string.Empty;
            ConsoleKey k;

            Console.Write("Password: ");
            do {
                var info = Console.ReadKey(intercept: true);
                k = info.Key;
                    if (k == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        Console.Write("\b \b");
                        pass = pass[0..^1];
                    }
                    else if (!char.IsControl(info.KeyChar))
                    {
                        Console.Write("*");
                        pass += info.KeyChar;
                    }
            }while(k != ConsoleKey.Enter);
        }
        
        protected override void Run()
        {
            Console.Write($"$ ");
            var input = Console.ReadLine();
            cm.Input(input);
            
        }

        protected override void AfterRun()
        {
            base.AfterRun();
        }
    }
}
