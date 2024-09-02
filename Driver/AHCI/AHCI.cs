using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.HAL.BlockDevice.Ports;
using Cosmos.HAL.BlockDevice.Registers;
using TerminalOS_L.FrameBuffer;


//This driver code harder than IDE.
//A ReCook version of Cosmos AHCI!

namespace TerminalOS_L.Driver.AHCI {
    public class AHCI {
        private static PCIDevice pci;
        public static bool IsAHCI() {
            pci = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.SATAController);
            if (pci.Subclass == (byte)SubclassID.SATAController) 
                return true;
            return false;
        }
        public unsafe struct AHCI_Dev {
            public uint Bar;
            public HBA_MEM mem;
        }
        private void AHCI_Handler(ref INTs.IRQContext aContext) {
            FrConsole.WriteLine("This should be work.");
        }
        // 
        private static void SetAHCIRegister(MemoryBlock memblock,int location,uint start_index, uint mem_value) {
            uint SetValue = 1U << location;
            mem_value = SetValue;
            memblock[start_index] = mem_value;
        }
        private static uint GetAHCIRegister(MemoryBlock memblock, uint start_index) {
            return memblock[start_index];
        }
        public AHCI() {
            pci.EnableDevice();
            pci.EnableMemory(true);
            // Built-in AHCI just...crash.
            //Cosmos.HAL.BlockDevice.AHCI _ = new (pci);
            var memblock = new MemoryBlock(pci.ReadRegister32(0x24),0x100);
            AHCI_Dev dev=new() {
                Bar = pci.ReadRegister32(0x24) , // Get BAR5 Register
                mem = new()
            };
            SetAHCIRegister(memblock,31,1,dev.mem.ghc); // Enable AHCI
            uint value = GetAHCIRegister(memblock,0);
            INTs.SetIrqHandler(pci.InterruptLine, AHCI_Handler);
            FrConsole.WriteLine($"Host Capabilities: {Convert.ToString(value)}");
        }
    }
}