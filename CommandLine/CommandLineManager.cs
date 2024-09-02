using System;
using System.Collections.Generic;
using System.Linq;
using TerminalOS_L.BuiltinProgram;
using TerminalOS_L.FrameBuffer;
using TerminalOS_L.Misc;

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
                new Mount("mount"),
                new Getroot("getroot"),
                new ls("ls"),
                new Viewpad(),
                new cd(),
                new SetMode()
            };
        }

        public string Command(string input) {
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
            string result = $"Command,File '{commandname}' not found";
            foreach(Command cmd in CommandInterface) {
                if (cmd.name == commandname) {
                    result = cmd.Execute(args.ToArray());
                }
                if (commandname.Length == 0) {
                    return "";
                }
            }
            return result;
        }
        public string Input(string input) {
            string result = "";
            if (input.Contains("&&")) {
                string[] spl = input.Split("&&");
                foreach(string command in spl) {
                    try {
                        result = Command(command);
                    } catch(Exception ex) {
                        FrConsole.WriteLine($"Getting Error while running command!\nException: {ex.Message}");
                        return "";
                    }
                }
            } else {
                try {
                    result = Command(input);
                } catch(Exception ex) {
                    FrConsole.WriteLine($"Getting Error while running command!\nException: {ex.Message}");
                    return "";
                }
            }
            return result;
        }
    }
}