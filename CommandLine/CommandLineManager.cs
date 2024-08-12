using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalOS_L {
    public class CommandManager {
        private readonly List<Command> CommandInterface;
        public CommandManager() {
            CommandInterface = new List<Command>
            {
                new Echo("echo"),
                new TestCase("test"),
                new ListBlock("lsblk"),
                new Win("win"),
                new Mount("mount")
            };
        }

        public string Input(string input) {
            string[] spl = input.Split(' ');
            string commandname = spl[0];

            List<string> args = new ();
            int count=0;
            foreach(string arg in spl) {
                if (count != 0) {
                    args.Add(arg);
                }
                ++count;
            }
            foreach(Command cmd in CommandInterface) {
                if (cmd.name == commandname) {
                   cmd.Execute(args.ToArray());
                }
                if (commandname.Length == 0) {
                    return "";
                }
            }
            return $"Command,File '{commandname}' not found";
        }
    }
}