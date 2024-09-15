using System;
using System.Drawing;
using System.Text;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;

namespace TerminalOS_L.FrameBuffer {
    public class FrConsole {
        [ManifestResourceStream(ResourceName = "TerminalOS_L.Windows_Based.Font.ter-v16n.psf")] public static byte[] ConsoleFont;
        private static VBECanvas consoleCanvas;
        private static int X=0,Y=0;
        private static Font f;
        public static uint Width,Height;
        public FrConsole() {
            if (Kernel.SVGASupport) {
                Message.Send_Error("SVGA Not support yet!");
                return;
            }
            Width = 800;
            Height = 600;
            consoleCanvas = new(new Mode(Width, Height, ColorDepth.ColorDepth32));
            consoleCanvas.Clear(0);
            f = PCScreenFont.LoadFont(ConsoleFont);
            consoleCanvas.Display();
        }

        public FrConsole(Mode m) {
            if (Kernel.SVGASupport) {
                Message.Send_Error("SVGA Not support yet!");
                return;
            }
            consoleCanvas = new(m);
            consoleCanvas.Clear(0);
            f = PCScreenFont.LoadFont(ConsoleFont);
            consoleCanvas.Display();
        }

        public static string ReadLine() {
            int Xread=0;
            KeyEvent ev = KeyboardManager.ReadKey();
            StringBuilder builder = new();
            while (ev.Key != ConsoleKeyEx.Enter) {
                switch (ev.Key) {
                    case ConsoleKeyEx.LeftArrow:
                        if (Xread > 0)
                            Xread--;
                        break;
                    case ConsoleKeyEx.RightArrow:
                        if (Xread < builder.Length)
                            Xread++;
                        break;
                    case ConsoleKeyEx.Backspace:
                        if (Xread > 0) {
                            WriteChr('\b');
                            Xread--;
                            builder.Remove(Xread,1);
                        }
                        else {
                            if (Kernel.UseAC97) {
                                PCSpeaker.Beep();
                            }
                        }
                        break;
                    default:
                        WriteChr(ev.KeyChar);
                        builder.Insert(Xread,ev.KeyChar);
                        Xread++;
                        break;
                }
                ev = KeyboardManager.ReadKey();
            }
            X=0;
            Y+=16;
            return builder.ToString();
        }
        public static Color ForegroundColor=Color.White;

        public static Color BackgroundColor=Color.Black;

        public static void WriteChr(char c) {
            if (X >= Width-8) {
                X=0;
                Y+=16;
            }
            if (Y >= Height-16) {
                //TODO: Scroll up
                Clear();
                X=Y=0;
            }
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
                    if (X > 0) {
                        //consoleCanvas.DrawChar('0', f, Color.Black, X-=8, Y);
                        consoleCanvas.DrawFilledRectangle(Color.Black,X-=8,Y,8,16);
                    }
                    break;
                default:
                    consoleCanvas.DrawChar(' ',f,BackgroundColor,X,Y);
                    consoleCanvas.DrawChar(c,f,ForegroundColor,X,Y);
                    X+=8;
                    break;
            }
            consoleCanvas.Display();
        }
        public static void WriteLine(){
            X=0;
            Y+=16;
        }
        public static void WriteLine(string str) {
            for (int i=0;i<str.Length;i++) {
                WriteChr(str[i]);
            }
            X=0;
            Y+=16;
        }
        public static void Write(string str) {
            for (int i=0;i<str.Length;i++) {
                WriteChr(str[i]);
            }
        }
        public static void ResetColor() {
            ForegroundColor = Color.White;
            BackgroundColor = Color.Black;
        }
        public static void Clear() {
            consoleCanvas.Clear();
            consoleCanvas.Display();
            X=Y=0;
        }
        public static ConsoleKeyEx ReadKey() {
            return KeyboardManager.ReadKey().Key;
        }
    }
}