using System;
using TerminalOS_L.FileSystemR;
using TerminalOS_L.Misc;

namespace TerminalOS_L.BuiltinProgram {
    public class ls : Command {
        public ls(string name) : base(name) {

        }
        public override string Execute(string[] args) {
            if (args.Length < 1) {
                Getroot.ext2.List(Getroot.Path);
                return "";
            }
            if (Getroot.Path == null) {
                Message.Send_Error("VFS is not ready!");
                return "VFS is not ready!";
            }
            string path = args[0];
            Getroot.ext2.List(args[0]);

            return "";
        }
    }
}