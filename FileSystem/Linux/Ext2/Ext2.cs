using System;
using System.IO;
using System.Text;
using TerminalOS_L.Driver;

// Some resource that helpping me in making this driver:
// https://wiki.osdev.org/Ext2

namespace TerminalOS_L.FileSystemR.Linux {
    public class Ext2 : VFS {
        public ATA ata;
        public Inode inode;
        public new uint LBA_Start;
        public Ext2(ATA ata,uint initLBA) : base(ata,initLBA) {
            this.ata = ata;
            this.LBA_Start=initLBA;
        }
        public override ATA ATA => ata;        
        
        public int Offset(int block,int size) {
            return 1024+(block-1)*size;
        }
        private static uint Block2LBA(uint block_no) {
            return block_no*2;
        }

        public override void Impl()
        {
            //Read Super Block
            byte[] superblock = new byte[1024];
            SuperBlockEnum spb=new();
            ExtendedSuperBlock esb=new();
            ata.Read28((int)(LBA_Start + 2), 1024,ref superblock);
            var r = new BinaryReader(new MemoryStream(superblock));
            spb.TotalInodes=r.ReadUInt32();
            spb.TotalBlock=r.ReadUInt32();
            spb.ReservedBlock=r.ReadUInt32();
            spb.TotalUnallocatedBlocks=r.ReadUInt32();
            spb.TotalUnallocatedInodes=r.ReadUInt32();
            spb.SuperBlockLocated=r.ReadUInt32();
            spb.BlockSize=r.ReadUInt32();
            spb.FragmentSize=r.ReadUInt32();
            spb.NumberOfBlock=r.ReadUInt32();
            spb.NumberofFragments=r.ReadUInt32();
            spb.NumberofInodes=r.ReadUInt32();
            spb.LastMountTime=r.ReadUInt32();
            spb.LastWrittenTime=r.ReadUInt32();
            spb.NumberoftimeMounted=r.ReadUInt16();
            spb.NUmberofMountAllowed=r.ReadUInt16();
            spb.Signature=r.ReadUInt16();
            spb.FSState=r.ReadUInt16();
            spb.Error=r.ReadUInt16();
            spb.MinorPortion=r.ReadUInt16();
            spb.TimeOfLastCheck=r.ReadUInt32();
            spb.Interval=r.ReadUInt32();
            spb.OS_ID=r.ReadUInt32();
            spb.MajorPortion=r.ReadUInt32();
            spb.UserID=r.ReadUInt16();
            spb.GroupID=r.ReadUInt16();
            if (spb.Signature == 0xef53) {
                Message.Send("EXT File System detected.");
            }
            var builder = new StringBuilder();
            builder.AppendFormat("Total Inode: {0}\n",spb.TotalInodes);
            builder.AppendFormat("Total Block: {0}\n", spb.TotalBlock);
            builder.AppendFormat("Free block: {0}\n", spb.TotalUnallocatedBlocks);
            builder.AppendFormat("Free Inode: {0}\n", spb.TotalUnallocatedInodes);
            builder.AppendFormat("Block Size: {0}\n", 1024 << (int)spb.BlockSize);
            builder.AppendFormat("Number of Block: {0}\n", spb.NumberOfBlock);
            builder.AppendFormat("Number of INode: {0}\n", spb.NumberofInodes);
            builder.AppendFormat("Minor Portion: {0}\n", spb.MinorPortion);
            builder.AppendFormat("Major Protion: {0}\n", spb.MajorPortion);
            if (spb.MajorPortion >=1) {
                esb.FirstInode = r.ReadUInt32();
                esb.SizeOfEachINode = r.ReadUInt16();
                esb.BlockGroup = r.ReadUInt16();
                esb.OptionalFeatures = r.ReadUInt32();
                esb.RequiredFeatures = r.ReadUInt32();
                esb.FeaturesNotSupport = r.ReadUInt32();
                esb.FsID = r.ReadBytes(16);
                esb.VolumeName = r.ReadBytes(16);
                esb.PathVolumeName = r.ReadBytes(64);
                esb.CompressionAlgotithms = r.ReadUInt32();
                esb.NumberofBlock_File = r.ReadByte();
                esb.NumberOfBlock_Dir = r.ReadByte();
                esb.Reserved = r.ReadUInt16();
                esb.JournalID = r.ReadBytes(16);
                esb.Journal_Inode = r.ReadUInt32();
                esb.Journal_Device = r.ReadUInt32();
                esb.HeadofOrphan =r.ReadUInt32();
                builder.AppendFormat("First Inode: {0}\n", esb.FirstInode);
                builder.AppendFormat("Inode Size: {0}\n", esb.SizeOfEachINode);
                builder.AppendFormat("Volume Name: {0}\n", Encoding.ASCII.GetString(esb.VolumeName));
                builder.AppendFormat("Last mount Path: {0}\n", Encoding.ASCII.GetString(esb.PathVolumeName));
            }

            // Size of BGD:
            int bgdsz = (int)(spb.TotalInodes/spb.NumberofInodes);
            builder.AppendFormat("BGD Size: {0}\n", bgdsz);
            Console.WriteLine(builder.ToString());
            BlockGroupDescriptor[] bgd=new BlockGroupDescriptor[bgdsz];
            byte[] blockdescriptor = new byte[32*bgdsz];
            ata.Read28((int)(LBA_Start+4), 32*bgdsz,ref blockdescriptor);
            Kernel.PrintByteArray(blockdescriptor);
            Console.ReadLine();
            r = new BinaryReader(new MemoryStream(blockdescriptor));
            for (int i=0;i<bgdsz;i++) {
                bgd[i].BlockBitmap=r.ReadUInt32();
                bgd[i].INodeBitMap=r.ReadUInt32();
                bgd[i].InodeTable=r.ReadUInt32();
                bgd[i].FreeBlockCount=r.ReadUInt16();
                bgd[i].FreeInodeCount=r.ReadUInt16();
                bgd[i].UsedDirCount=r.ReadUInt16();
                bgd[i].Flags = r.ReadUInt16();
                bgd[i].ExcludeBitmap = r.ReadUInt32();
                bgd[i].BlockBitmap = r.ReadUInt16();
                bgd[i].INodeBitMap = r.ReadUInt16();
                bgd[i].ItableFree = r.ReadUInt16();
                bgd[i].checksum = r.ReadUInt16();
                var build2 = new StringBuilder();
                build2.AppendFormat("BlockBitMap: {0}\n",    bgd[i].BlockBitmap);
                build2.AppendFormat("InodeBitmap: {0}\n",    bgd[i].INodeBitMap);
                build2.AppendFormat("Inode Table: {0}\n",    bgd[i].InodeTable);
                build2.AppendFormat("FreeBlockCount: {0}\n", bgd[i].FreeBlockCount);
                build2.AppendFormat("FreeInodeCount: {0}\n", bgd[i].FreeInodeCount);
                build2.AppendFormat("UsedDirCount: {0}\n",   bgd[i].UsedDirCount);
                Console.WriteLine(build2.ToString());
            }
            StartIndexTable(bgd[0],esb,spb);
        }




        private void StartIndexTable(BlockGroupDescriptor blockGroup,ExtendedSuperBlock esb,
        SuperBlockEnum spb) {
            uint InodeTable = Block2LBA(blockGroup.InodeTable);
            uint count;
            if (spb.MajorPortion>=1) {
                count = esb.SizeOfEachINode;
            } else {
                count = 128;
            }
            uint TotalInodesTable = (uint)(spb.TotalInodes*count/(1024<<(int)spb.BlockSize));
            //Console.WriteLine($"Total Inode Table: {TotalInodesTable}");
            byte[] inode = new byte[TotalInodesTable];
            ata.Read28((int)((int)LBA_Start + InodeTable), (int)TotalInodesTable,ref inode);
            var INodeRead = new BinaryReader(new MemoryStream(inode));
            Inode[] Inodes = new Inode[TotalInodesTable];
            for (int i=0;i<3;i++) {
                Inodes[i].TypeAndPermissions = INodeRead.ReadUInt16();
                Inodes[i].UserID = INodeRead.ReadUInt16();
                Inodes[i].SizeinBytes = INodeRead.ReadUInt32();
                Inodes[i].LastAccessTime = INodeRead.ReadUInt32();
                Inodes[i].CreationTime = INodeRead.ReadUInt32();
                Inodes[i].LastModificationTime = INodeRead.ReadUInt32();
                Inodes[i].DeletionTime = INodeRead.ReadUInt32();
                Inodes[i].GroupID = INodeRead.ReadUInt16();
                Inodes[i].CountofHardLinks = INodeRead.ReadUInt16();
                Inodes[i].CountofDiskSectors = INodeRead.ReadUInt32();
                Inodes[i].Flags = INodeRead.ReadUInt32();
                Inodes[i].OS_Value1 = INodeRead.ReadUInt32();
                for (int j=0;j<12;j++) {
                    Inodes[i].DirectBlockPointer[j] = INodeRead.ReadUInt32();
                }
                Inodes[i].SinglyIndirectBlockPointer = INodeRead.ReadUInt32();
                Inodes[i].DoublyIndirectBlockPointer = INodeRead.ReadUInt32();
                Inodes[i].TriplyIndirectBlockPointer = INodeRead.ReadUInt32();
                Inodes[i].GenerationNumber = INodeRead.ReadUInt32();
                Inodes[i].ExtendedAttributeBlock = INodeRead.ReadUInt32();
                Inodes[i].UpperFileSize= INodeRead.ReadUInt32();
                Inodes[i].BlockAddr = INodeRead.ReadUInt32();
                Inodes[i].OS_Value2 = INodeRead.ReadUInt32();
                /*var build = new StringBuilder();
                build.AppendFormat("Type: {0}\n", Inodes[i].TypeAndPermissions);
                build.AppendFormat("UserID: {0}\n", Inodes[i].UserID);
                build.AppendFormat("Size: {0}\n", Inodes[i].SizeinBytes);
                Console.WriteLine(build.ToString());
                Console.ReadLine();
                */
            }
        }
    }
}