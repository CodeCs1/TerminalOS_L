using System;
using TerminalOS_L.Driver;

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
                Console.WriteLine(" +- /dev/{0}1",name);
            }
            return "";
        }
    }
}