using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;

namespace TerminalOS_L.FrameBuffer {
    public class FrConsole {
        [ManifestResourceStream(ResourceName = "TerminalOS_L.Windows_Based.Font.ter-v16n.psf")] public static byte[] ConsoleFont;
        private static Canvas consoleCanvas;
        private static int X=0,Y=0;
        private static Font f;
        public FrConsole() {
            if (Kernel.SVGASupport) {
                Message.Send_Error("SVGA Not support yet!");
                return;
            }
            consoleCanvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(640, 480, ColorDepth.ColorDepth32));
            consoleCanvas.Clear(0);

            f = PCScreenFont.LoadFont(ConsoleFont);
            consoleCanvas.Display();
        }

        public static string ReadLine() {
            return "";
        }

        public static void WriteChr(char c) {
            switch(c) {
                case '\n':
                    Y+=16;
                    X=0;
                    break;
                case '\t':
                    X += 32; // 8*4
                    break;
                case '\r':
                    X = 0;
                    break;
                case '\b':
                    consoleCanvas.DrawChar(' ', f, Color.Black, X, Y);
                    X--;
                    break;
                default:
                    consoleCanvas.DrawChar(c,f,Color.White,X,Y);
                    X+=8;
                    break;
            }
            if (X >= 640-8) {
                X=0;
                Y+=16;
            }
            if (Y >= 480-16) {
                //TODO: Scroll up
                Clear();
                X=Y=0;
            }
            consoleCanvas.Display();
        }
        public static void WriteLine(string str) {
            for (int i=0;i<str.Length;i++) {
                WriteChr(str[i]);
            }
            X=0;
            Y+=16;
        }
        public static void Clear() {
            consoleCanvas.Clear();
            consoleCanvas.Display();
            X=Y=0;
        }
    }
}