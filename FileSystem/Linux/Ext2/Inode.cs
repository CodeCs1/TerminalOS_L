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
        [StructLayout(LayoutKind.Explicit)]
        public struct OS_value1 {
            [Serializable]
            public struct Linux {
                /// <summary>
                /// Little-Endian value
                /// </summary>
                public uint Reserved;
            }

            [Serializable]
            public struct Hurd {
                /// <summary>
                /// Little-Endian value
                /// </summary>
                public uint Translator;
            }

            [Serializable]
            public struct Masix {
                /// <summary>
                /// Little-Endian value
                /// </summary>
                public uint Reserved;
            }
        }
        public uint OS_Value1;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =12)]
        public uint[] DirectBlockPointer;
        public uint SinglyIndirectBlockPointer;
        public uint DoublyIndirectBlockPointer;
        public uint TriplyIndirectBlockPointer;
        public uint GenerationNumber;
        public uint ExtendedAttributeBlock;
        public uint UpperFileSize;
        public uint BlockAddr;
        
        [StructLayout(LayoutKind.Explicit)]
        public struct OS_value2 {
            [Serializable]
            public struct Linux {
                public byte frags;
                public byte frags_size;
                public UInt16 Reserved1;
                public ushort HighUserID;
                public ushort HightGroupID;
                public uint Reserved2;
            }
            [Serializable]
            public struct Hurd {

            }
            [Serializable]
            public struct Masix {
                public byte FragmentNumber;
                public byte FragmentSize;
                public ushort Pad;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst =2)]
                public uint[] Reserved;
            }
        }

        public uint OS_Value2;
    }
}