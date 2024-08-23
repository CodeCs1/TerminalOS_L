using System;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.Misc;

namespace TerminalOS_L.BuiltinProgram {
    public class ls : Command {
        public ls(string name) : base(name) {

        }
        public override string Execute(string[] args) {
            if (args.Length < 1) {
                Getroot.ext2.List();
                return "";
            } else {
                Console.WriteLine("Not support path listing yet.");
            }

            return "";
        }
    }
}