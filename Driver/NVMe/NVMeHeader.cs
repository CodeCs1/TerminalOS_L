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
    // For life become easier
    public struct BAR_NVMe {
        public ulong ControllerCap;
        public uint Version;
        public uint InterruptMaskset;
        public uint InterruptMaskClear;
        public uint ControllerConfig;
        public uint ControllerStatus;
        public uint AdminQueueAttributes;
        public uint AdminSubmissionQueue;

        public uint AdminCompletionQueue;
    }
    public struct SubmissionEntry {
        public uint Command;
        public uint NSID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =2)]
        public uint[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst =2)]
        public uint[] MetaPointer;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public uint[] DataPointer;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst =6)]
        public uint[] CommandSpecific;

    }
}