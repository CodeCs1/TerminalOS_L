using System;
using System.Text;
using TerminalOS_L.Driver;
using TerminalOS_L.FileSystemR;

namespace TerminalOS_L {
    public class ListBlock : Command
    {
        public ListBlock(string name) : base(name)
        {
        }
        public override string Execute(string[] args)
        {
            Console.WriteLine("Disk Name:");
            foreach(string name in ATA.DeviceName) {
                Console.WriteLine("/dev/{0}",name);
                if (GPT.TotalPartition != -1) {
                    for (int i=1;i<=GPT.TotalPartition;i++) {
                        Console.WriteLine($" +- /dev/{name}{i}");
                    }
                } else {
                    for (int i=1;i<=MBR.TotalPartition;i++) {
                        Console.WriteLine($" +- /dev/{name}{i}");
                    }
                }
            }
            return "";
        }
    }
}