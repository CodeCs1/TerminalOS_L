using System;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
using TerminalOS_L.Driver.AHCIHeader;


//This driver code harder than IDE.
//Another ReCook version of Cosmos AHCI!

namespace TerminalOS_L.Driver {
    public class AHCI {
        private static PCIDevice pci;
        public static bool IsAHCI() {
            pci = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.SATAController);
            if (pci.Subclass == (byte)SubclassID.SATAController) 
                return true;
            return false;
        }
        public struct AHCI_Dev {
            public uint Bar;
            public HBA_MEM mem;
        }

        INTs.IRQDelegate AHCI_Handler;
        public AHCI() {
            pci.EnableMemory(true);
            StringBuilder build = new();
            for (int i=0;i<5;i++) {
                if (pci.BaseAddressBar[i].BaseAddress != uint.MaxValue) {
                    build.AppendFormat("PCI {0}: {1}\n",i, pci.BaseAddressBar[i].BaseAddress);
                }
            }
            build.AppendFormat("INTLine: {0}", pci.InterruptLine);
            Console.WriteLine(build.ToString());
            pci.Command=PCIDevice.PCICommand.Memory;
            //What am i even doing with my life....
            INTs.SetIrqHandler(pci.InterruptLine, AHCI_Handler);
            
        }

        public void Write8(uint offset, byte data){

	        uint address = (pci.bus << 16) | (pci.slot << 11) |
	                                  (pci.function << 8) | (offset & 0xfc) | (0x80000000);
            IOPort.Write32(0xCF8, address);
        }

        public void Init(HBA_MEM mem) {
            uint pi = mem.PortImpl;
            int i = 0;
	        while (i<32)
	        {
	        	if ((pi & 1) != 0)
	        	{
	        		Console.WriteLine("OK!");
	        	}

	        	pi >>= 1;
	        	i ++;
	        }
        }
    }
}