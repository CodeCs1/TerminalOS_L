using System;
using System.IO;
using System.Text;
namespace TerminalOS_L {
    public class BinaryReaderBigEndian : BinaryReader
    {
        public BinaryReaderBigEndian(Stream input) : base(input)
        {
        }
        public override Stream BaseStream => base.BaseStream;
        public override uint ReadUInt32()
        {
            var data = base.ReadBytes(4);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(data);
            }
            return BitConverter.ToUInt32(data,0);
        }
        public override ushort ReadUInt16()
        {
            var data = base.ReadBytes(2);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(data);
            }
            return BitConverter.ToUInt16(data,0);
        }
        public override byte ReadByte()
        {
            return base.ReadByte();
        }
        public override byte[] ReadBytes(int count)
        {
            return base.ReadBytes(count);
        }
    }
}