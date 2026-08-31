using Sys = Cosmos.Kernel.System;
using Cosmos.Kernel.System.Vfs;
using Cosmos.Kernel.System.Filesystems.Fat;
using Cosmos.Kernel.HAL.Vfs;
using TerminalOS_Lgen3.Disk;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace TerminalOS_Lgen3;

/// <summary>
/// Main kernel class - inherits from Cosmos.Kernel.System.Kernel.
/// </summary>
public class Kernel : Sys.Kernel
{
    protected override void BeforeRun()
    {
        Console.WriteLine("Welcome to TerminalOS_L gen3");
        Console.Write("[*] Creating RAM Disk for temporary use... (32 MB) ");
        MemoryBlockDevice memoryBlockDevice = new("RAMDISK", 512, 65536);
        FatFilesystemType fat = new(memoryBlockDevice);
        fat.TryFormat(default, new FatFormatOptions { Type = FatType.Fat16 });
        VfsManager.RegisterFilesystem("fat", fat);
        Console.WriteLine($"{VfsManager.TryMount("fat", "", MountFlags.None, "/root", out _)}");
    }

    protected override void Run()
    {
        Console.Write($"[{Path.Path.CurrentPath}] $ ");
        string cmd = Console.ReadLine()!;
        Shell.Shell s = new(cmd);
        s.Execute();
    }
}
