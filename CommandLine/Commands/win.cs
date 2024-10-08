using System;
using System.Drawing;
using Sys=Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.HAL.Drivers.Video.SVGAII;

namespace TerminalOS_L {
    public class Win : Command {
        public Win(string name) : base(name) {}
        VBECanvas canvas;
        VMWareSVGAII vm;
        private readonly int[] Windows1Cursor = new int[]
            {
                1,0,0,0,0,0,0,0,0,0,0,0,
                1,1,0,0,0,0,0,0,0,0,0,0,
                1,2,1,0,0,0,0,0,0,0,0,0,
                1,2,2,1,0,0,0,0,0,0,0,0,
                1,2,2,2,1,0,0,0,0,0,0,0,
                1,2,2,2,2,1,0,0,0,0,0,0,
                1,2,2,2,2,2,1,0,0,0,0,0,
                1,2,2,2,2,2,2,1,0,0,0,0,
                1,2,2,2,2,2,2,2,1,0,0,0,
                1,2,2,2,2,2,2,2,2,1,0,0,
                1,2,2,2,2,2,2,2,2,2,1,0,
                1,2,2,2,2,2,2,1,1,1,1,1,
                1,2,2,2,1,2,2,1,0,0,0,0,
                1,2,2,1,1,2,2,1,0,0,0,0,
                1,2,1,0,0,1,2,2,1,0,0,0,
                1,1,0,0,0,1,2,2,1,0,0,0,
                1,0,0,0,0,0,1,2,2,1,0,0,
                0,0,0,0,0,0,1,2,2,1,0,0,
                0,0,0,0,0,0,0,1,2,2,1,0,
                0,0,0,0,0,0,0,1,2,2,1,0,
                0,0,0,0,0,0,0,0,1,1,0,0
            };

        private void DrawCursor(uint x, uint y) {
            for (uint i=0;i<21;i++) {
                for (uint j=0;j<12;j++) {
                    if (Windows1Cursor[i*12+j]==1) {
                        canvas.DrawPoint(Color.Black, (int)(j + x), (int)(i + y));
                    }
                    if (Windows1Cursor[i*12+j]==2) {
                        canvas.DrawPoint(Color.White, (int)(j + x), (int)(i + y));
                    }
                }
            }
        }

        private void DrawCursorVMWareGraphics(uint x, uint y) {
            for (uint i=0;i<21;i++) {
                for (uint j=0;j<12;j++) {
                    if (Windows1Cursor[i*12+j]==1) {
                        vm.SetPixel(j + x, i + y, 0);
                    }
                    if (Windows1Cursor[i*12+j]==2) {
                        vm.SetPixel(j + x, i + y,15);
                    }
                }
            }
        }
        public override string Execute(string[] args)
        {
            if (!Kernel.SVGASupport) {
                canvas = new(new Mode(800, 600, ColorDepth.ColorDepth32));
                /*Init the cursor*/
                canvas.Clear(5635925);
                
                Sys.MouseManager.ScreenWidth = 800;
                Sys.MouseManager.ScreenHeight = 600;
                Sys.MouseManager.X = 800/2;
                Sys.MouseManager.Y = 600/2;

                while(true) {
                    MSExe exe=new(canvas,"MSExecutable", 100,100);
                    DrawCursor(Sys.MouseManager.X,Sys.MouseManager.Y);
                    exe.Init();
                    canvas.Display();
                    canvas.Clear(5635925);
                }
            } else {
                vm = new();
                vm.Clear(5635925);
                Sys.MouseManager.ScreenWidth = 640;
                Sys.MouseManager.ScreenHeight = 480;
                Sys.MouseManager.X = 640/2;
                Sys.MouseManager.Y = 480/2;
                vm.SetMode(640,480);
                while(true) {
                    DrawCursorVMWareGraphics(Sys.MouseManager.X,Sys.MouseManager.Y);
                }
                
            }
        }
    }
}