using System;
using System.IO;
using System.Text;
using TerminalOS_L.Driver;
using TerminalOS_L.FrameBuffer;

namespace TerminalOS_L.FileSystemR.Microsoft.FAT32
{
    public class FAT32 : VFS {
        public BootRecord br;
        public override uint LBA_Start { get => base.LBA_Start; set => base.LBA_Start = value; }
        public ATA ata;

        public override ATA ATA => ata;
        public override string Type => "EXT2";
        public FAT32(ATA ata) : base(ata) {
            this.ata = ata;
        }
        //http://wiki.osdev.org/User:Requimrar/FAT32#Helper_Functions
        private uint Cluster2LBA(uint cluster) {
            return (uint)(LBA_Start+br.ReservedSectors+(br.NumberofFat*br.SectorsPerFAT)+
            cluster+br.NumberPerCluster-(2*br.NumberPerCluster));
        }

        public override int Impl()
        {
            byte[] BPB = new byte[512];
            ata.Read28((int)LBA_Start,512,ref BPB);
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
            FrConsole.WriteLine("-------File System-------");
            builder.AppendFormat("Bytes Per Sector: {0}\n",br.BytePerSectors);
            builder.AppendFormat("Number of Sectors: {0}\n",br.NumberofSectors);
            builder.AppendFormat("Reserved Sectors: {0}\n",br.ReservedSectors);
            builder.AppendFormat("Number of FAT: {0}\n",br.NumberofFat);
            builder.AppendFormat("Number of Root: {0}\n",br.NumberofRoot);
            builder.AppendFormat("Total Sectors: {0}\n",br.totalSectors);
            builder.AppendFormat("Media: {0}\n",br.Media);
            builder.AppendFormat("Sectors Per Track: {0}\n",br.SectorsPerTrack);
            builder.AppendFormat("Number of Head: {0}\n",br.NumberofHead);
            FrConsole.WriteLine(builder.ToString());

            FrConsole.WriteLine("Getting Cluster 2 [Root]");
            byte[] b = new byte[512];
            FrConsole.WriteLine(Convert.ToString(Cluster2LBA(2)));
            ata.Read28((int)Cluster2LBA(2), 512, ref b);
            Kernel.PrintByteArray(b);
            return 0;
        }
    }
}