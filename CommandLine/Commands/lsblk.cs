using System;
using System.Text;
using TerminalOS_L.Driver;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.FrameBuffer;

namespace TerminalOS_L {
    public class ListBlock : Command
    {
        public ListBlock(string name) : base(name)
        {
        }
        public override string Execute(string[] args)
        {
            FrConsole.WriteLine("Disk Name:");
            foreach(string name in ATA.DeviceName) {
                FrConsole.WriteLine($"/dev/{name}");
                if (GPT.TotalPartition != -1) {
                    for (int i=1;i<=GPT.TotalPartition;i++) {
                        FrConsole.WriteLine($" +- /dev/{name}{i}");
                    }
                } else {
                    for (int i=1;i<=MBR.TotalPartition;i++) {
                        FrConsole.WriteLine($" +- /dev/{name}{i}");
                    }
                }
            }
            return "";
        }
    }
}