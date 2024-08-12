using System;
using System.Runtime.InteropServices;

namespace TerminalOS_L.FileSystemR.Microsoft.FAT32 {
    [StructLayout(LayoutKind.Sequential,Pack =1)]
    public struct BootRecord {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =3)]
        public byte[] ShortJmp;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =8)]
        public byte[] OEM;
        public UInt16 BytePerSectors;
        public byte NumberPerCluster;
        public ushort ReservedSectors;
        public byte NumberofFat;
        public ushort NumberofRoot;
        public ushort totalSectors;
        public byte Media;
        /// <summary>
        /// FatFz16 field
        /// </summary>
        public ushort NumberofSectors;
        public ushort SectorsPerTrack;
        public ushort NumberofHead;
        public uint HiddenSectors;
        public uint SectorsCount;
        /*FAT32*/

        /// <summary>
        /// FatFz32 field
        /// </summary>
        public uint SectorsPerFAT;
        public ushort Flags;
        public ushort FATVersion;
        public uint ClusterNumber;
        public ushort Sectorsofinfo;
        public ushort SectorsofBackup;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =12)]
        public byte[] Reserved;
        public byte DriveNumber;
        public byte ReservedNT;
        public byte Signature;
        public uint VOlumeID;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =11)]
        public byte[] VolumeLabel;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =8)]
        public byte[] SysIdentifiString;

    }
    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct DirectoryEntry {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=11)]
        public byte[] name;
        public byte Attribute;
        public byte Reserved;
        public byte CreationTime;
        public ushort TimeCreated;
        public ushort DateCreated;
        public ushort LastAccessedDate;
        public ushort High16FirstCluster;
        public ushort LastModificationTime;
        public ushort LastModificationDate;
        public ushort Low16BitFirstCluster;
        public uint SizeofFile;
    }
}