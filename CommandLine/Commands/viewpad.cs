using System;
using TerminalOS_L.FrameBuffer;
using TerminalOS_L.Misc;
using TerminalOS_L.TSystem;

//Ah yes, the return of viewpad program.
namespace TerminalOS_L.BuiltinProgram  {
    public class Viewpad : Command {
        public Viewpad() : base("viewpad") { }

        public override string Execute(string[] args) {
            if (args.Length < 1) {
                FrConsole.WriteLine("Usage: viewpad <filename>");
                return "No argument given";
            }
            Console.WriteLine("[Viewpad]");
            string rfile=Getroot.RegisteredVFS[Getroot.RegisteredVFSIndex].ReadFile(args[0]);
            FrConsole.Write(rfile);
            return "";
        }
    }
}