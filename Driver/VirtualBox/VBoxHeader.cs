using System;

namespace TerminalOS_L.Driver.VBox {
    public struct Header {
        public uint Sz;
        public uint Version;
        public uint Request;
        public int rc;
        public uint Reserved;
        public uint Reserved1;
    }
    public struct GuestInfo {
        public Header header;
        public uint version;
        public uint OSType;

        public static explicit operator uint(GuestInfo v)
        {
            throw new NotImplementedException();
        }
    }
}