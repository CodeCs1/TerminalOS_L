using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TerminalOS_L.Driver;

namespace TerminalOS_L.FileSystemR.Microsoft {
    public class MBR {
        [StructLayout(LayoutKind.Sequential, Pack =1)]
        public  struct PartitionTable {
            public byte DriveAttribute;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst =3)]
            public byte[] CHSAddrStart;
            public byte PartitionType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst =3)]
            public byte[] CHSAddrEnd;
            public UInt32 LBAStart;
            public UInt32 NumberofSectors;
        }
        [StructLayout(LayoutKind.Sequential, Pack =1)]
        public struct MBR_t {
            public byte[] mbrboot;
            public UInt32 UniqueID;
            public UInt16 Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
            public PartitionTable[] Partitions;
            public UInt16 Signature;
        }
        public MBR_t mbr_;

        public MBR(ATA ata) {
            var mbr = new byte[512];
            ata.Read28(0,512,ref mbr);
            using var r = new BinaryReader(new MemoryStream(mbr));

            mbr_.Partitions = new PartitionTable[4];
            Message.Send_Log("Read Upper MBR");
            mbr_.mbrboot = r.ReadBytes(440);
            mbr_.UniqueID = r.ReadUInt32();
            mbr_.Reserved = r.ReadUInt16();

            Message.Send_Log("Read Partitions MBR");
            for (int i=0;i<4;i++) {
                mbr_.Partitions[i].DriveAttribute = r.ReadByte();
                mbr_.Partitions[i].CHSAddrStart = r.ReadBytes(3);
                mbr_.Partitions[i].PartitionType = r.ReadByte();
                mbr_.Partitions[i].CHSAddrEnd = r.ReadBytes(3);
                mbr_.Partitions[i].LBAStart = r.ReadUInt32();
                mbr_.Partitions[i].NumberofSectors = r.ReadUInt32();
            }

            Message.Send_Log("Read Signature MBR");
            mbr_.Signature = r.ReadUInt16();
            if (mbr_.Signature == 0xAA55) {
                Message.Send("Detected MBR");
            } else {
                Message.Send_Error("Invaild MBR Signature!");
            }
        }
        public void List() {
            Console.WriteLine("Press anykey to continue for each output.");
            for (int i=0;i<4;i++) {
                Console.WriteLine("--- MBR Log ---");
                Console.WriteLine("Partition: #"+(i+1));
                StringBuilder build = new();
                build.AppendFormat("Drive Attribute: {0}\n", (mbr_.Partitions[i].DriveAttribute & 0x80)==0 ? "Not bootable" : "Bootable");
                build.AppendFormat("Partition Type: {0:x2}\n", mbr_.Partitions[i].PartitionType);
                build.AppendFormat("LBAStart: {0}\n", mbr_.Partitions[i].LBAStart);
                build.AppendFormat("Total Sectors: {0}\n", mbr_.Partitions[i].NumberofSectors);
                Console.WriteLine(build.ToString());
                Console.ReadKey();
            }
        }
    }
}