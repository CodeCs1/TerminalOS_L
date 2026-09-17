# TerminalOS_Lgen3  (Gen3 branch version)
![sign](Artwork/OS_Sign.png)

[![NETver](https://img.shields.io/badge/.Net_version-10.0-green?logo=dotnet&?style=flat)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

This branch is only compatible with Cosmos Gen3 version

## Requirement
1. Latest version of dotnet 
2. Cosmos C# Gen 3

## Compile
If you use `dotnet`, run:
```sh
$ dotnet build
```

If you have `Cosmos.Tools` installed and setting up correctly, run:
```sh
$ cosmos build
```

## Road Map
- [ ] EXT2/EXT3/EXT4, NTFS File System Support. (Changed since gen3 now support storage)
- [ ] Support syscall stuff (be able to jump into Ring3 (user mode))
- [ ] Use Elf executable as main executable file.
- [ ] Floppy Disk Driver Support.
- [ ] Intel High Definition Audio (IHDA).
- [ ] VM Guest
- [ ] Plug n Play
- [ ] USB support
