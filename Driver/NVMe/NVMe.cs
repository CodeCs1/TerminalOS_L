using System;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;

namespace TerminalOS_L.Driver.NVMe {
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
        private void NVMe_Handler(ref INTs.IRQContext aContext) {

        }
        public NVMe() {
            PCIDevice dev = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.NVMController);
            if (dev.ClassCode == (ushort)ClassID.MassStorageController && dev.Subclass == (ushort)SubclassID.NVMController) {
                Message.Send_Log("Found NVMe device.");
                Message.Send_Log($"{dev.bus}:{dev.DeviceID}.{dev.function} Non-Volatile memory controller: {PCIDevice.DeviceClass.GetDeviceString(dev)}");
            } else {
                Message.Send_Error("No NVMe device was found!");
                return;
            }
            dev.EnableDevice();
            dev.EnableBusMaster(true);
            dev.EnableMemory(true);
            BaseAddr = ((ulong)dev.ReadRegister32(0x14) << 32) | (dev.ReadRegister32(0x10) & 0xfffffff0);
            var nvme_cap = (BaseAddr >> 12) &0xf;

            StringBuilder builder =new();
            builder.AppendFormat("NVMBase: {0}\nNVMECap: {1}\n", BaseAddr, nvme_cap);
            builder.AppendFormat("Interrupt Line: {0}\n", dev.InterruptLine);
            Console.WriteLine(builder.ToString());

            INTs.SetIrqHandler(dev.InterruptLine, NVMe_Handler);
        }
    }
}