using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TerminalOS_L.Driver;

namespace TerminalOS_L.FileSystemR {
    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct PMBR {
        public byte BootIndicator;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =3)]
        public byte[] StartingCHS;
        public byte OSType;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =3)]
        public byte[] EndingCHS;
        public UInt32 StartingLBA;
        public UInt32 EndingLBA;
    }
    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct PTH {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
        public byte[] Signature;
        public UInt32 Revision;
        public UInt32 HeaderSize;
        public UInt32 CRC32;
        public UInt32 Reserved;
        public UInt64 LBAHeader;
        public UInt64 LBAAltHeader;
        public UInt64 FirstUsableBlock;
        public UInt64 LastUsableBlock;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =16)]
        public byte[] GUID;
        public UInt64 StartLBA;
        public UInt32 PartitionEntries;
        public UInt32 SizeOfEachEntry;
        public UInt32 CRC32_PartitionEntry;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct PartitionType {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =16)]
        public byte[] PartitionTypeGUID;

        [MarshalAs(UnmanagedType.ByValArray,SizeConst =16)]
        public byte[] UniquePartitionGUID;

        public UInt64 StartingLBA;
        public UInt64 EndingLBA;
        public UInt64 Attributes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =72)]
        public byte[] PartitionName;
    }

    public class GPT {
        private readonly PMBR pmbr;
        public static PTH pth;
        public static PartitionType[] type;
        public static int TotalPartition=-1;

        public GPT(ATA ata) {
            byte[] PMBR_data = new byte[512];
            ata.Read28(0, 512, ref PMBR_data);

            var GPTReader = new BinaryReader(new MemoryStream(PMBR_data));
            pmbr.BootIndicator = GPTReader.ReadByte();
            pmbr.StartingCHS = GPTReader.ReadBytes(3);
            pmbr.OSType = GPTReader.ReadByte();
            pmbr.EndingCHS = GPTReader.ReadBytes(3);
            pmbr.StartingLBA = GPTReader.ReadUInt32();
            pmbr.EndingLBA = GPTReader.ReadUInt32();

            byte[] PTH = new byte[512];
            ata.Read28(1, 512, ref PTH);
            GPTReader = new BinaryReader(new MemoryStream(PTH));
            pth.Signature = GPTReader.ReadBytes(8);
            pth.Revision = GPTReader.ReadUInt32();
            pth.HeaderSize = GPTReader.ReadUInt32();
            pth.CRC32 = GPTReader.ReadUInt32();
            pth.Reserved = GPTReader.ReadUInt32();
            pth.LBAHeader = GPTReader.ReadUInt64();
            pth.LBAAltHeader = GPTReader.ReadUInt64();
            pth.FirstUsableBlock = GPTReader.ReadUInt64();
            pth.LastUsableBlock = GPTReader.ReadUInt64();
            pth.GUID = GPTReader.ReadBytes(16);
            pth.StartLBA = GPTReader.ReadUInt64();
            // TODO: Fix the litmitation.
            // Although total partition in GPT is unlimited, it's hard for developor to allocate a Byte array
            // Limit GPT Drive: 32
            byte[] PT = new byte[512*64]; 
            ata.Read28(2, 512*64, ref PT);
            type = new PartitionType[32];
            BinaryReader PartitionEntry =new(new MemoryStream(PT));
            int PartitionCount=0;
            for (;PartitionCount<32;PartitionCount++) {
                type[PartitionCount].PartitionTypeGUID = PartitionEntry.ReadBytes(16);
                type[PartitionCount].UniquePartitionGUID = PartitionEntry.ReadBytes(16);
                type[PartitionCount].StartingLBA = PartitionEntry.ReadUInt64();
                type[PartitionCount].EndingLBA = PartitionEntry.ReadUInt64();
                type[PartitionCount].Attributes = PartitionEntry.ReadUInt64();
                type[PartitionCount].PartitionName = PartitionEntry.ReadBytes(72);
                //Sometime binaryreader read byte is max of uint type.
                if (type[PartitionCount].StartingLBA == ulong.MaxValue && type[PartitionCount].EndingLBA == ulong.MaxValue) { // Why?
                    break;
                } else if (type[PartitionCount].StartingLBA == 0 && type[PartitionCount].EndingLBA == 0) {
                    break;
                }
            }

            TotalPartition = PartitionCount;
        }

        public static bool ISGpt() {
            return Encoding.ASCII.GetString(pth.Signature) == "EFI PART";
        }

    }
}