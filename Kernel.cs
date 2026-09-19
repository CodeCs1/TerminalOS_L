using Sys = Cosmos.Kernel.System;
using TerminalOS_Lgen3.SystemKernel;
using Cosmos.Kernel.System.Vfs;
using Cosmos.Kernel.System.Filesystems.Fat;
using Cosmos.Kernel.HAL.Vfs;
using TerminalOS_Lgen3.Disk;
using System.Reflection;
using System.Formats.Tar;
using Cosmos.Kernel.System.Graphics;
using System.Runtime.InteropServices;
using Cosmos.Kernel.System.Diagnostics;
using Cosmos.Kernel.System.Graphics.Fonts;
using System.Drawing;

namespace TerminalOS_Lgen3;

/// <summary>
/// Main kernel class - inherits from Cosmos.Kernel.System.Kernel.
/// </summary>
public partial class Kernel : Sys.Kernel
{
    [LibraryImport("*", EntryPoint = "load_gdt")]
    public static partial void load_gdt();

    protected override void BeforeRun()
    {
        Console.WriteLine("Welcome to TerminalOS_L gen3");
        Console.WriteLine($"[The kernel run with command args: {Environment.CommandLine}]");
        Console.WriteLine("[*] Reloading GDT and TSS...");
        load_gdt();
        Log.Write($"[SYSTEM] Syscall Initialization OK: {Syscall.Init()}\n");


        Console.Write("[*] Creating RAM Disk for temporary use... (64 Mib) ");
        MemoryBlockDevice memoryBlockDevice = new("RAMDISK", 512, 131_072);
        FatFilesystemType fat = new(memoryBlockDevice);
        fat.TryFormat(default, new FatFormatOptions { Type = FatType.Fat32 });
        VfsManager.RegisterFilesystem("fat", fat);
        Console.WriteLine($"{VfsManager.TryMount("fat", "", MountFlags.None, "/root", out _)}");
        using Stream? str = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("TerminalOS_Lgen3.init.tar");
        if (str != null)
        {
            Log.WriteString("[INIT] Extracting gz content\n");
            byte[] f = new byte[str.Length];
            str.Read(f, 0, (int)str.Length);
            //I've to do this manually cuz when I use tarfile.extract, It create a tmp folder which name /tmp.
            using TarReader tarReader = new(new MemoryStream(f));
            while (tarReader.GetNextEntry() is TarEntry entry)
            {
                string fullPath = System.IO.Path.Combine("/root/", entry.Name);
                Console.Write($"{entry.Name} -> {fullPath}... ");
                try
                {
                    if (entry.EntryType == TarEntryType.Directory)
                        Directory.CreateDirectory(fullPath);
                    else if (entry.EntryType == TarEntryType.RegularFile || entry.EntryType == TarEntryType.V7RegularFile)
                    {
                        string? parentDir = System.IO.Path.GetDirectoryName(fullPath);
                        if (parentDir != null) Directory.CreateDirectory(parentDir);
                        entry.ExtractToFile(fullPath, true);
                    }
                }
                catch
                {
                    Console.WriteLine("FAILED");
                }
                finally
                {
                    Console.WriteLine("OK");
                }
            }
            Console.WriteLine("[*] Changing font...");
            try
            {
                TrueTypeFont ttf = new("/root/etc/fonts/unifont-18.0.01.ttf");
                KernelConsole.Default!.Font = ttf;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot load font: " + ex);
            }

            var canvas = Canvas.GetFullScreen();
            var png = new Png("/root/etc/img.png");
            canvas.DrawImage(png, canvas.Width - (png.Width/7), 0,png.Width/7, png.Height/7);
        }
    }

    protected override void Run()
    {
        Console.Write($"[{Path.Path.CurrentPath}] $ ");
        string cmd = Console.ReadLine()!;
        Shell.Shell s = new(cmd);
        try { s.Execute(); } catch (Exception ex) { Console.WriteLine(ex.Message); }
    }
}
