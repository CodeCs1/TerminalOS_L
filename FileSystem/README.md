## What is this folder ?
#### This folder used to support other File System disk-format method (from windows (FAT32, NTFS), tool from fdisk, cfdisk, etc).

Check here: [Cosmos VFS articles.](https://cosmosos.github.io/articles/Kernel/VFS.html)
```txt
Attention: Always format your drive with Cosmos and only Cosmos if you plan to use it with Cosmos. Using any other tool such as Parted, FDisk, or any other tool might lead to weird things when using that drive with Cosmos' VFS. Those tools are much more advanced and might format and read/write to the disk differently than Cosmos.
```

## Why don't you use Cosmos format method ?
Cuz simple answer: I need it for cross-tool support.