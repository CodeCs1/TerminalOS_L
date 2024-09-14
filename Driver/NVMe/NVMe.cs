using System;
using System.Drawing;
using System.Text;
using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using TerminalOS_L.FrameBuffer;

// https://nvmexpress.org/wp-content/uploads/NVM-Express-Base-Specification-Revision-2.1-2024.08.05-Ratified.pdf
// For NVMe 1.2 version: https://www.nvmexpress.org/wp-content/uploads/NVM-Express-1_2a.pdf

//A Cooked version of NVMe driver.

namespace TerminalOS_L.Driver.NVMe {
    public class NVMe {
        public ulong BaseAddr;
        private static MemoryBlock bl;
        private static uint ReadRegisters(uint offset) {
            if (offset >=  0x39) {
                FrConsole.BackgroundColor = Color.Red;
                FrConsole.WriteLine("The offset of register can not greater-than 0x38 (56 in decimal).");
                FrConsole.ResetColor();
                return uint.MaxValue; // Can not find other value.
            } else {
                return bl[offset];
            }
        }
        private static void WriteRegisters(uint offset, uint value) {
            bl[offset] = value;
        }

        private static bool CreateAdminSubmission(Queue sq) {
            sq = new()
            {
                Address = new ulong(),
                Size=63
            };
            if (sq.Address == 0) {
                return false;
            }
            WriteRegisters(0x28, (uint)sq.Address);
            return true;
        }
        private static bool CreateAdminCompletion(Queue sq) {
            sq = new()
            {
                Address = new ulong(),
                Size=63
            };
            if (sq.Address == 0) {
                return false;
            }
            WriteRegisters(0x30, (uint)sq.Address);
            return true;
        }
        private void NVMe_Handler(ref INTs.IRQContext aContext) {

        }


        /// <summary>
        ///     Return Version of NVMe device.
        /// </summary>
        /// <param name="MJR">Major Field</param>
        /// <param name="MNR">Minor Field</param>
        /// <param name="TER">Tertiary Field</param>

        // 'If else' but switch version

        private static string PrintNVMeVersion(uint MJR, uint MNR, uint TER) {
            if (MJR == 0 && MNR == 0 && TER ==0) {
                return "Unknown device.";
            } else {
                return $"NVMe version {Convert.ToString(MJR)}.{Convert.ToString(MNR)}.{Convert.ToString(TER)}";
            }
        }

        private enum DoorBellType {
            Submission,
            Completion
        }


        // (1000h + ((2y) * (4 << CAP.DSTRD))
        // (1000h + ((2y+1) * (4 << CAP.DSTRD))
        // WTF ??
        //The host should not read the doorbell registers. If a doorbell register is read, the value returned is vendor
        //specific. Writing to a non-existent Submission Queue Tail Doorbell has undefined results
        private static void ReadDoorbell(DoorBellType type,
        uint index, uint BaseAddr) {
            uint DSTRD = (uint)((bar_nvme.ControllerCap >> 31) & 0xff);
            MemoryBlock doorblock_mem;
            switch (type)
            {
                case DoorBellType.Submission:
                    doorblock_mem = new((uint)(BaseAddr + 0x1000 + (2 * index * (4 << (int)DSTRD))), 0x03);
                    break;
                case DoorBellType.Completion:
                    doorblock_mem = new((uint)(BaseAddr + 0x1000 + ((2 * index + 1) * (4 << (int)DSTRD))), 0x03);
                    break;
                default:
                    return;
            }
            doorblock_mem[0] =0x54455354;
            doorblock_mem[1] =0x54455354; //can't write!
            // wdym the value return  0 ?
            FrConsole.Write($"Doorbell at index: {Convert.ToString(index)}:");
            for (uint i=0;i<3;i++) {
                FrConsole.Write($" {Convert.ToString(doorblock_mem[i])}");
            }
            FrConsole.WriteLine();
        }

        private static BAR_NVMe bar_nvme;

        // That is a lot of writeline...
        public static bool IsEnable=false;
        public static string Version;

        public static void EnableDevice() {
            //Get Controller Config value.
            var Enable = bar_nvme.ControllerConfig & 1;
            if (Enable == 0) {
                uint tmp = bar_nvme.ControllerConfig & 1 | (1 << 0); // Enable the Device
                WriteRegisters(0x14, tmp);
                bar_nvme.ControllerConfig = ReadRegisters(0x14); // Read it again.
            } else {
                FrConsole.WriteLine("The device is already enabled.");
            }
        }

        public static void DisableDevice() {
            //Get Controller Config value.
            var Enable = bar_nvme.ControllerConfig & 1;
            if (Enable == 1) {
                uint tmp = bar_nvme.ControllerConfig & 1 | (0 << 0); // Disable the Device
                WriteRegisters(0x14, tmp);
                bar_nvme.ControllerConfig = ReadRegisters(0x14); // Read it again.
            } else {
                FrConsole.WriteLine("The device is already disabled.");
            }
        }

        public NVMe() {
            PCIDevice dev = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.NVMController);
            if (dev.ClassCode == (ushort)ClassID.MassStorageController &&
            dev.Subclass == (byte)SubclassID.NVMController) {
                Message.Send_Log("Found NVMe device.");
                Message.Send_Log($"{dev.bus}:{dev.DeviceID}.{dev.function} Non-Volatile memory controller: {PCIDevice.DeviceClass.GetDeviceString(dev)}");
            } else {
                Message.Send_Error("No NVMe device was found!");
                return;
            }

            if (IsEnable) {
                FrConsole.WriteLine("The device is already enable, skipping...");
                FrConsole.WriteLine($"Getting NVMe version: {Version}");
                FrConsole.WriteLine($"Getting 'Enable' value: {Convert.ToString(bar_nvme.ControllerConfig & 1)}");
                FrConsole.WriteLine($"Gettind 'Ready' value: {Convert.ToString(bar_nvme.ControllerStatus & 1)}");
                return;
            }
            dev.EnableDevice();
            dev.EnableBusMaster(true);
            dev.EnableMemory(true);
            IsEnable=true;
            BaseAddr = ((ulong)dev.BaseAddressBar[1].BaseAddress << 32) | (dev.BAR0 & 0xFFFFFFF0);
            var nvme_cap = (BaseAddr >> 12) &0xf;

            StringBuilder builder =new();
            builder.AppendFormat("NVMBase: {0}\nNVMECap: {1}\n", BaseAddr, nvme_cap);
            builder.AppendFormat("Interrupt Line: {0}\n", dev.InterruptLine);
            Console.WriteLine(builder.ToString());

            INTs.SetIrqHandler(dev.InterruptLine, NVMe_Handler);
            INTs.SetIRQMaskState(dev.InterruptLine,true);

            //Read NVMe Base Addr
            bl = new((uint)BaseAddr,0x38);
            bar_nvme = new() {
                ControllerCap = ReadRegisters(0),
                Version = ReadRegisters(0x08),
                InterruptMaskset = ReadRegisters(0x0C),
                InterruptMaskClear = ReadRegisters(0x10),
                ControllerConfig = ReadRegisters(0x14),
                ControllerStatus = ReadRegisters(0x1C),
                AdminQueueAttributes = ReadRegisters(0x24),
                AdminSubmissionQueue = ReadRegisters(0x28),
                AdminCompletionQueue = ReadRegisters(0x30)
            };
            var MQES = bar_nvme.ControllerCap & 0x0ffff; // get first 15 bit of value
            var CQR = BitPort.GetBit((uint)bar_nvme.ControllerCap, 16);
            byte[] bAMS =  {
                BitPort.GetBit((uint)bar_nvme.ControllerCap, 17),
                BitPort.GetBit((uint)bar_nvme.ControllerCap, 18)
            };
            var AMS = BitConverter.ToUInt16(bAMS,0);

            FrConsole.WriteLine($"NVMe Controller Capabillities: {Convert.ToString(bar_nvme.ControllerCap)}");
            FrConsole.WriteLine($"MQES: {Convert.ToString(MQES)}");
            FrConsole.WriteLine($"CQR: {Convert.ToString(CQR)}");
            FrConsole.WriteLine($"AMS: {Convert.ToString(AMS)}");
            byte TER = (byte)(bar_nvme.Version & 0xff); // get the first 8 bits (byte)
            var MNR = (bar_nvme.Version >> 8) & 0xff;
            var MJR = (bar_nvme.Version >> 16) & 0xff;
            FrConsole.WriteLine(PrintNVMeVersion(MJR,MNR,TER));

            Version = PrintNVMeVersion(MJR,MNR,TER);

            var Enable = bar_nvme.ControllerConfig & 1; // get the first bit
            if (Enable == 0 && !IsEnable) {
                uint tmp = bar_nvme.ControllerConfig & 1 | (1 << 0); // Enable the Device
                WriteRegisters(0x14, tmp);
                bar_nvme.ControllerConfig = ReadRegisters(0x14);
                Enable = bar_nvme.ControllerConfig & 1; // ?
            }
            FrConsole.WriteLine($"Control Config: {Convert.ToString(bar_nvme.ControllerConfig)}");
            FrConsole.WriteLine($"Enable: {Convert.ToString(Enable)}");
            //FrConsole.WriteLine($"IOCQES: {Convert.ToString((bar_nvme.ControllerConfig >> 20) & 0x0f)}");
            //uint CRIME = (bar_nvme.ControllerConfig >> 24) & 0xf & 1;
            //FrConsole.WriteLine($"CRIME: {Convert.ToString(CRIME)}");
            FrConsole.WriteLine($"Controller Status: {Convert.ToString(bar_nvme.ControllerStatus)}");
            FrConsole.WriteLine($"Ready: {Convert.ToString(bar_nvme.ControllerStatus & 1)}");
            //Testing NVM SubSystem Reset
            WriteRegisters(0x20, 0x4E564D65);
            /*
            FrConsole.WriteLine($"ASQ: {Convert.ToString(bar_nvme.AdminSubmissionQueue)}");
            FrConsole.WriteLine($"ASQB: {Convert.ToString(bar_nvme.AdminSubmissionQueue >> 12)}");
            FrConsole.WriteLine($"ACQ: {Convert.ToString(bar_nvme.AdminCompletionQueue)}");
            FrConsole.WriteLine($"ACQB: {Convert.ToString(bar_nvme.AdminCompletionQueue >> 12)}");
            FrConsole.WriteLine($"ASQS: {Convert.ToString(bar_nvme.AdminQueueAttributes & 0x0FFF)}");
            FrConsole.WriteLine($"ACQS: {Convert.ToString(bar_nvme.AdminQueueAttributes & 17 & 0x0FFF)}");
            */
            ReadDoorbell(DoorBellType.Submission,0,(uint)BaseAddr);
            Queue q=new();
            if (CreateAdminCompletion(q)) {
                FrConsole.WriteLine($"Admin Completetion Addr: {Convert.ToString(q.Address)}");
            }
        }
    }
}