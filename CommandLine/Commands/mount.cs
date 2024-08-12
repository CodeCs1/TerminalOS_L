using System;
using TerminalOS_L.Driver;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.FileSystemR.Microsoft;

namespace TerminalOS_L {
    public class Mount : Command {
        public Mount(string name) : base(name) {}
        public override string Execute(string[] args)
        {
            //mount ext2 IDE_0 1
            //mount fat32 IDE_0 2 -l
            if (args.Length < 4) {
                Console.WriteLine("Usage: mount <FileSystem> IDE_<0->4> Partition_number(1->4)");
                return "Less argsuments";
            }
            string type=args[0];
            ATA ata=new(0x1f0,true);
            switch (args[1]) {
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
                    Console.WriteLine("NO such IDE Device: {0}", type);
                    break;
            }
            ata.Identify();
            _ = new GPT(ata);
            if (GPT.ISGpt()) {
                Message.Send_Error("GPT hasn't support yet!");
                return "GPT not support!";
            }
            MBR mbr = new(ata);
            mbr.List();
            if (Convert.ToInt32(args[2]) > 3) {
                Message.Send_Error("Only support 4 partitions!");
                return "Out of partition";
            }
            switch(type) {
                case "ext2":
                    var e = new FileSystemR.Linux.Ext2(ata, mbr.mbr_.Partitions[Convert.ToInt32(args[2])-1].LBAStart);
                    e.Impl();
                    break;
                case "fat32":
                    _ = new FileSystemR.Microsoft.FAT32.FAT32(ata,(int)mbr.mbr_.Partitions[Convert.ToInt64(args[2])-1].LBAStart);
                    break;
            }
            return base.Execute(args);
        }
    }
}