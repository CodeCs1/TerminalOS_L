using Cosmos.Kernel.System.Storage;
using Cosmos.Kernel.System.Vfs;
using TerminalOS_Lgen3.Shell.BuiltinCmds;

public class Lsblk : IBuiltinCmd
{
    public string CmdName => "lsblk";

    public int Execute(string[] args)
    {
        Console.WriteLine("-- List available device on this system --");
        for (int i=0; i<StorageManager.DeviceCount;i++ ){
            Console.WriteLine($"{StorageManager.GetDevice(i)?.Name}");
        }
        return 0;
    }
}
