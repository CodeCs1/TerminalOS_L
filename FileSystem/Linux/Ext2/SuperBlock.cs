using System;
using System.Runtime.InteropServices;

namespace TerminalOS_L.FileSystemR.Linux {

    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct SuperBlockEnum {
        /// <summary>
        /// s_inodes_count field
        /// </summary>
        public UInt32 TotalInodes;
        /// <summary>
        /// s_block_count field
        /// </summary>

        public UInt32 TotalBlock;
        /// <summary>
        /// s_free_blocks_count field
        /// </summary>
        public UInt32 ReservedBlock;
        /// <summary>
        /// s_free_blocks_count field
        /// </summary>

        public UInt32 TotalUnallocatedBlocks;
        public UInt32 TotalUnallocatedInodes;
        public UInt32 SuperBlockLocated;
        public UInt32 BlockSize;
        public UInt32 FragmentSize;
        /// <summary>
        /// s_block_per_group field
        /// </summary>
        public UInt32 NumberOfBlock;
        /// <summary>
        /// s_flags_per_group field
        /// </summary>
        public UInt32 NumberofFragments;
        /// <summary>
        /// s_inodes_per_group field
        /// </summary>
        public UInt32 NumberofInodes;
        public UInt32 LastMountTime;
        public UInt32 LastWrittenTime;
        public UInt16 NumberoftimeMounted;
        public UInt16 NUmberofMountAllowed;
        public UInt16 Signature;
        public UInt16 FSState;
        public UInt16 Error;
        public UInt16 MinorPortion;
        public UInt32 TimeOfLastCheck;
        public UInt32 Interval;
        public UInt32 OS_ID;
        public UInt32 MajorPortion;
        public UInt16 UserID;
        public UInt16 GroupID;
    }

    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct ExtendedSuperBlock {
        public UInt32 FirstInode;
        public UInt16 SizeOfEachINode;
        public UInt16 BlockGroup;
        public UInt32 OptionalFeatures;
        public UInt32 RequiredFeatures;
        public UInt32 FeaturesNotSupport;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =16)]
        public byte[] FsID;

        [MarshalAs(UnmanagedType.ByValArray,SizeConst =16)]
        public byte[] VolumeName;
        
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =64)]
        public byte[] PathVolumeName;
        public UInt32 CompressionAlgotithms;
        public byte NumberofBlock_File;
        public byte NumberOfBlock_Dir;
        public UInt16 Reserved;
        
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =16)]
        public byte[] JournalID;
        public UInt32 Journal_Inode;
        public UInt32 Journal_Device;
        public UInt32 HeadofOrphan;
    }
}