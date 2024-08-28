using System.Runtime.InteropServices;

//Send help...

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
        public byte Command;
        public byte FeaturesL;
        public byte LBA0;
        public byte LBA1;
        public byte LBA2;
        public byte Devices;
        public byte LBA3;
        public byte LBA4;
        public byte LBA5;
        public byte FeaturesH;
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

        public byte Status;
        public byte Error;
        public byte LBA0;
        public byte LBA1;
        public byte LBA2;
        public byte Device;
        public byte LBA3;
        public byte LBA4;
        public byte LBA5;
        public byte Reserved;
        public byte CountL;
        public byte CountH;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =2)]
        public byte[] Reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public byte[] Reserved3;
    }
    public struct FIS_DATA {
        public byte Type;
        public struct Field1 {
            [BitField(4)]
            public byte PMPort;
        }
        public struct Field2 { [BitField(4)] public byte Reserved0; }
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =2)]
        public byte Reserved1;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =1)]
        public uint[] Data;
    }
    public struct FIS_PIO_SETUP {
        public byte Type;
        public struct Field1 { [BitField(4)] public byte PMPort; }
        public struct Field2 { [BitField(1)] public byte Reserved0; }

        public struct FIeld3 { [BitField(1)] public byte Data; }
        public struct FIeld4 { [BitField(1)] public byte Interrupt; }
        public struct Field5 { [BitField(1)] public byte Reserved; }
        public byte Status;
        public byte Error;
        public byte LBA0;
        public byte LBA1;
        public byte LBA2;
        public byte Device;
        public byte LBA3;
        public byte LBA4;
        public byte LBA5;
        public byte Reserved;
        public byte CountL;
        public byte CountH;
        public byte Reserved2;
        public byte N_Status;
        public short TransferCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst= 2)]
        public byte[] Reserved3;
    }
    public struct FIS_DMA_SETUP {
public byte Type;
        public struct Field1 { [BitField(4)] public byte PMPort; }
        public struct Field2 { [BitField(1)] public byte Reserved0; }

        public struct FIeld3 { [BitField(1)] public byte Data; }
        public struct FIeld4 { [BitField(1)] public byte Interrupt; }
        public struct Field5 { [BitField(1)] public byte Reserved; }
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =2)]
        public byte[] Reserved;
        public ulong DMABufferID;
        public uint Reserved2;
        public uint DMABufferOffset;
        public uint TransferCount;
        public uint Reserved3;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HBA_MEM {
        public uint cap;
        public uint ghc;
        public uint InterruptStatus;
        public uint PortImpl;
        public uint Version;
        public uint CommandComplCC;
        public uint CommandComplCP;
        public uint EnclosurseLocation;
        public uint Cap2;
        public uint Bohc;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =0xA0-0x2C)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =0x100-0xA0)]
        public byte[] Vendor;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =1)]
        public HBA_PORT[] port;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HBA_PORT {
        public uint CLB;
        public uint CLBU;
        public uint FB;
        public uint FBU;
        public uint InterruptStatus;
        public uint InterruptEnable;
        public uint Command;
        public uint Reserved;
        public uint TFD;
        public uint Signature;
        public uint SATAStatus;
        public uint SATAControl;
        public uint SATAError;
        public uint SATAActive;
        public uint CommandIssue;
        public uint SATANotification;
        public uint FBS;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =11)]
        public uint[] Reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public uint[] Vendor;
    }

}