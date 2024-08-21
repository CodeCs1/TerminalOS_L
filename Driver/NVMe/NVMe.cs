using System;
using Cosmos.HAL;

namespace TerminalOS_L.Driver {
    public class NVMe {
        public NVMe() {
            PCIDevice dev = PCI.GetDeviceClass(ClassID.BridgeDevice,SubclassID.NVMController);
            if (dev.ClassCode == (ushort)ClassID.BridgeDevice && dev.Subclass == (ushort)SubclassID.NVMController) {
                Message.Send_Log("Found NVMe device.");
            } else {
                Message.Send_Error("No NVMe device was found!");
                return;
            }
            ulong nvme_base = (UInt64)(((UInt64)(dev.BAR0+1)<<32) | (dev.BAR0 & 0xfffffff0));
            
        }
    }
}