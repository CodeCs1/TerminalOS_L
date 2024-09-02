using System;
using Cosmos.Core;
using Cosmos.HAL;

namespace TerminalOS_L.Driver.VBox {
    public class VBox {
        private static uint PCI_Addr(PCIDevice dev, uint field) {
            return 0x80000000 | (dev.bus << 16)
            | dev.slot << 11 | dev.function << 8 |
            ((field) & 0xfc);
        }
        public VBox() {
            PCIDevice dev = PCI.GetDevice(
                VendorID.VirtualBox,
                DeviceID.VBoxGuest
            );
            if (!dev.DeviceExists) {
                return;
            }

            IOPort.Write32(0xCF8, PCI_Addr(dev,0x10));
            uint res = IOPort.Read32(0xCFC) & 0xFFFFFFFC;

            GuestInfo info = new()
            {
                header = new()
                {
                    Sz = 32,
                    Version = 0x10001,
                    Request = 50,
                    Reserved = 0,
                    Reserved1 = 0
                },
                version = 0x00010003,
                OSType = 0
            };
            try {
                MemoryBlock bl = new(res,0x100);
                bl[0] = (uint)info;
                IOPort.Write32((int)res, bl[0]);
            } catch (Exception ex) {
                Message.Send_Error(ex.Message);
                return;
            }
        }
    }
}