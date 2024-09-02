using System;
using Cosmos.System.Graphics;
using TerminalOS_L.FrameBuffer;

namespace TerminalOS_L.Misc {
    public class SetMode : Command {
        public SetMode() : base ("setmode") {}
        public static string CurrentMode="800x600";
        public override string Execute(string[] args)
        {
            //setmode 800x600

            if (args.Length < 1) {
                FrConsole.WriteLine("Usage: setmode <width>x<height>");
            }

            string[] mode = args[0].Split('x');

            if (CurrentMode == args[0]) {
                Message.Send("The resolution is already set, skip");
                return "";
            }

            /*Mode m = new(Convert.ToUInt32(mode[0]),Convert.ToUInt32(mode[1]),ColorDepth.ColorDepth32);
            _ = new FrConsole(m);*/
            FrConsole.Height = Convert.ToUInt32(mode[1]);
            FrConsole.Width = Convert.ToUInt32(mode[0]);

            CurrentMode = args[0];


            return "";
        }
    }
}