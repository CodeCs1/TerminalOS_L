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
        }

        public static bool ISGpt() {
            return Encoding.ASCII.GetString(pth.Signature) == "EFI PART";
        }
    }
}