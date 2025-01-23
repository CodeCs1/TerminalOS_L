using System;
using System.Collections.Generic;
using TerminalOS_L.Driver;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.FileSystemR.Linux;
using TerminalOS_L.FileSystemR.Microsoft.FAT32;
using TerminalOS_L.FrameBuffer;

namespace TerminalOS_L.Misc {

    public class Getroot : Command{
        public Getroot(string name) : base(name) { }
        public static ATA ata {get;set;}
        public static string Path {get;set;}
        private static uint initLBA=0;
        //public static Ext2 ext2 {get;set;}
        public static int RegisteredVFSIndex=0;
        public static List<VFS> RegisteredVFS = new()
        {
            new Ext2(ata),
            new FAT32(ata)
        };
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
            if (ata == null) {
                Message.Send_Error("ATA drive need to be mount first!");
                return "";
            }
            
            string dev = args[0][5..];
            if (dev.StartsWith("sd")) {
                string partition_number = dev[3..];

                if (!int.TryParse(partition_number, out int partition)) {
                    Message.Send_Error("Can't parse string2int");
                    return "str2int";
                }

                if (GPT.ISGpt()) {
                    initLBA = (uint)GPT.type[partition-1].StartingLBA;
                } else {
                    initLBA = MBR.mbr_.Partitions[partition-1].LBAStart;
                }
                RegisteredVFSIndex=0;
                int NonRegistered = 0;
                foreach(VFS vfs in RegisteredVFS) {
                    vfs.LBA_Start = initLBA;
                    int error=vfs.Impl();
                    RegisteredVFSIndex++;
                    if (error == -1) NonRegistered++;
                    else break;
                }
                if (NonRegistered == RegisteredVFSIndex) {
                    Message.Send_Error("No such file system. [Error: 0x01]");
                    return "Wrong File System or The file system isn't implemented";
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