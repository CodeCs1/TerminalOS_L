using System;
using System.IO;
using System.Text;
using TerminalOS_L.Misc;
using TerminalOS_L.Driver.NVMe;
using TerminalOS_L.FrameBuffer;
using TerminalOS_L.Driver.VBox;

namespace TerminalOS_L {
            //MemoryStream memtest2 = new(a);
    public class TestCase : Command {
        public TestCase (string name) : base(name) {}
        public struct Test1 {
            [BitField(5)]
            public byte test1;
        }
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
            FrConsole.WriteLine(build.ToString());
            //Seek with BinaryReader
            var memtest2 = new BinaryReader(new MemoryStream(a));
            StringBuilder build2 =new();
            build2.AppendFormat("First Read: {0}\n", memtest2.ReadByte());
            memtest2.BaseStream.Seek(2,SeekOrigin.Current);
            var test3 = memtest2.ReadByte();
            build2.AppendFormat("Seek Read: {0}\n", test3);
            if (test3 != 5) {
                FrConsole.WriteLine("Test failed.");
                return "Failed.";
            }
            FrConsole.WriteLine(build2.ToString());
            // Uncomment this to start the BSOD test.
            //DeathScreen screen = new("Test passed.");screen.DrawGUI();
            if (Getroot.ata == null) {
                Message.Send_Warning("Skip ata test");
            } else {

            }
            _ = new NVMe();
            _ = new VBox();

            /*FrameBuffer.FrConsole fr =new();
            FrameBuffer.FrConsole.WriteLine("Done!");*/

            Message.Send("If you gone this far, then congrat!");

            return "";
        }
    }
}   