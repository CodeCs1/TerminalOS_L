using TerminalOS_Lgen3.Shell.BuiltinCmds;

public class Files : IBuiltinCmd
{
    public string CmdName => "file";

    public int Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"Usage: {CmdName} <task (read/write)> <path> <cont-for write arguments>");
            return 1;
        }
        string task = args[0];
        string path = TerminalOS_Lgen3.Path.Path.Format(args[1]);
        switch(task) {
            case "read":
                if (!File.Exists(path))
                {
                    Console.WriteLine($"File {path} not exist!");
                    return 1;
                }
                Console.WriteLine($"{File.ReadAllText(path)}");
                break;
            case "write":
                File.WriteAllText(path, args[2]);
                break;
            default:
                Console.WriteLine($"Unknown task '{task}'");
                break;
        }
        return 0;
    }
}
