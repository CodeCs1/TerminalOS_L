using Cosmos.Kernel.Core.IO;
using TerminalOS_Lgen3.Shell.BuiltinCmds;

namespace TerminalOS_Lgen3.Shell {
    public class Shell
    {
        private readonly string[] Cmds;
        public Shell(string cmd) {
            List<string> new_cmd = [];
            var split = cmd.Split('"');
            for (int i = 0; i < split.Length; i++)
                split[i] = split[i].Trim();

            for (int i = 0; i < split.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var split2 = split[i].Split(' ');
                    foreach (var el in split2)
                        new_cmd.Add(el);
                }
                else new_cmd.Add(split[i]);
            }
            Cmds=[..new_cmd];
        }

        private readonly IBuiltinCmd[] builtinCmds = [
            new Echo(),
            new Files(),
            new Touch(),
            new Ls(),
            new Mount(),
            new Lsblk(),
        ];

        public void Execute()
        {
            string execute_cmd = Cmds[0];
            bool notfound = true;
            foreach (var cmd in builtinCmds)
            {
                if (cmd.CmdName == execute_cmd)
                {
                    Serial.Write($"[SHELL] Builtin Command exited with code: {cmd.Execute(Cmds[1..])}\n");
                    notfound = false;
                    break;
                }
            }
            if (notfound) Console.WriteLine($"Command '{execute_cmd}' not found!");
        }
    }
}
