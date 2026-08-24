# TerminalOS_Lgen3  (Gen3 branch version)
![sign](Artwork/OS_Sign.png)

[![NETver](https://img.shields.io/badge/.Net_version-10.0-green?logo=dotnet&?style=flat)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

This branch is only compatible with Cosmos Gen3 version

## Requirement
1. Latest version of dotnet 
2. Cosmos C# Gen 3

## Compile

In order to use the OS in UEFI, you need to build [nativeaot-patcher](https://github.com/valentinbreiz/nativeaot-patcher) from source and apply patch from `packages/no-efi-time.patch`. After that, you need to copy all nupkg files from `/path/to/nativeaot-patcher/artifacts/package/release` to `packages` folder. Finally, change the cosmos sdk version inside csproj to the compiled version.

You can skip this step and delete `nuget.config` if you want BIOS only.

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
- [ ] Use windows (coff/pe32+) executable as main executable file (POSIX compatible)
- [ ] Floppy Disk Driver Support.
- [ ] Intel High Definition Audio (IHDA).
- [ ] VM Guest
- [ ] Plug n Play
- [ ] USB support
