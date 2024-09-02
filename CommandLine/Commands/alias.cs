using System;
using TerminalOS_L.CFGRunner;
using System.Text;
using TerminalOS_L.CFGRunner.Core;


namespace TerminalOS_L {

    public class Alias : Command {
        public Alias(string name) :base (name){ }
        public override string Execute(string[] args)
        {
            if (args.Length==0) {
                //string res=Kernel.f.ReadFile("Loader.cfg");
                string[] command = {"a"};
                StringBuilder builder=new();
                foreach(string commands in command) {
                    switch(commands.Split(' ')[0]) {
                        case " ":
                            continue;
                        case "ret":
                        /*
                            Kernel.f.CreateFile(commands.Split(' ')[1]);
                            Kernel.f.WriteFileByString(commands.Split(' ')[1],builder.ToString());
                            */
                            builder.Clear();   
                            break;
                        default:
                            builder.Append(commands);
                            builder.Append('\n');
                            break;
                    }
                }
                return "";
            } else {
                if (args[0] == "/h") {
                    Console.WriteLine(@"Usage: alias <filename> [command]
                    command:
                        /h: show this help and quit.
                
                    Note: when run this command without command args, it will create
                    a script file from Loader.cfg");
                    return "";
                }
                CFGRunner.CFGRunner c = new();
                //string res=Kernel.ReadFile(args[0]);
                //string[] command = res.Split('\n');
                /*foreach(string commands in command) {
                    if (commands == "#!alias") continue;
                    c.Run(commands);
                }*/
                return "";
            }
        }
    }
}