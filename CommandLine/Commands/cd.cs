using System;
using TerminalOS_L.FrameBuffer;
using TerminalOS_L.Misc;

namespace TerminalOS_L.BuiltinProgram {
    public class cd:Command {
        public cd() : base("cd") {}
        public override string Execute(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Usage: cd <path>");
                return "Not enough arguments";
            }
            if (Getroot.ata == null) {
                FrConsole.WriteLine("ATA need to be mount first.");
                return "";
            }
            Getroot.RegisteredVFS[Getroot.RegisteredVFSIndex].ChangePath(args[0]);
            return "";
        }
    }
}