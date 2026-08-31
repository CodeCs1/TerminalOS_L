using TerminalOS_Lgen3.Shell.BuiltinCmds;

public class Ls : IBuiltinCmd
{
    public string CmdName => "ls";

    public int Execute(string[] args)
    {
        string path = TerminalOS_Lgen3.Path.Path.Format(args.Length == 0 ? "." : args[0]);

        string[] files = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);

        foreach (string directory in directories)
        {
            Console.WriteLine(directory);
        }
        foreach (string file in files)
        {
            Console.WriteLine(file);
        }

        return 0;
    }
}
