# TerminalOS_L (Terminal OS Linux reborn)
![sign](Artwork/OS_Sign.png)

[![NETver](https://img.shields.io/badge/.Net_version-8.0-green?logo=dotnet&?style=flat)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

This project used to restore the old TerminalOS Project with some new features and fixes.

If you want to see the old one, check it here: [Old project github link](https://github.com/CodeCs1/TerminalOS-v1-C-)

> [!NOTE]
> This should be worked on all PC platform[^1]

## Requirement
1. Latest version of dotnet 
2. Cosmos C# DevKit[^2]

## Installation
> Compiling this by typing this command to Terminal.
```sh
dotnet build
```
> Or if you're using Visual Studio, just click on Build -> Build Solution

## Road Map
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
- [ ] Intel High Definition Audio (IHDA).
- [ ] Some network support.

### Driver that I should left it for future implement.
- Some Graphics driver (no not some proprietary one)
- Virtual Machine Guest (VirtualBox, VMWare, HyperV, QEMU).
- Some Plug-n-Play driver.
- Support PCIe.
- Doing some USB Implementation. (xHCI)


[^1]: I don't know if Cosmos C# is support MacOS
[^2]: UserKit is ok, but I recommend using DevKit for better performance or something.
[^3]: If Cosmos support Paging.
