namespace TerminalOS_Lgen3.Shell.BuiltinCmds {
    public interface IBuiltinCmd {
        public int Execute(string[] args);
        string CmdName { get; }
    }
}
