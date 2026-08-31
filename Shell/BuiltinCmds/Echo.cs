namespace TerminalOS_Lgen3.Shell.BuiltinCmds {
    public class Echo : IBuiltinCmd
    {
        public string CmdName => "echo";

        int IBuiltinCmd.Execute(string[] args)
        {
            foreach (var s in args) Console.Write($"{s} ");
            Console.WriteLine();
            return 0;
        }
    }
}
