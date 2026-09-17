namespace TerminalOS_Lgen3.Shell.BuiltinCmds {
    public class Clear : IBuiltinCmd
    {
        public string CmdName => "clear";

        int IBuiltinCmd.Execute(string[] args)
        {
            Console.Clear();
            return 0;
        }
    }
}
