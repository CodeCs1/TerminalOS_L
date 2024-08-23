using System;
using TerminalOS_L.Driver;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.Misc;

namespace TerminalOS_L {
    public class Mount : Command {
        public static ATA ata;
        public Mount(string name) : base(name) {}
        public override string Execute(string[] args)
        {
            //mount IDE_0
            if (args.Length < 1) {
                Console.WriteLine("Usage: mount IDE_<0->4> ");
                return "Less argsuments";
            }
            switch (args[0]) {
                case "IDE_0":
                    ata = new(0x1f0, true);
                    break;
                case "IDE_1":
                    ata = new(0x1f0, false);
                    break;
                case "IDE_2":
                    ata = new(0x170, true);
                    break;
                case "IDE_3":
                    ata = new(0x170, false);
                    break;
                default:
                    Console.WriteLine("NO such IDE Device: {0}", args[0]);
                    return "No such IDE Device";
            }
            ata.Identify();
            _ = new GPT(ata);
            if (!GPT.ISGpt()) // MBR Partition only support 4 Partitions
            {
                _ = new MBR(ata);
            }
            Getroot.ata = ata;
            return base.Execute(args);
        }
    }
}