using System.Runtime.InteropServices;

namespace TerminalOS_L.FileSystemR.Linux {
    [StructLayout(LayoutKind.Sequential,Pack =1)]
    public struct BlockGroupDescriptor {
        public uint BlockBitmap;
        public uint INodeBitMap;
        public uint InodeTable;
        public ushort FreeBlockCount;
        public ushort FreeInodeCount;
        public ushort UsedDirCount;
        public ushort Flags;
        public uint ExcludeBitmap; 
        public ushort BlockBitmapCheckSum;
        public ushort InodeBitmapCheckSum;
        public ushort ItableFree;
        public ushort checksum;
    }
}