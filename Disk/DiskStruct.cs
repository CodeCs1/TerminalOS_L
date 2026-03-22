using System.Buffers.Binary;
using System.Text;

namespace TerminalOS_Lgen3.Disk
{
    public unsafe struct IdentifyDevice
    {
        public ushort GeneralConfig;
        public ushort NumSylinders;
        public ushort SpecificConfiguration;
        public ushort NumHeads;
        public fixed ushort Retired[2];
        public ushort NumSPT;
        public fixed ushort VenderUnique1[3];
        public fixed byte SerialNumber[20];
        public fixed ushort Retired2[2];
        public ushort Obsolete1;
        public fixed byte FirmwareRevision[8];
        public fixed byte ModelNumber[40];
        public byte MaximumBlockTransfer;
        public byte VenderUnique2;

        public readonly string GetModelName {
            get
            {
                fixed(byte* c = ModelNumber) {
                    Span<byte> bs = new(c,40);

                    for (int i=0;i<40;i+=2)
                    {
                        (bs[i+1], bs[i]) = (bs[i], bs[i+1]);
                    }


                    return Encoding.UTF8.GetString(bs);
                }
            }
        }
    }
}