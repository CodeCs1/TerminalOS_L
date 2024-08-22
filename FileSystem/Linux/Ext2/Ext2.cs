using System;
using System.IO;
using System.Text;
using TerminalOS_L.Driver;

// Some resource that helping me in making this driver:
// https://wiki.osdev.org/Ext2
// http://www.science.smith.edu/~nhowe/262/oldlabs/ext2.html
// https://www.youtube.com/@cjumpdotcom

namespace TerminalOS_L.FileSystemR.Linux {
    public class Ext2 : VFS {


        public struct DirectoryEntry {
            public uint inode;
            public uint TotalSize;
            public uint NameLength;
            public byte TypeIndicator;
            public byte[] Name;
        }

        public ATA ata;
        public Inode inode;
        public new uint LBA_Start;

        public Ext2(ATA ata,uint initLBA) : base(ata,initLBA) {
            this.ata = ata;
            this.LBA_Start=initLBA;
        }
        public override ATA ATA => ata;
        public override string Type => "EXT2";

        
        private static uint Block2LBA(uint block_no) {
            return block_no*2;
        }

        private SuperBlockEnum spb=new();
        private ExtendedSuperBlock esb=new();

        private BlockGroupDescriptor[] bgd;

        public override int Impl()
        {
            //Read Super Block
            byte[] superblock = new byte[1024];
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
            } else {
                Message.Send_Error("Bad super block signature!");
                return -1;
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
            bgd=new BlockGroupDescriptor[bgdsz];
            byte[] blockdescriptor = new byte[32*bgdsz];
            ata.Read28((int)(LBA_Start+4), 32*bgdsz,ref blockdescriptor);
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

            return 0; 
        }
        public override void List(string Path) {
            Inode Root = GetInodeInfo(2,bgd,esb,spb);
            if (Path == "/") {
                ListDir(Root);
            } else {
                //TODO
                Message.Send_Warning("Still in development!");
            }
        }

        public override string DiskLabel { 
            get {
                if (spb.MajorPortion>=1) {
                    return Encoding.ASCII.GetString(esb.VolumeName);
                } else {
                    return "NoLabel";
                }
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override string ReadFile(string path)
        {
            Inode Root = GetInodeInfo(2,bgd,esb,spb);
            DirectoryEntry[] en = GetDirectoryEntry(Root, esb, spb);
            for (int i=0;i<en.Length;i++) {
                if (Encoding.ASCII.GetString(en[i].Name).Equals(path)) {
                    byte[] buffer = new byte[en[i].TotalSize];
                    Inode Block = GetInodeInfo((int)en[i].inode,bgd,esb,spb);
                    ata.Read28((int)(LBA_Start +Block2LBA(Block.DirectBlockPointer0)), (int)en[i].TotalSize,ref buffer);
                    return Encoding.ASCII.GetString(buffer);
                }
            }
            return "";
        }

        private Inode GetInodeInfo(int index,
            BlockGroupDescriptor[] blockGroup,ExtendedSuperBlock esb,
        SuperBlockEnum spb) {
            if (index == 0) {
                Message.Send_Error("Index 0 is illegal.");
                return new Inode();
            }
            uint count;
            if (spb.MajorPortion>=1) {
                count = esb.SizeOfEachINode;
            } else {
                count = 128;
            }
            int BlockGroup = (int)((index-1)/spb.NumberofInodes);
            uint ItableStart = Block2LBA(blockGroup[BlockGroup].InodeTable);
            uint Length = (uint)(spb.NumberofInodes/((1024<<(int)spb.BlockSize)/count));
            int _index_=0;
            int Split = (int)(512 / count);
            for (int i=1;i<=Length;i++) {
                if (i % Split == 0) {
                    if (i == index) {
                        break;
                    }
                    _index_++;
                }
            }
            byte[] buffer = new byte[512];
            ata.Read28((int)(LBA_Start+ItableStart+_index_), 512,ref buffer);

            using var diren = new BinaryReader(new MemoryStream(buffer));
            if (index%2 == 0) {
                diren.BaseStream.Seek(count,SeekOrigin.Current);
            }
            Inode inode = new()
            {
                TypeAndPermissions = diren.ReadUInt16(),
                UserID=diren.ReadUInt16(),
                SizeinBytes=diren.ReadUInt32(),
                LastAccessTime=diren.ReadUInt32(),
                LastModificationTime=diren.ReadUInt32(),
                CreationTime=diren.ReadUInt32(),
                DeletionTime=diren.ReadUInt32(),
                GroupID=diren.ReadUInt16(),
                CountofHardLinks=diren.ReadUInt16(),
                CountofDiskSectors=diren.ReadUInt32(),
                Flags=diren.ReadUInt32(),
                OS_Value1=diren.ReadUInt32(),
                DirectBlockPointer0=diren.ReadUInt32(),
                DirectBlockPointer1=diren.ReadUInt32(),
                DirectBlockPointer2=diren.ReadUInt32(),
                DirectBlockPointer3=diren.ReadUInt32(),
                DirectBlockPointer4=diren.ReadUInt32(),
                DirectBlockPointer5=diren.ReadUInt32(),
                DirectBlockPointer6=diren.ReadUInt32(),
                DirectBlockPointer7=diren.ReadUInt32(),
                DirectBlockPointer8=diren.ReadUInt32(),
                DirectBlockPointer9=diren.ReadUInt32(),
                DirectBlockPointer10=diren.ReadUInt32(),
                DirectBlockPointer11=diren.ReadUInt32(),
                SinglyIndirectBlockPointer=diren.ReadUInt32(),
                DoublyIndirectBlockPointer=diren.ReadUInt32(),
                TriplyIndirectBlockPointer=diren.ReadUInt32(),
                GenerationNumber=diren.ReadUInt32(),
                ExtendedAttributeBlock=diren.ReadUInt32(),
                UpperFileSize=diren.ReadUInt32(),
                BlockAddr=diren.ReadUInt32(),
                OS_Value2=diren.ReadBytes(12)
            };

            return inode;
        }
        private DirectoryEntry[] GetDirectoryEntry(Inode inode,ExtendedSuperBlock esb,
        SuperBlockEnum spb) {
            uint count;
            if (spb.MajorPortion>=1) {
                count = esb.SizeOfEachINode;
            } else {
                count = 128;
            }
            byte[] buffer = new byte[count];
            ata.Read28((int)(LBA_Start + Block2LBA(inode.DirectBlockPointer0)), (int)count,ref buffer);
            using var diren_root = new BinaryReader(new MemoryStream(buffer));
            int count_dir = 0;

            DirectoryEntry[] dir=new DirectoryEntry[256]; // TODO: Fix the limit of directory
            while(count_dir<256) {
                dir[count_dir].inode = diren_root.ReadUInt32();
                if (Convert.ToInt32(dir[count_dir].inode) == 0) { // Inode 0 is illegal.
                    break;
                }
                dir[count_dir].TotalSize=diren_root.ReadUInt16();
                dir[count_dir].NameLength=diren_root.ReadByte();
                dir[count_dir].TypeIndicator = diren_root.ReadByte();
                dir[count_dir].Name =diren_root.ReadBytes((int)dir[count_dir].NameLength);
                if (dir[count_dir].NameLength < 4) {
                    diren_root.BaseStream.Seek(4 - dir[count_dir].NameLength,SeekOrigin.Current);
                } else if (dir[count_dir].NameLength > 4) {
                    int remainder = (int)(dir[count_dir].NameLength % 4);
                    if (remainder != 0) {
                        int seektime = 4 - remainder;
                        diren_root.BaseStream.Seek(seektime,SeekOrigin.Current);
                    }
                }

                if (Convert.ToInt32(dir[count_dir].inode) == 0) {
                    break;
                }

                count_dir++;
            }

            return dir;
        }

        private void ListDir(Inode inode) {
            byte[] buffer = new byte[1024];
            ata.Read28((int)(LBA_Start + Block2LBA(inode.DirectBlockPointer0)), (int)1024,ref buffer);
            using var diren_root = new BinaryReader(new MemoryStream(buffer));
            DirectoryEntry[] root=new DirectoryEntry[256]; // TODO: Fix the limit of directory
            int count_dir = 0;
            while(count_dir<256) {
                root[count_dir].inode = diren_root.ReadUInt32();

                root[count_dir].TotalSize=diren_root.ReadUInt16();
                root[count_dir].NameLength=diren_root.ReadByte();
                root[count_dir].TypeIndicator = diren_root.ReadByte();
                root[count_dir].Name =diren_root.ReadBytes((int)root[count_dir].NameLength);

                //ye
                // https://stackoverflow.com/questions/7648911/are-ext2-directory-entry-names-guaranteed-to-be-null-terminated-on-a-valid-file
                if (root[count_dir].NameLength < 4) {
                    diren_root.BaseStream.Seek(4 - root[count_dir].NameLength,SeekOrigin.Current);
                } else if (root[count_dir].NameLength > 4) {
                    int remainder = (int)(root[count_dir].NameLength % 4);
                    if (remainder != 0) {
                        int seektime = 4 - remainder;
                        diren_root.BaseStream.Seek(seektime,SeekOrigin.Current);
                    }
                }

                if (Convert.ToInt32(root[count_dir].inode) == 0) {
                    break;
                }
                count_dir++;
            }

            for (int i=0;i<count_dir;i++) 
            {
                var builder = new StringBuilder();
                switch(root[i].TypeIndicator) {
                    case 1:
                        builder.AppendFormat("{0}    {1}",
                        Encoding.ASCII.GetString(root[i].Name), 
                        root[i].TotalSize);
                        break;
                    case 2:
                        builder.AppendFormat("{0}    <DIR>",Encoding.ASCII.GetString(root[i].Name));
                        break;
                }
                Console.WriteLine(builder.ToString());
            }
        }
    }
}