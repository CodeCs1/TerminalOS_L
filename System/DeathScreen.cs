using System;
using System.Drawing;
using System.Threading;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
namespace TerminalOS_L.System {
    public class DeathScreen {
        private readonly string message;
        public DeathScreen(string message) {
            this.message = message;
        }
        public void DrawGUI() {
            if (Kernel.SVGASupport) {
                Message.Send_Error("I'm currently not support SVGAs yet!");
                return;
            }
            Canvas canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(640, 480, ColorDepth.ColorDepth32));

            canvas.Clear(153);
            canvas.DrawString("TerminalOS_L reborn",PCScreenFont.Default,Color.White,(640/2)-100,0);
            canvas.DrawString("*** KERNEL HALTED ***",PCScreenFont.Default,Color.White,0,20);
            canvas.DrawString($"Error Occurred: <{message}>",PCScreenFont.Default,Color.White,0,40);
            canvas.DrawString($"This Kernel is still in development, so it may have some bugs while using it.",PCScreenFont.Default,Color.White,0,60);
            //canvas.DrawString($"Registers info: EAX:{XSharp.XSRegisters.EAX}",PCScreenFont.Default,Color.White,0,50);
            canvas.DrawString("For possible fixes, visit: https://github.com/codecs1/TerminalOS_L/issues",PCScreenFont.Default,Color.White,0,250);
            //canvas.DrawString("or from https://github.com/CosmosOS/Cosmos/issues",PCScreenFont.Default,Color.White,0,270);
            canvas.Display();
            Thread.Sleep(10000); // 10 seconds
            canvas.DrawString("To restart TerminalOS Kernel, Press CTRL+ALT+DEL",PCScreenFont.Default,Color.White,0,280);
            canvas.DrawString("To continue, Press Enter <please don't do that>",PCScreenFont.Default,Color.White,0,300);
            canvas.Display();
            while(true) {
                KeyEvent e = Cosmos.System.KeyboardManager.ReadKey();
                if ((e.Modifiers & ConsoleModifiers.Control)==ConsoleModifiers.Control && 
                (e.Modifiers & ConsoleModifiers.Alt)==ConsoleModifiers.Alt) {
                    if (e.Key == ConsoleKeyEx.Delete) {
                        Cosmos.System.Power.Reboot();
                    }
                } else if (e.Key == ConsoleKeyEx.Enter) {
                    canvas.Disable();
                }
            }
        }
    }
}