/*Welcome to the world of Virtual File System!*/
using System.Collections.Generic;
using System.Text;
using TerminalOS_L.Driver;

/*

    We don't know what File System in each mounted file system
    So instead, this VFS here will be abstract class
    (Support: FAT32, EXT2/4, NTFS <maybe>, ISO9660)
*/

namespace TerminalOS_L.FileSystemR {
    public abstract class VFS {
        public VFS(ATA ata,uint LBA_Start) {}
        public virtual int Impl() {return 0;}
        public virtual string ReadFile(string path) { return ""; }

        public virtual void ChangePath(string path) {}

        public virtual byte[] ReadFileBytes(string path) { return Encoding.ASCII.GetBytes(""); }
        public virtual void GetInfo() {}
        public virtual string DiskLabel {get;set;}
        public virtual uint LBA_Start {get;set;}
        public virtual ATA ATA {get;}
        public virtual string Type {get;}
        public virtual void List() {}
    }
}