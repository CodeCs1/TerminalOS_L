using System;
using System.Runtime.InteropServices;

namespace TerminalOS_L.FileSystemR.Linux {
    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct Inode {
        public ushort TypeAndPermissions;
        public ushort UserID;
        public uint SizeinBytes;
        public uint LastAccessTime;
        public uint CreationTime;
        public uint LastModificationTime;
        public uint DeletionTime;
        public ushort GroupID;
        public ushort CountofHardLinks;
        public uint CountofDiskSectors;
        public uint Flags;
        public uint OS_Value1;
        public uint DirectBlockPointer0;
        public uint DirectBlockPointer1;
        public uint DirectBlockPointer2;
        public uint DirectBlockPointer3;
        public uint DirectBlockPointer4;
        public uint DirectBlockPointer5;
        public uint DirectBlockPointer6;
        public uint DirectBlockPointer7;
        public uint DirectBlockPointer8;
        public uint DirectBlockPointer9;
        public uint DirectBlockPointer10;
        public uint DirectBlockPointer11;
        public uint SinglyIndirectBlockPointer;
        public uint DoublyIndirectBlockPointer;
        public uint TriplyIndirectBlockPointer;
        public uint GenerationNumber;
        public uint ExtendedAttributeBlock;
        public uint UpperFileSize;
        public uint BlockAddr;
        public uint OS_Value2;
    }
}