using System;
using System.Drawing;
using System.Threading;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
namespace TerminalOS_L.TSystem {
    public class DeathScreen {
        private readonly string message;
        public bool WinXP = false;
        public DeathScreen(string message) {
            this.message = message;
        }
        public void DrawGUI() {
            if (Kernel.SVGASupport) {
                Message.Send_Error("I'm currently not support SVGAs yet!");
                return;
            }
            VBECanvas canvas = new(new Mode(800, 600, ColorDepth.ColorDepth32));
            switch (WinXP) {
                case false:
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
                default:
                    canvas.Clear(153);
                    canvas.DrawString("A problem has been detected and TerminalOS_L has been shutdown to prevent damage.",PCScreenFont.Default,Color.White,0,10);
                    canvas.DrawString("to your computer.",PCScreenFont.Default,Color.White,0,20);
                    canvas.DrawString("The probem seems to be caused by the following file: <TerminalOS_L.bin>",PCScreenFont.Default,Color.White,0,40);
                    canvas.DrawString($"{message}",PCScreenFont.Default,Color.White,0,60);
                    canvas.DrawString("If this is the first time you've seen this Stop error screen,",PCScreenFont.Default,Color.White,0,80);
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
}