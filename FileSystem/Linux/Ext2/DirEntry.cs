using System;
using System.Runtime.InteropServices;

namespace TerminalOS_L.FileSystemR.Linux {
    public struct DirEntry {
        public uint inode;
        public UInt16 rec_len;
        public byte NameLength;
        public byte file_type;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =255)]
        public byte[] name;
    }
}