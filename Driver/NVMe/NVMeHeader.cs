using System;
using System.Runtime.InteropServices;

namespace TerminalOS_L.Driver.NVMe {
    public struct Queue {
        public ulong Address;
        public ulong Size;
    }
    public struct CompletetionQueue {
        public uint Command;
        public uint Reserved;
        public ushort SubmissionHeadPointer;
        public ushort SubmissionIdentifier;
        public ushort CommandIdentifier;
        public byte PhaseBit;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =15)]
        public byte[] Status;
    }
}