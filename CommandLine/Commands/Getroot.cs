using System;
using TerminalOS_L.Driver;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.FileSystemR.Linux;
using TerminalOS_L.FileSystemR.Microsoft.FAT32;

namespace TerminalOS_L.Misc {

    public class Getroot : Command{
        public Getroot(string name) : base(name) { }
        public static ATA ata {get;set;}
        public static string Path {get;set;}
        public static Ext2 ext2 {get;set;}
        public override string Execute(string[] args) {
            // getroot /dev/sda<partition number>
            if (args.Length < 1) {
                Message.Send_Error("Usage: getroot /dev/sda<partition number>");
                return "Less argsuments";
            }
            if (!args[0].StartsWith("/dev/")) {
                Message.Send_Error("Bad device format!");
                return "Bad device format";
            }
            string dev = args[0][5..];
            if (dev.StartsWith("sd")) {
                string partition_number = dev[3..];

                if (!int.TryParse(partition_number, out int partition)) {
                    Message.Send_Error("Can't parse string2int");
                    return "str2int";
                }

                if (GPT.ISGpt()) {
                    int LBA_Start = (int)GPT.type[partition-1].StartingLBA;
                    ext2 = new Ext2(ata, (uint)LBA_Start);
                    if (ext2.Impl() == -1) {
                        _ = new FAT32(ata, LBA_Start);
                        return "";
                    }
                } else {
                    int LBA_Start = (int)MBR.mbr_.Partitions[partition-1].LBAStart;
                    ext2 = new Ext2(ata, (uint)LBA_Start);
                    if (ext2.Impl() == -1) {
                        _ = new FAT32(ata, LBA_Start);
                        return "";
                    }
                }
                Path = "/";

            } else if (dev.StartsWith("sr")) { // cdRom boi
                //TODO
            } else {
                Message.Send_Error("Bad device format!");
                return "Bad device format";
            }
            return "";
        }
    }
}