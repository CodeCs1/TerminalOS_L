using System;
using TerminalOS_L.Misc;
using TerminalOS_L.System;

//Ah yes, the return of viewpad program.
namespace TerminalOS_L.BuiltinProgram  {
    public class Viewpad : Command {
        public Viewpad() : base("viewpad") { }

        public override string Execute(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Usage: viewpad <filename>");
                return "No argument given";
            }
            try {
                Console.WriteLine("[Viewpad]");
                string rfile=Getroot.ext2.ReadFile(args[0]);
                Console.WriteLine(rfile);
            } catch(Exception ex) {
                DeathScreen d = new(ex.Message);
                d.DrawGUI();
            }
            return "";
        }
    }
}