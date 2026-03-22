using Cosmos.Kernel.Core.IO;
using TerminalOS_Lgen3.Disk;
using TerminalOS_Lgen3.Drivers.AHCI;
using Sys = Cosmos.Kernel.System;

namespace TerminalOS_Lgen3;

/// <summary>
/// Main kernel class - inherits from Cosmos.Kernel.System.Kernel.
/// </summary>
public class Kernel : Sys.Kernel
{
    private readonly IDisk?[] disks = [
        AHCI.Init()
    ];
    protected override void BeforeRun()
    {
        Console.WriteLine("Welcome to TerminalOS_L gen3");
        Console.WriteLine("* Initializing Disk driver...");
        foreach(var d in disks) {
            if (!DiskDriver.AddDisk(d)) {
                Console.WriteLine("* A disk driver is not exist, skip");
                continue;
            }
            var b = d!.Read(0,0,511);
            Serial.Write($"First byte: {b[0]:x}\n");
        }
        if (!DiskDriver.Dump()) Console.WriteLine("* No disk available 2 dump");
    }

    protected override void Run()
    {
        Console.Write("# ");
        Console.ReadLine();
    }
}
