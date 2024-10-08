using System;
using System.Collections.Generic;
using System.Drawing;
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
            public string name;
            public bool Is64Support;
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
                block = PortBlock,
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
                            port_impl.Add(i);
                            break;
                        case 2:
                            FrConsole.WriteLine($"SATAPI Found at port: {Convert.ToString(i)}");
                            port_impl.Add(i);
                            break;
                        case 3:
                            FrConsole.WriteLine($"EMB Found at port: {Convert.ToString(i)}");
                            port_impl.Add(i);
                            break;
                        case 4:
                            FrConsole.WriteLine($"PM Found at port: {Convert.ToString(i)}");
                            port_impl.Add(i);
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

        private static void StartCommand(HBA_PORT port) {
            while((port.Command & 0x8000) != 0) {
                port.Command = PortBlock[0x18];
            }
            port.Command |= 0x0010;
            PortBlock[0x18] = port.Command;

            port.Command |= 0x0001;
            PortBlock[0x18] = port.Command;
        }

        private static void StopCommand(HBA_PORT port) {
            port.Command &= ~(uint)0x0001;
            port.Command &= ~(uint)0x0010;
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

        private static void GetVersion(ref AHCI_Dev dev) {
            uint minor = dev.mem.Version & 0x0FFFF;
            uint major = dev.mem.Version >> 16;
            minor = (minor >> 8) | ((minor & 0x0F) << 4);
            dev.name = $"AHCI version: {Convert.ToString(major)}.{Convert.ToString(minor)}";
        }

        private static void ReadCommandList(int portno, AHCI_Dev dev) {
            if (dev.mem.port[portno].CLB == 0) {
                FrConsole.ForegroundColor = Color.OrangeRed;
                FrConsole.WriteLine("The AHCI port you selected is not allocated!");
                FrConsole.ResetColor();
                return;
            }
            ulong addr = (ulong)dev.mem.port[portno].CLBU << 32 | dev.mem.port[portno].CLB;
            FrConsole.WriteLine($"Start: {Convert.ToString(dev.mem.port[portno].Command & (1 << 0))}");
            MemoryBlock clbbl = new((uint)addr,0x21);
            FrConsole.WriteLine($"DW0 of Port {Convert.ToString(portno)}: {Convert.ToString(clbbl[0])}");
            uint dw0 = clbbl[0];
            dev.mem.port[portno].header = new()
            {
                CommandFISLength = (byte)(dw0 & 0xF),
                prdtl = (ushort)(dw0 >> 16),
                ATAPI = (byte)(dw0 & (1 << 5)),
                Write = (byte)(dw0 & (1 << 6)),
                Prefetch = (byte)(dw0 & (1 << 7)),
                Reset = (byte)(dw0 & (1 << 8)),
                BIST = (byte)(dw0 & (1 << 9)),
                ClearBusy = (byte)(dw0 & (1 << 10)),
                PortMultiplier = (byte)((dw0 >> 12) & 0xF)
            };

            HBA_CMD_HEADER header = dev.mem.port[portno].header;

            bool IsATAPI = header.ATAPI!=0;
            if (header.CommandFISLength == 0 || header.CommandFISLength == 1) {
                FrConsole.ForegroundColor = Color.OrangeRed;
                FrConsole.WriteLine("Illegal Command List Length!");
                FrConsole.ResetColor();
                return;
            }
            /*FrConsole.WriteLine($"Length of Command List: {Convert.ToString(header.CommandFISLength)}");
            FrConsole.WriteLine($"Physical Region Descriptor Table Length: {Convert.ToString(header.prdtl)}");*/
            FrConsole.WriteLine($"Is ATAPI device ?: {Convert.ToString(IsATAPI)}");
            FrConsole.WriteLine($"{Convert.ToString(clbbl[0x02])}");
            ulong addr2;
            if (dev.Is64Support)
                addr2 = (ulong)clbbl[0x03] << 32 | clbbl[0x02];
            else
                addr2 = clbbl[0x02];

            FrConsole.WriteLine($"CTB Addr: {Convert.ToString(clbbl[0x02])} (Original: {Convert.ToString(clbbl[0x02])}) -> {Convert.ToString(addr2)}");
            MemoryBlock ctba = new((uint)addr2,0x100);
            ulong addr3;
            if (dev.Is64Support) {
                addr3 = (ulong)ctba[0x1e] << 32 | ctba[0x1D];
            } else {
                addr3 = ctba[0x1d];
            }
            MemoryBlock data = new((uint)addr3,0x100);
            for (uint i=0;i<0x10;i++) {
                FrConsole.Write($"{Convert.ToString(data[i])} ");
            }
        }
        private static void WriteCommand2Device(int portno, AHCI_Dev dev, uint Command) {
            if (dev.mem.port[portno].CLB == 0) {
                FrConsole.ForegroundColor = Color.OrangeRed;
                FrConsole.WriteLine("The AHCI port you selected is not allocated!");
                FrConsole.ResetColor();
                return;
            }
            ulong addr = ((ulong)dev.mem.port[portno].CLBU << 32) | dev.mem.port[portno].CLB;
            MemoryBlock clbbl = new((uint)addr,0x21);
            uint dw0 = clbbl[0];
            HBA_CMD_HEADER header=new() {
                CommandFISLength = (byte)(dw0 & 0xF),
                prdtl = (ushort)(dw0 >> 16),
                ATAPI = (byte)(dw0 & (1 << 5)),
                Write = (byte)(dw0 & (1 << 6)),
                Prefetch = (byte)(dw0 & (1 << 7)),
                Reset = (byte)(dw0 & (1 << 8)),
                BIST = (byte)(dw0 & (1 << 9)),
                ClearBusy = (byte)(dw0 & (1 << 10)),
                PortMultiplier = (byte)((dw0 >> 12)&0xF)
            };
            if (header.CommandFISLength == 0 || header.CommandFISLength == 1) {
                FrConsole.ForegroundColor = Color.OrangeRed;
                FrConsole.WriteLine("Illegal Command List Length!");
                FrConsole.ResetColor();
                return;
            }
            ulong addr2;
            if (dev.Is64Support)
                addr2 = (ulong)clbbl[0x03] << 32 | clbbl[0x02];
            else
                addr2 = clbbl[0x02];

            //FrConsole.WriteLine($"CTB Addr: {Convert.ToString(clbbl[0x02])} (Original: {Convert.ToString(clbbl[0x02])}) -> {Convert.ToString(addr2)}");
            MemoryBlock ctba = new((uint)addr2,0x100);
            ctba[0] = Command;
        }

        private static void ReadFIS(int portno, AHCI_Dev dev) {
            if (dev.mem.port[portno].CLB == 0) {
                FrConsole.ForegroundColor = Color.OrangeRed;
                FrConsole.WriteLine("The AHCI port you selected is not allocated!");
                FrConsole.ResetColor();
                return;
            }
            ulong addr = (ulong)dev.mem.port[portno].FBU << 32 | dev.mem.port[portno].FB;
            MemoryBlock bl = new((uint)addr,0xFF);
            FrConsole.Write($"Some value: ");
            for (uint i=0;i<4;i++) {
                FrConsole.Write($"{Convert.ToString(bl[i])} ");
            }
        }
        // Let's fcking go!!! 🗣️🗣️🔥🔥🔥
        private static void Read28(int portno,AHCI_Dev dev) {
            dev.mem.port[portno].InterruptStatus = 0xFFFFFFFF;
            dev.mem.port[portno].block[0x10] = dev.mem.port[portno].InterruptStatus;
            dev.mem.port[portno].header.CommandFISLength=5;
            dev.mem.port[portno].header.Write=0;
            
            
        }

        // Let's go! 🗣️🗣️🔥🔥🔥
        private static void Identify(int portno, AHCI_Dev dev) {
            FIS_REG_H2D fis = new()
            {
                fis_type = (byte)FIS_TYPE.FIS_TYPE_REG_H2D,
                Command = 0xEC,
                Devices = 0x80,
                WriteCommand = 1
            };
    
            uint Command= (uint)(fis.WriteCommand << 24 | fis.Command << 16 | fis.Devices << 8 | fis.fis_type);
            FrConsole.WriteLine($"Identify Command: {Convert.ToString(Command)}");
            WriteCommand2Device(portno,dev,Command);
            dev.mem.port[portno].block[0x38] = 1 ; // start command from FIS
            dev.mem.port[portno].CommandIssue = dev.mem.port[portno].block[0x38];
            ReadCommandList(portno,dev);
        }
        public static List<int> port_impl;

        private static int FreeCmdSlot(int portno, AHCI_Dev dev) {
            uint slot = dev.mem.port[portno].SATAControl |dev.mem.port[portno].CommandIssue;
            for (int i=0;i<64;i++) {
                if ((slot&1)==0) {
                    return i;
                }
                slot >>=1;
            }
            return -1;
        }

        void port_Rebase(AHCI_Dev d, int portno) {
            StopCommand(d.mem.port[portno]);
            d.mem.port[portno].CLB = (uint)(d.Bar + (portno << 10));
            d.mem.port[portno].CLBU = 0;
            d.mem.port[portno].block[0]=d.mem.port[portno].CLB;
            d.mem.port[portno].block[1]=d.mem.port[portno].CLBU;
            HBA_CMD_HEADER[] header=new HBA_CMD_HEADER[32];
            MemoryBlock bl=new(d.mem.port[portno].CLB, 256);
            for (int i=0;i<32;i++) {
                header[i].prdtl=8;
                header[i].ctba = (uint)(d.Bar + (40 << 10) + (portno << 13) + (i << 8));
                header[i].ctbau = 0;
                bl[0] = (uint)(header[i].prdtl << 16);
                bl[2]=header[i].ctba;
                bl[3]=header[i].ctbau;
            }
            StartCommand(d.mem.port[portno]);
        }
 
        public AHCI() {
            pci.EnableDevice();
            pci.EnableMemory(true);
            // Built-in AHCI just...crash.
            //Cosmos.HAL.BlockDevice.AHCI _ = new (pci);
            var memblock = new MemoryBlock(pci.ReadRegister32(0x24),0x100);
            port_impl=new List<int>();
            dev=new() {
                Bar = pci.ReadRegister32(0x24) , // Get BAR5 Register
                mem = new() {
                    port = new HBA_PORT[31],
                }
            };
            SetAHCIRegister(memblock,31,1,dev.mem.ghc); // Enable AHCI
            Message.Send("AHCI Enabled");
            dev.mem.cap = GetAHCIRegister(memblock,0);
            dev.mem.Version = GetAHCIRegister(memblock,0x10);
            GetVersion(ref dev);
            INTs.SetIrqHandler(pci.InterruptLine, AHCI_Handler);
            SetAHCIRegister(memblock,1,1,dev.mem.ghc);
            FrConsole.WriteLine($"Host Capabilities: {Convert.ToString(dev.mem.cap)}");
            FrConsole.WriteLine($"AHCI Version: {dev.name}");
            FrConsole.WriteLine($"Is support 64-bit addressing: {Convert.ToString((dev.mem.cap >> 31) & 1)}");
            dev.Is64Support = ((dev.mem.cap >> 31) & 1) == 1;
            dev.mem.PortImpl = GetAHCIRegister(memblock,0x0C);
            FrConsole.WriteLine($"Port Implemented: {Convert.ToString(dev.mem.PortImpl)}");
            ProbePort(dev.mem);
            foreach(int i in port_impl) {
                Identify(0,dev);
            }
        }
    }
}