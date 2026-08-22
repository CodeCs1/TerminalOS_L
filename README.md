# TerminalOS_Lgen3  (Gen3 branch version)
![sign](Artwork/OS_Sign.png)

[![NETver](https://img.shields.io/badge/.Net_version-10.0-green?logo=dotnet&?style=flat)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

This branch is only compatible with Cosmos Gen3 version

> [!NOTE]
> This ONLY work for BIOS as UEFI cause Page Fault on RTC Initialize (Will be removed if fixed)

## Requirement
1. Latest version of dotnet 
2. Cosmos C# Gen 3

## Compile project

If you use `dotnet`, run:
```sh
$ dotnet build
```

If you have install `cosmos` tool, run:
```sh
$ cosmos build
```

Or if you're using Visual Studio, just click on Build -> Build Solution (hasn't test yet)

## Road Map
- [ ] EXT2/EXT3/EXT4, NTFS File System Support. (Changed since gen3 now support storage)
- [ ] Use windows (coff/pe32+) executable as main executable file (POSIX compatible)
- [ ] Floppy Disk Driver Support.
- [ ] Intel High Definition Audio (IHDA).
- [ ] VM Guest
- [ ] Plug n Play
- [ ] USB support
