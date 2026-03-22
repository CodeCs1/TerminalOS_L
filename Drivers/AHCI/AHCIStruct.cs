using System.Runtime.InteropServices;

namespace TerminalOS_Lgen3.Drivers.AHCI
{
    public enum SATAPortType
    {
        NONE = 0,
        SATA = 1,
        SEMB = 2,
        PM = 3,
        ATAPI = 4
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct HBA_PORT
    {
        public ulong clb;
        public ulong fb;
        public uint int_stat;
        public uint ie;
        public uint cmd;
        public uint rsv0;
        public uint tfd;
        public uint sig;
        public uint ssts;
        public uint sctl;
        public uint serr;
        public uint sact;
        public uint ci;
        public uint sntf;
        public uint fbs;
        public fixed uint rsv1[11];
        public fixed uint vendor[4];
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct HBA_MEM
    {
        public volatile uint cap;
        public volatile uint ghc;
        public volatile uint int_stat;
        public volatile uint pi;
        public volatile uint vs;
        public volatile uint ccc_ctl;
        public volatile uint ccc_pts;
        public volatile uint em_loc;
        public volatile uint em_ctl;
        public volatile uint cap2;
        public volatile uint bohc;
        public fixed byte rsv[0xa0 - 0x2c];
        public fixed byte vendor[0x100 - 0xa0];

        public HBA_PORT ports;

        public static nint GetPort(ulong ABAR, uint port) {
            return (nint)(ABAR+ (0x100 + 0x80 * port));
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct HBA_CMD_HEADER
    {
        private byte p1;
        private byte p2;
        public ushort prdtl;
        public uint prdbc;
        public ulong ctba;
        public fixed uint reserved[4];

        public byte CommandFISLength
        {
            readonly get
            {
                return (byte)(p1 & 0x1f);
            }
            set
            {
                unchecked { p1 &= (byte)~0x1f; }
                p1 |= (byte)(value & 0x1f);
            }
        }
        public readonly bool ATAPI
        {
            get
            {
                return (p1 & (1<<5)) != 0;
            }
        }
        public bool Write
        {
            readonly get
            {
                return (p1 & (1<<6)) != 0;
            }
            set
            {
                if (value)
                {
                    p1 |= 1<<6;
                }
                else
                {
                    unchecked {p1 &=(byte)~(1<<6);}
                }
            }
        }
        public readonly bool Prefetchable
        {
            get
            {
                return (p1 & (1<<7)) !=0;
            }
        }
        
        public readonly bool Reset
        {
            get
            {
                return (p2 & (1<<0)) !=0;
            }
        }
        
        public readonly bool BIST
        {
            get
            {
                return (p2 & (1<<1)) !=0;
            }
        }
        public bool ClearBusy
        {
            readonly get
            {
                return (p2 & (1<<2)) != 0;
            }
            set
            {
                if (value) p2 |= 1<<2;
                else unchecked {p2 &= (byte)~(1<<2);}
            }
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct HBA_CMD_TBL
    {
        public fixed byte cfis[64];
        public fixed byte acmd[16];
        public fixed byte reserved[48];
        public HBA_PRDT_ENTRY prdt_entry;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HBA_PRDT_ENTRY
    {
        public ulong dba;
        public uint reserved0;

        public uint flags;
        public bool IOC
        {
            readonly get
            {
                return (flags & (1<<31))!=0;
            }
            set
            {
                if (value)
                {
                    flags |= 1U<<31;
                } else
                {
                    unchecked
                    {
                        flags &= ~(1u<<31);
                    }
                }
            }
        }
        public uint ByteCount
        {
            get
            {
                return flags & 0x3fffff;
            }set
            {
                flags &= ~0x3fffffu;
                flags |= value & 0x3fffffu;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FIS_REG_H2D
    {
        public byte fis_type;
        public byte port_multiplier;
        public byte command;
        public byte featurel;
        public byte lba0;
        public byte lba1;
        public byte lba2;
        public byte device;
        public byte lba3;
        public byte lba4;
        public byte lba5;
        public byte featureh;
        public ushort count;
        public byte icc;
        public byte control;

        public fixed byte reserved1[4];
    }
}