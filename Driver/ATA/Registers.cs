using Cosmos.Core;
using XSharp.Assembler.x86.SSE;

namespace TerminalOS_L.Driver {
    public class ATARegisters {
        public static ushort Port;
        public ATARegisters(ushort port) {
            Port  = port;
        }
        public static ushort DataRegister {
                get {
                    return IOPort.Read16(Port);
                }
                set {
                    IOPort.Write16(Port, value);
                }
            }

        public static byte DataRegisterBit8 {
                get {
                    return IOPort.Read8(Port);
                }
                set {
                    IOPort.Write8(Port, value);
                }
        }
        public static byte ErrorRegister {
            get {
                return IOPort.Read8(Port+1);
            }
        }
        public static byte FeaturesRegister {
            set {
                IOPort.Write8(Port+1,value);
            }
        }
        public static byte SectorCountRegister {
            get {
                return IOPort.Read8(Port+2);
            }
            set {
                IOPort.Write8(Port+2, value);
            }
        }
        public static byte LBALow {
            get {
                return IOPort.Read8(Port+3);
            }
            set {
                IOPort.Write8(Port+3, value);
            }
        }
        public static byte LBAMid {
            get {
                return IOPort.Read8(Port+4);
            }
            set {
                IOPort.Write8(Port+4, value);
            }
        }
        public static byte LBAHi {
            get {
                return IOPort.Read8(Port+5);
            }
            set {
                IOPort.Write8(Port+5, value);
            }
        }
        public static byte DriveRegisters {
            get {
                return IOPort.Read8(Port+6);
            }
            set {
                IOPort.Write8(Port+6, value);
            }
        }
        public static byte StatusRegisters {
            get {
                return IOPort.Read8(Port+7);
            }
        }
        public static byte CommandRegisters {
            set {
                IOPort.Write8(Port+7,value);
            }
        }
        public static byte ControlRegisters {
            get {
                return IOPort.Read8(Port+0x0C);
            }
            set {
                IOPort.Write8(Port+0x0c, value);
            }
        }
    }
}