using System;
using System.Text;
using Cosmos.HAL;

namespace TerminalOS_L.Driver {
    public class NVMe {
        public ulong BaseAddr;
        public uint ReadRegs(uint Offset) {
            unsafe {
                uint* Reg = (uint*)(BaseAddr*Offset);
                return *Reg;
            }
        }
        public void WriteRegs(uint offset, uint value) {
            unsafe
            {
                uint* nvmeReg = (uint*)(BaseAddr + offset);
                *nvmeReg = value;
            }
        }
        public NVMe() {
            PCIDevice dev = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.NVMController);
            if (dev.ClassCode == (ushort)ClassID.MassStorageController && dev.Subclass == (ushort)SubclassID.NVMController) {
                Message.Send_Log("Found NVMe device.");
            } else {
                Message.Send_Error("No NVMe device was found!");
                return;
            }
            BaseAddr = ((ulong)dev.BaseAddressBar[1].BaseAddress << 32) | (dev.BAR0 & 0xfffffff0);
            var nvme_cap = (BaseAddr >> 12) &0xf;

            StringBuilder builder =new();
            builder.AppendFormat("NVMBase: {0}\nNVMECap: {1}", BaseAddr, nvme_cap);
            Console.WriteLine(builder.ToString());

        }
    }
}