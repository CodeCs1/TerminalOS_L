using System.Runtime.InteropServices;

namespace TerminalOS_L.Driver.AHCIHeader {
    public enum FIS_TYPE {
        FIS_TYPE_REG_H2D = 0x27,
        FIS_TYPE_REG_D2H = 0x34,
        FIS_TYPE_DMA_ACT = 0x39,
        FIS_TYPE_DMA_SETUP = 0x41,
        FIS_TYPE_DATA = 0x46,
        FIS_TYPE_BIST = 0x58,
        FIS_TYPE_PIO_SETUP = 0x5f,
        FIS_TYPE_DEV_BITS = 0xA1
    }
    public struct FIS_REG_H2D {
        public byte fis_type;
        //bit 4 -> PortMultiplier
        //bit 3 -> Reserved;
        //bit 1 -> Command
        public struct Field1 {
            [BitField(4)]
            public byte Multiplier;
        }
        public struct Field2 {
            [BitField(3)]
            public byte Reserved0;
        }
        public struct Field3 {
            [BitField(1)]
            public byte Command;
        }
        public byte LBA0;
        public byte LBA1;
        public byte LBA2;
        public byte Devices;
        public byte LBA3;
        public byte LBA4;
        public byte LBA5;
        public byte FeaturesR;
        public byte CountL;
        public byte CountH;
        public byte ICC;
        public byte Control;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =4)]
        public byte[] Reserved;
    }

    public struct FIS_REG_D2H {
        public byte Type;
        // bit 4 -> Port Multiplier
        // bit 2 -> Reserved;
        // bit 1 -> Interrupt Bit
        // bit 1 -> Reserved;
        public struct Field1 { [BitField(4)] public byte PMPort; }
        public struct Field2 { [BitField(2)] public byte Reserved0; }
        public struct FIeld3 { [BitField(1)] public byte Interrupt; }
        public struct Field4 { [BitField(1)] public byte Reserved; }

    }
}