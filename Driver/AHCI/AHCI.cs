using System;
using Cosmos.Core;
using Cosmos.HAL;
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
        private static int CheckType(HBA_PORT port) {
            uint SATAStatus = port.SATAStatus;
            byte ipm = (byte)((SATAStatus >> 8) & 0x0F);
            byte det = (byte)(SATAStatus & 0x0F);

            if (det != 3) {
                return 0;
            }
            if (ipm != 1) {
                return 0;
            }

            return port.Signature switch
            {
                //SATA
                0x00000101 => 1,
                // SATAPI
                0xEB140101 => 2,
                // Enclosure MB
                0xC33C0101 => 3,
                // PM
                0x96690101 => 4,
                _ => -1,// Unknown
            };
        }

        private static MemoryBlock PortBlock;

        private static void WritePort(ref HBA_PORT port, int PortImpl) {
            uint Offset = (uint)(pci.ReadRegister32(0x24)+(0x100 + PortImpl * 0x80));
            PortBlock = new(Offset, 0x80);
            port = new() {
                CLB = PortBlock[0],
                CLBU = PortBlock[0x04],
                FB = PortBlock[0x08],
                FBU = PortBlock[0x0C],
                InterruptStatus = PortBlock[0x10],
                InterruptEnable = PortBlock[0x14],
                Command = PortBlock[0x18],
                Reserved = PortBlock[0x1C],
                TFD = PortBlock[0x20],
                Signature = PortBlock[0x24],
                SATAStatus = PortBlock[0x28],
                SATAControl = PortBlock[0x2C],
                SATAError = PortBlock[0x30],
                SATAActive = PortBlock[0x34],
                CommandIssue = PortBlock[0x38],
                SATANotification = PortBlock[0x3c],
                FBS=PortBlock[0x40],
                
            };
        }

        private static void ProbePort(HBA_MEM abar) {
            uint pi = abar.PortImpl;
            int i=0;
            while(i < 32) {
                if ((pi & 1) !=0) {
                    WritePort(ref abar.port[i], i);
                    int dt = CheckType(abar.port[i]);
                    switch(dt) {
                        case 1:
                            FrConsole.WriteLine($"SATA Found at port: {Convert.ToString(i)}");
                            break;
                        case 2:
                            FrConsole.WriteLine($"SATAPI Found at port: {Convert.ToString(i)}");
                            break;
                        case 3:
                            FrConsole.WriteLine($"EMB Found at port: {Convert.ToString(i)}");
                            break;
                        case 4:
                            FrConsole.WriteLine($"PM Found at port: {Convert.ToString(i)}");
                            break;
                        default:
                            FrConsole.WriteLine($"No Drive found at port: {Convert.ToString(i)}");
                            break;
                    }
                    
                }

                pi >>=1;
                i++;
            }
        }

        private void StartCommand(HBA_PORT port) {
            while((port.Command & 0x8000) != 0) {
                FrConsole.Write(".");
                port.Command = PortBlock[0x18];
            }
            port.Command |= 0x0010;
            PortBlock[0x18] = port.Command;

            port.Command |= 0x0001;
            PortBlock[0x18] = port.Command;
        }

        private static int findCmd(HBA_PORT port) {
            uint slots = port.SATAActive | port.CommandIssue;
            int cmdsl = (int)((dev.mem.cap & 0x0f00) >> 8);
            for (int i=0;i<cmdsl;i++) {
                if ((slots & 1) == 0) {
                    return i;
                }
                slots >>=1;
            }
            FrConsole.WriteLine("Not OK");
            return -1;
        }

        private static AHCI_Dev dev;

        public AHCI() {
            pci.EnableDevice();
            pci.EnableMemory(true);
            // Built-in AHCI just...crash.
            //Cosmos.HAL.BlockDevice.AHCI _ = new (pci);
            var memblock = new MemoryBlock(pci.ReadRegister32(0x24),0x100);
            dev=new() {
                Bar = pci.ReadRegister32(0x24) , // Get BAR5 Register
                mem = new() {
                    port = new HBA_PORT[31],
                }
            };
            SetAHCIRegister(memblock,31,1,dev.mem.ghc); // Enable AHCI
            dev.mem.cap = GetAHCIRegister(memblock,0);
            INTs.SetIrqHandler(pci.InterruptLine, AHCI_Handler);
            FrConsole.WriteLine($"Host Capabilities: {Convert.ToString(dev.mem.cap)}");
            dev.mem.PortImpl = GetAHCIRegister(memblock,0x0C);
            FrConsole.WriteLine($"Port Implemented: {Convert.ToString(dev.mem.PortImpl)}");
            ProbePort(dev.mem);
            FrConsole.WriteLine($"Port 0 CLB: {dev.mem.port[0].CLB}");
            findCmd(dev.mem.port[0]);
            StartCommand(dev.mem.port[0]);
        }
    }
}