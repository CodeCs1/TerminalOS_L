# TerminalOS_L (Terminal OS Linux reborn)
This project is used to restore the old TerminalOS Project with some new features and fixes.

If you want to see the old one, check it here: [Old project github link](https://github.com/CodeCs1/TerminalOS-v1-C-)

> [!NOTE]
> This should be worked on all PC platform[^1]

# Requirement
1. Latest version of dotnet 
2. Cosmos C# DevKit[^2]

# Compilation.
> Compilation:<br>
To compile this, open up your favorite Terminal and type this command:
```sh
dotnet build
```
Or if you're using Visual Studio, just click on Build -> Build Solution

* Road Map:
    - [x] A custom ATA PIO Driver.
    - [x] GUI forked from Win 1.0.<sup>Just done the basic</sup>
    - [x] MBR and GPT Partition table.
    - [x] Ext2, FAT32 File System Support.<sup>Read-only</sup>
    - [ ] Network Support.
    - [ ] EXT3/EXT4, NTFS File System Support.
    - [ ] A VFS replacement.
    - [ ] A custom ATAPI Driver.<sup>CDRom driver</sup>
    - [ ] Porting lua programming language.
    - [ ] ELF Reader and Loader.<sup>Exe file as well</sup>
    - [ ] Windows-like path.
    - [ ] AHCI, NVMe[^3] Driver Support.
    - [ ] Floppy Disk Driver Support.
    - [ ] Virtual Machine Guest (VirtualBox, VMWare, HyperV, QEMU)
    - [ ] Doing some USB Implementation. (xHCI)
    - [ ] Some Plug-n-Play driver.


[^1]: I don't know if Cosmos C# is support MacOS
[^2]: UserKit is ok, but I recommend using DevKit for better performance or something.
[^3]: If Cosmos support Paging.
