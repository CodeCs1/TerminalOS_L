using System;
using System.Text;
using Cosmos.HAL;
using TerminalOS_L.Driver.AHCIHeader;


//This driver code harder than IDE.
//Another ReCook version of Cosmos AHCI!

namespace TerminalOS_L.Driver {
    public class AHCI {
        private static PCIDevice pci;
        public static bool IsAHCI() {
            pci = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.SATAController);
            if (pci.Subclass == (byte)SubclassID.SATAController) {
                return true;
            } else {
                StringBuilder build = new();
                build.AppendFormat("Get PCI: {0} {1}", pci.Subclass, pci.ClassCode);
                Console.WriteLine(build.ToString());
            }
            return false;
        }
        public struct AHCI_Dev {
            public uint Bar;
            public HBA_MEM mem;
        }
        public AHCI() {
            AHCI_Dev dev = new()
            {
                Bar = pci.BaseAddressBar[5].BaseAddress,
                mem = new()
            };

            pci.EnableMemory(true);
        }
    }
}