using System.Text;
using Cosmos.HAL;
using Cosmos.HAL.Drivers.Audio;
using TerminalOS_L.Driver.AHCI;
using TerminalOS_L.FrameBuffer;
using Sys = Cosmos.System;

namespace TerminalOS_L
{
    public class Kernel: Sys.Kernel
    {
        public static FileIO f;
        public static CommandManager cm;
        public static string file;
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
            FrConsole.WriteLine(sb.ToString());
        }

        public static bool SVGASupport;
        private static string Username;
        private static string Password;
        public static bool UseAC97;

        protected override void BeforeRun()
        {
            _ = new FrConsole();
            FrConsole.WriteLine("TerminalOS_L (TerminalOS reborn Linux version)");
            FrConsole.WriteLine($"Boot date: {DOW(RTC.DayOfTheWeek)} - {RTC.DayOfTheMonth}/{RTC.Month}/{RTC.Century}{RTC.Year} {RTC.Hour:D2}:{RTC.Minute:D2}:{RTC.Second:D2}");
            cm = new CommandManager();
            PCIDevice dev = PCI.GetDevice(VendorID.VMWare,DeviceID.SVGAIIAdapter);
            if (dev.VendorID == (ushort)VendorID.VMWare && dev.DeviceID == (ushort)DeviceID.SVGAIIAdapter) {
                Message.Send("Detected VMWare SVGA-II Device, Replacing VGA method...");
                SVGASupport=true;
            }
            dev = PCI.GetDeviceClass(ClassID.MultimediaDevice, SubclassID.IDEInterface); // SubClassID is actually Multimedia Audio Controller, not IDEInterface
            if (dev.DeviceExists) {
                Message.Send("Detected AC97 Driver.");
                AC97.Initialize(4096); // Load builtin driver.
            }
            if (AHCI.IsAHCI()) {
                Message.Send("Detected AHCI Driver.");
                _ = new AHCI();
            }
            FrConsole.WriteLine("\nThis is root from Terminal OS.");
            FrConsole.Write("root login: ");
            Username = FrConsole.ReadLine();
            FrConsole.WriteLine();
        }
        
        protected override void Run()
        {
            FrConsole.Write("$ ");
            var input = FrConsole.ReadLine();
            FrConsole.WriteLine(cm.Input(input));
        }

        protected override void AfterRun()
        {
            base.AfterRun();
        }
    }
}
