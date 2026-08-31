using TerminalOS_Lgen3.Shell.BuiltinCmds;

public class Mount : IBuiltinCmd
{
    public string CmdName => "mount";

    public int Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"Usage: {CmdName} <fs> <dest>");
            return 1;
        }
        return 0;
    }
}
