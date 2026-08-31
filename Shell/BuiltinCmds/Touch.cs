using TerminalOS_Lgen3.Shell.BuiltinCmds;

public class Touch : IBuiltinCmd
{
    public string CmdName => "touch";

    public int Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"Usage: {CmdName} <file>");
            return 1;
        }
        if (args.EndsWith("/")) {
            Console.WriteLine($"{CmdName}: Path must be a file, not folder.");
            return 1;
        }
        string path = TerminalOS_Lgen3.Path.Path.Format(args[0]);

        if (File.Exists(path)) {
            File.SetLastAccessTime(path, DateTime.Now);
            File.SetLastWriteTime(path, DateTime.Now);
        } else {
            File.Create(path);
        }

        return 0;
    }
}
