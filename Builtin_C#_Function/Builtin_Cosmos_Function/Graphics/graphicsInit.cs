using System;
using Cosmos.System.Graphics;

namespace TerminalOS_L {
    public class GraphicsInit:AliasRun {
        public GraphicsInit(string name):base(name) {}
        public override object ARun(string[] args)
        {
            //args0 -> width
            //args1 -> height
            //args2 -> ColorDepth (2^x)
            //args3 -> r
            //args4 -> g
            //args5 -> b

            if (args.Length < 6) {
                Console.WriteLine("Not enough args!");
                return null;
            }
            uint width,height,colord;
            int r,g,b;
            width = Convert.ToUInt32(args[0]);
            height = Convert.ToUInt32(args[1]);
            colord = Convert.ToUInt32(args[2]);
            r = Convert.ToInt32(args[3]);
            g = Convert.ToInt32(args[4]);
            b = Convert.ToInt32(args[5]);
            Console.WriteLine($@"
            wid: {width}
            hei: {height}
            cold: {colord}
            r: {r}
            g: {g}
            b: {b}");


            switch(colord) {
                case 4:
                case 8:
                case 16:
                case 32:
                    Graphics.Graphics.Init(width,height,r,g,b,colord);
                    break;
                default:
                    break;
            }
            return 0;
        }
    }
}