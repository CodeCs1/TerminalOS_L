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

            int Root_Sectors = LBA_Start+br.ReservedSectors;
            uint FatSz = br.SectorsPerFAT;
            uint Data_Start = (uint)(Root_Sectors +FatSz*br.NumberofFat);
            uint Root_Start = (uint)(Data_Start +br.NumberofSectors*(br.NumberofRoot-2));
            Console.WriteLine("Root Start: "+Root_Start);
            /*byte[] Root_Dir = new byte[512];
            ata.Read28(Root_Sectors,512,ref Root_Dir);
            Kernel.PrintByteArray(Root_Dir);*/
        }
        //This function should be resemble to Cosmos File System.
        public void ListAll() {
            var builder = new StringBuilder();
            Console.WriteLine("-------File System-------");
            builder.AppendFormat("Bytes Per Sector: {0}\n",br.BytePerSectors);
            builder.AppendFormat("Number of Sectors: {0}\n",br.NumberofSectors);
            builder.AppendFormat("Reserved Sectors: {0}\n",br.ReservedSectors);
            builder.AppendFormat("Number of FAT: {0}\n",br.NumberofFat);
            builder.AppendFormat("Number of Root: {0}\n",br.NumberofRoot);
            builder.AppendFormat("Total Sectors: {0}\n",br.totalSectors);
            builder.AppendFormat("Media: {0}\n",br.Media);
            builder.AppendFormat("Sectors Per Track: {0}\n",br.SectorsPerTrack);
            builder.AppendFormat("Number of Head: {0}\n",br.NumberofHead);
            Console.WriteLine(builder.ToString());
        }
    }
}