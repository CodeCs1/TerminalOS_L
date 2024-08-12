using System;
using System.IO;
using System.Text;
using TerminalOS_L.Driver;

namespace TerminalOS_L.FileSystemR.Microsoft.FAT32
{
    public class FAT32 {
        public BootRecord br;
        public FAT32(ATA ata, int LBA_Start) {
            byte[] BPB = new byte[512];
            ata.Read28(LBA_Start,512,ref BPB);
            using var r = new BinaryReader(new MemoryStream(BPB));
            br.ShortJmp = r.ReadBytes(3);
            br.OEM = r.ReadBytes(8);
            br.BytePerSectors=r.ReadUInt16();
            br.NumberofSectors = r.ReadByte();
            br.ReservedSectors = r.ReadUInt16();
            br.NumberofFat = r.ReadByte();
            br.NumberofRoot = r.ReadUInt16();
            br.totalSectors = r.ReadUInt16();
            br.Media = r.ReadByte();
            br.SectorsPerTrack = r.ReadUInt16();
            br.NumberofHead = r.ReadUInt16();
            br.HiddenSectors = r.ReadUInt32();
            br.SectorsCount = r.ReadUInt32();
            br.SectorsPerFAT = r.ReadUInt32();
            br.Flags = r.ReadUInt16();
            br.FATVersion = r.ReadUInt16();
            br.ClusterNumber = r.ReadUInt16();
            br.ClusterNumber = r.ReadUInt32();
            br.Sectorsofinfo = r.ReadUInt16();
            br.SectorsofBackup = r.ReadUInt16();
            br.Reserved = r.ReadBytes(12);
            br.DriveNumber = r.ReadByte();
            br.ReservedNT = r.ReadByte();
            br.Signature = r.ReadByte();
            br.VOlumeID = r.ReadUInt32();
            br.VolumeLabel = r.ReadBytes(11);
            br.SysIdentifiString = r.ReadBytes(8);
            var builder = new StringBuilder();
            builder.AppendFormat("Volume Label: {0}\n",Encoding.ASCII.GetString(br.VolumeLabel));
            builder.AppendFormat("System Identifier String: {0}\n",Encoding.ASCII.GetString(br.SysIdentifiString));
            builder.AppendFormat("OEM: {0}\n",Encoding.ASCII.GetString(br.OEM));
            builder.AppendFormat("Fatsz32: {0}\n",br.SectorsPerFAT);
            Console.WriteLine(builder.ToString());

            byte[] Fat = new byte[br.SectorsPerFAT];
            ata.Read28(LBA_Start + br.ReservedSectors, (int)br.SectorsPerFAT, ref Fat);
            Kernel.PrintByteArray(Fat);
        }
    }
}