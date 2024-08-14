# TerminalOS_L (Terminal OS Linux reborn)
This project is used to restore the old TerminalOS Project which broken for some reason (I think it's caused by Cosmos C# Kernel itself).

If you want to see it, check here: [Old project github link](https://github.com/CodeCs1/TerminalOS-v1-C-)

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

* Road Map:
    - [x] A custom ATA PIO Driver.
    - [x] GUI forked from Win 1.0.<sup>Just done the basic</sup>
    - [x] MBR and GPT Partition table
    - [ ] Network Support<br>
    - [ ] EXT3/EXT4, NTFS File System Support.<br>
    - [ ] A VFS replacement

- Developmenting:
> EXT2/FAT32 File System Driver.

[^1]: I don't know if Cosmos C# is support MacOS
[^2]: User Kit is ok, but I recommend using DevKit for better performance or something?
