using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.HAL.BlockDevice.Ports;
using Cosmos.HAL.BlockDevice.Registers;
using TerminalOS_L.Driver.AHCIHeader;


//This driver code harder than IDE.
//A Non-ReCook version of Cosmos AHCI!

namespace TerminalOS_L.Driver {
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
            public HBA_MEM* mem;
        }
        private void AHCI_Handler(ref INTs.IRQContext aContext) {
            Console.WriteLine("This should be work.");
        }
        public AHCI() {
            pci.EnableDevice();
            pci.EnableMemory(true);
            // Built-in AHCI just...crash.
            //Cosmos.HAL.BlockDevice.AHCI _ = new (pci);
            AHCI_Dev dev=new() {
                Bar = pci.ReadRegister32(0x24), // Get BAR5 Register
            };
            //I hate my life.
            unsafe {
                dev.mem = (HBA_MEM*)dev.Bar;
            }
            StringBuilder b=new();
            Console.WriteLine(b.ToString());
            INTs.SetIrqHandler(pci.InterruptLine, AHCI_Handler);
        }
    }
}