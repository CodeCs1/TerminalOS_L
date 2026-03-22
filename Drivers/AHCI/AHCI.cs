using Cosmos.Kernel.HAL.Cpu;
using Cosmos.Kernel.HAL.Cpu.Data;
using Cosmos.Kernel.HAL.X64.Pci;
using TerminalOS_Lgen3.Disk;
using Cosmos.Kernel.Core.IO;
using System.Runtime.InteropServices;
using Cosmos.Kernel.Core.Memory;
using System.Runtime.CompilerServices;

namespace TerminalOS_Lgen3.Drivers.AHCI
{

    public unsafe partial class SATAPort
    {
        public volatile HBA_PORT* p;
        public uint port_num;
        public SATAPortType sATA;
        private static ulong VirtualToPhysical(ulong virtualAddress)
        {
            // Higher Half Kernel mapping: virtual addresses start with 0xFFFF8000
            // Remove the higher half offset to get physical address
            const ulong HigherHalfOffset = 0xFFFF800000000000UL;
            if (virtualAddress >= HigherHalfOffset)
            {
                return virtualAddress - HigherHalfOffset;
            }
            // If not in higher half mapping, assume it's already physical
            return virtualAddress;
        }

        private void StopPort()
        {
            unchecked
            {
                p->cmd &= (uint)~0x01;
                while ((p->cmd & (1 << 15)) != 0) ;
                p->cmd &= (uint)~0x10;
                while ((p->cmd & (1 << 14)) != 0) ;
            }
        }

        private void StartPort()
        {
            while ((p->cmd & (1 << 15)) != 0) ;
            p->cmd |= 0x10;
            p->cmd |= 0x01;
        }
        private int FindSlot()
        {
            uint slot = p->sact | p->ci;
            for (int i = 0; i < 32; i++)
            {
                if ((slot & (1 << i)) == 0) return i;
            }
            return -1;
        }
        public static byte* Span2Byte(Span<byte> s)
        {
            if (s.IsEmpty) return null;

            ref byte refspan = ref MemoryMarshal.GetReference(s);

            fixed(byte* p = &refspan)
            {
                return p;
            }
        }

        internal bool ReadWrite(ulong sector, ushort count, byte* buffer, bool write)
        {
            if (count >= 512) { Serial.Write("[AHCI] Overflow sector count \n");return false; }
            
            unchecked
            {
                p->int_stat = (uint)-1;
            }
            int slot = FindSlot();

            if (slot == -1) return false;
            HBA_CMD_HEADER* h = (HBA_CMD_HEADER*)p->clb;
            h += slot;

            h->CommandFISLength = (byte)(sizeof(FIS_REG_H2D) / sizeof(uint));
            h->Write = write;
            h->ClearBusy = true;
            h->prdtl = (ushort)(((count-1)>>4)+1);

            HBA_CMD_TBL* tbl = (HBA_CMD_TBL*)h->ctba;
            int i=0;

            h->prdtl = 8;

            for (i=0;i<h->prdtl-1;i++)
            {
                (&tbl->prdt_entry)[i].dba = VirtualToPhysical((ulong)buffer);
                (&tbl->prdt_entry)[i].ByteCount = 8 * 1024 - 1;
                (&tbl->prdt_entry)[i].IOC = true;
                buffer += 4 * 1024;
                count -= 16;
            }

            
            (&tbl->prdt_entry)[i].dba = VirtualToPhysical((ulong)buffer);
            (&tbl->prdt_entry)[i].ByteCount = (uint)((count<<9)-1);
            (&tbl->prdt_entry)[i].IOC = true;

            FIS_REG_H2D* FIS = (FIS_REG_H2D*)tbl->cfis;
            FIS->fis_type = 0x27;
            FIS->control = 1;
            FIS->command = (byte)(write ? 0x30 : 0x20);

            FIS->lba0 = (byte)(sector & 0xff);
            FIS->lba1 = (byte)((sector>>8) & 0xff);
            FIS->lba2 = (byte)((sector >>16)& 0xff);
            FIS->lba3 = (byte)((sector >>24)& 0xff);
            FIS->lba4 = (byte)((sector >>32)& 0xff);
            FIS->lba5 = (byte)((sector >>40)& 0xff);

            FIS->device = 1<<6;
            FIS->count = count;
            uint timer=100_000_000;
            while( (p->tfd & (0x80 | 0x08)) !=0 && timer-- > 0);
            if (timer == 0) {
                Serial.Write("[AHCI]: TFD is still running, exit\n");
                return false;
            }

            p->ci = (uint)(1<<slot);
            timer=100_000_000;

            while(timer-- > 0)
            {
                if ((p->ci & (1<<slot)) == 0) break;

                if ((p->int_stat & (1<<30)) !=0) return false;
            }
            if (timer == 0) {
                Serial.Write("[AHCI]: CI never cleared, exiting\n");
                return false;
            }
            return true;
        }

        internal IdentifyDevice? Identify()
        {
            unchecked
            {
                p->int_stat = (uint)-1;
            }
            int slot = FindSlot();

            if (slot == -1) return null;
            HBA_CMD_HEADER* h = (HBA_CMD_HEADER*)p->clb;
            h += slot;

            h->CommandFISLength = (byte)(sizeof(FIS_REG_H2D) / sizeof(uint));
            h->Write = false;
            h->ClearBusy = true;
            h->prdtl = 1;


            HBA_CMD_TBL* tbl = (HBA_CMD_TBL*)h->ctba;
            var alloc = new ManagedMemoryBlock(512, 512);
            alloc.Fill(0);
            byte* ptr = Span2Byte(alloc.Span);
            (&tbl->prdt_entry)[0].dba = VirtualToPhysical((ulong)ptr);
            (&tbl->prdt_entry)[0].ByteCount = 511;

            FIS_REG_H2D* FIS = (FIS_REG_H2D*)tbl->cfis;
            FIS->fis_type = 0x27;
            FIS->control = 1;
            FIS->command = 0xec;


            uint timer=100_000_000;
            while( (p->tfd & (0x80 | 0x08)) !=0 && timer-- > 0);
            if (timer == 0) {
                Serial.Write("[AHCI]: TFD is still running, exit\n");
                return null;
            }

            p->ci = (uint)(1<<slot);
            timer=100_000_000;

            while(timer-- > 0)
            {
                if ((p->ci & (1<<slot)) == 0) break;

                if ((p->int_stat & (1<<30)) !=0) return null;
            }
            if (timer == 0) {
                Serial.Write("[AHCI]: CI never cleared, exiting\n");
                return null;
            }

            var sp = alloc.GetSpan(0, Unsafe.SizeOf<IdentifyDevice>());

            return MemoryMarshal.Read<IdentifyDevice>(sp);
        }
    }

    public unsafe partial class AHCI : PciDevice, IDisk
    {
        private static PciDevice? dev;

        private static HBA_MEM* mem;
        private static ulong ABAR;


        public string Id { get => "ahci"; }
        
        private static readonly List<SATAPort> portImpl = new(32);

        public uint AvailableDisk
        {
            get{
                uint disks = 0;
                for (int i=0;i<portImpl.Count;i++)
                {
                    switch (portImpl[i].sATA)
                    {
                        case SATAPortType.SATA:
                            disks |= 1u<<i;
                            break;
                        default:
                            break;
                    }     
                }
                return disks;
            }
        }

        private static SATAPortType CheckType(HBA_PORT* port)
        {
            uint ssts = port->ssts;
            byte ipm = (byte)((ssts >> 8) & 0x0f);
            byte det = (byte)(ssts & 0x0f);


            Serial.WriteString($"IPM: {ipm}; DET: {det}; ssts: {ssts}\n");

            if (det != 3 || ipm != 1) return 0;

            Serial.Write($"[AHCI] Get SATA Signature 0x{port->sig:X}\n");

            return port->sig switch
            {
                0xEB140101 => SATAPortType.ATAPI,
                0xC33C0101 => SATAPortType.SEMB,
                0x96690101 => SATAPortType.PM,
                0x00000101 => SATAPortType.SATA,
                _ => SATAPortType.NONE
            };
        }

        private static void ProbePort()
        {
            uint pi = mem->pi;
            uint i = 0;
            while (pi > 0)
            {
                if ((pi & 1) == 1)
                {
                    Serial.Write($"[AHCI] HBA port #{i}: 0x{HBA_MEM.GetPort(ABAR,i):x}\n");
                    Serial.Write($"[AHCI] HBA port cast value: 0x{(ulong)&(&mem->ports)[i]:x}\n");
                    //uint dt = CheckType(HBA_MEM.GetPort(i,ABAR));

                    SATAPortType dt = CheckType(
                        &(&mem->ports)[i]
                    );

                    Serial.Write($"[AHCI] Get SATAPortType: {(uint)dt}\n");

                    switch (dt)
                    {
                        case SATAPortType.NONE:
                            Serial.Write($"[AHCI] No disk found on port #{i}\n");
                            break;
                        default:
                            Serial.Write($"[AHCI] Disk found at port {i}\n");
                            SATAPort p = new()
                            {
                                p = &(&mem->ports)[i],
                                sATA = dt,
                                port_num = i
                            };
                            portImpl.Add(
                                p
                            );
                            break;
                    }
                }
                pi >>= 1;
                i++;
            }
        }

        public AHCI(uint bus, uint slot, uint function) : base(bus, slot, function)
        {
            ABAR = new UIntPtr(
                BaseAddressBar[5].BaseAddress & 0xFFFFFFF0
            ); // map hba_mem
            Serial.Write($"[AHCI] ABAR 0x{ABAR:x}\n");
            mem = (HBA_MEM*)new UIntPtr(ABAR);
            ProbePort();
        }

        private static void AHCI_IRQHandler(ref IRQContext cont)
        {
            Console.WriteLine("AHCI Handler occured while doing AHCI job");
            Console.WriteLine($"INTERRUPT NO: #{cont.interrupt}");
        }
        public static AHCI? CreateInterface()
        {
            dev = PciManager.GetDeviceClass(
                Cosmos.Kernel.HAL.X64.Pci.Enums.ClassId.MassStorageController,
                Cosmos.Kernel.HAL.X64.Pci.Enums.SubclassId.SataController
            );
            if (dev != null && !dev.Claimed)
            {
                InterruptManager.SetIrqHandler(dev.InterruptLine, AHCI_IRQHandler);
                return new AHCI(dev.Bus, dev.Slot, dev.Function);
            }

            return null;
        }

        public static AHCI? Init()
        {
            var ahci = CreateInterface();
            if (ahci == null) return null;

            ahci.EnableDevice();
            ahci.EnableBusMaster(true);
            ahci.EnableMemory(true);
            ahci.Claimed = true;

            return ahci;
        }


        public IdentifyDevice? Identify(uint DiskNo)
        {
            switch(portImpl[(int)DiskNo].sATA) {
                case SATAPortType.SATA:
                    return portImpl[(int)DiskNo].Identify();
                default:
                    Serial.WriteString("[AHCI]: Port not implemented");
                    return null;
            }
        }

        public Span<byte> Read(uint DiskNo,uint sectorNo, uint size)
        {
            var data = new ManagedMemoryBlock(size);
            switch(portImpl[(int)DiskNo].sATA) {
                case SATAPortType.SATA:
                    portImpl[(int)DiskNo].ReadWrite(sectorNo,(ushort)size,SATAPort.Span2Byte(data.Span),false);
                    break;
                default:
                    Serial.WriteString("[AHCI]: Port not implemented");
                    return [];
            }
            return data.Span;

        }

        public void Write(uint DiskNo,uint sectorNo, uint size, byte[] data)
        {
            throw new NotImplementedException();
        }

        
        
    }
}