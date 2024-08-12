using System;
using System.Drawing;
using Sys=Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.Core.Memory;
using Cosmos.HAL.Drivers.Video.SVGAII;

namespace TerminalOS_L {
    public class Win : Command {
        public Win(string name) : base(name) {}
        Canvas canvas;
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
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(640, 480, ColorDepth.ColorDepth32));
                /*Init the cursor*/
                canvas.Clear(5635925);
                
                Sys.MouseManager.ScreenWidth = 640;
                Sys.MouseManager.ScreenHeight = 480;
                Sys.MouseManager.X = 640/2;
                Sys.MouseManager.Y = 480/2;
                MSExe exe = new (canvas,"MS Executable",640,480);

                while(true) {
                    Point cur = new((int)Sys.MouseManager.X, (int)(int)Sys.MouseManager.Y);
                    canvas.DrawPoint(Color.Black, cur.X, cur.Y);

                    if (Sys.MouseManager.MouseState == Sys.MouseState.Left) {
                        Sys.PCSpeaker.Beep();
                    }

                    exe.Init();
                    DrawCursor(Sys.MouseManager.X,Sys.MouseManager.Y);
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