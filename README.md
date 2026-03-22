# TerminalOS_Lgen3  (Gen3 branch version)
![sign](Artwork/OS_Sign.png)

[![NETver](https://img.shields.io/badge/.Net_version-10.0-green?logo=dotnet&?style=flat)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

This branch is only compatible with Cosmos Gen3 version

> [!NOTE]
> This should be worked on all PC platform

## Requirement
1. Latest version of dotnet 
2. Cosmos C# Gen 3

## Compile project
> Compiling this by typing this command to Terminal.
```sh
dotnet build
```
> Or if you're using Visual Studio, just click on Build -> Build Solution

## Road Map
- [x] AHCI Driver Support.
- [ ] A custom ATA PIO Driver.
- [ ] MBR and GPT Partition table.
- [ ] Ext2, FAT32 File System Support.
- [ ] EXT3/EXT4, NTFS File System Support.
- [ ] A VFS replacement.
- [ ] A custom ATAPI Driver.<sup>CDRom driver</sup>
- [ ] Use windows executable as main executable file (POSIX compatible)
- [ ] Floppy Disk Driver Support.
- [ ] Intel High Definition Audio (IHDA).
- [ ] VM Guest
- [ ] Plug n Play
- [ ] USB support
