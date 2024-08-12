/*Welcome to the world of Virtual File System!*/
using System.Collections.Generic;
using TerminalOS_L.Driver;

/*

    We don't know what File System in each mounted file system
    So instead, this VFS here will be abstract class
    (Support: FAT32, EXT2/4, NTFS <maybe>, ISO9660)
*/

namespace TerminalOS_L.FileSystemR {
    public abstract class VFS {
        public VFS(ATA ata,uint LBA_Start) {}
        public virtual void Impl() {}
        public virtual void ReadFile() {}
        public virtual void GetInfo() {}
        public virtual string DiskLabel {get;set;}
        public virtual uint LBA_Start {get;set;}
        public virtual ATA ATA {get;}
    }
}