using System;
using System.IO;
using System.Text;
using TerminalOS_L.Driver;
using TerminalOS_L.System;

namespace TerminalOS_L {
            //MemoryStream memtest2 = new(a);
    public class TestCase : Command {
        public TestCase (string name) : base(name) {}
        public override string Execute(string[] args)
        {
            Console.WriteLine("All test case will be placed in this test command");

            //Seek with MemoryStream
            byte[] a = {3,4,2,5,6};
            MemoryStream memtest = new(a);
            StringBuilder build =new();
            build.AppendFormat("First Read: {0}\n", memtest.ReadByte());
            memtest.Seek(2,SeekOrigin.Current);
            var test2 = memtest.ReadByte();
            build.AppendFormat("Seek Read: {0}\n", test2);
            if (test2 != 5) {
                Console.WriteLine("Test failed.");
                return "Failed.";
            }
            Console.WriteLine(build.ToString());
            //Seek with BinaryReader
            var memtest2 = new BinaryReader(new MemoryStream(a));
            StringBuilder build2 =new();
            build2.AppendFormat("First Read: {0}\n", memtest2.ReadByte());
            memtest2.BaseStream.Seek(2,SeekOrigin.Current);
            var test3 = memtest2.ReadByte();
            build2.AppendFormat("Seek Read: {0}\n", test3);
            if (test3 != 5) {
                Console.WriteLine("Test failed.");
                return "Failed.";
            }
            Console.WriteLine(build2.ToString());
            // Uncomment this to start the BSOD test.
            //DeathScreen screen = new("Test passed.");screen.DrawGUI();
            Message.Send("If you gone this far, then congrat!");

            return "";
        }
    }
}