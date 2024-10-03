using System;
using Cosmos.Core;
using Cosmos.HAL;
using TerminalOS_L.FrameBuffer;

// Some Cooked version of IHDA (Intel High Definiton Audio)

namespace TerminalOS_L.Driver.IHDA {
    public class IHDA {
        private static PCIDevice dev_;
        public static bool IsIHDA() {
            PCIDevice dev=PCI.GetDevice(VendorID.Intel,(DeviceID)0x2668);
            if (dev.VendorID!=(ushort)VendorID.Intel&&
            dev.DeviceID != 0x2668) {
                dev=PCI.GetDevice(VendorID.Intel,(DeviceID)0x27D8);
                if (dev.VendorID!=(ushort)VendorID.Intel&&
                dev.DeviceID != 0x2668) {
                    return false;
                }
                dev_ = dev;
                return true;
            }
            dev_ = dev;
            return true;
        }
        private static bool Is64Support;

        public IHDA() {
            var bl=new MemoryBlock(dev_.BAR0,0x100);
            var Cap=bl[0];
            Is64Support = BitPort.GetBit(Cap,0)!=0;
            FrConsole.WriteLine($"Number of Bidirectional streams: {Convert.ToString((Cap >> 3) & 0x0F)}");
            FrConsole.WriteLine($"Number of Input streams: {Convert.ToString((Cap >> 8) & 0x0E)}");
            FrConsole.WriteLine($"Number of Output streams: {Convert.ToString((Cap >> 13) & 0x0E)}");
        }
    }
}