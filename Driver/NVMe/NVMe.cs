using System;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
using TerminalOS_L.FrameBuffer;

// https://nvmexpress.org/wp-content/uploads/NVM-Express-Base-Specification-Revision-2.1-2024.08.05-Ratified.pdf

//A Cooked version of NVMe driver.

namespace TerminalOS_L.Driver.NVMe {
    public class NVMe {
        public uint BaseAddr;
        private static MemoryBlock bl;
        private static uint ReadRegisters(uint offset) {
            return bl[offset];
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
            switch(MJR) {
                case 1:
                    switch(MNR) {
                        case 0:
                            return "NVMe Version 1.0";
                        case 1:
                            return "NVMe Version 1.1";
                        case 2:
                            switch(TER) {
                                case 0:
                                    return "NVMe Version 1.2";
                                case 1:
                                    return "NVMe Version 1.2.1";
                            }
                            break;
                        case 3:
                            return "NVMe Version 1.3";
                        case 4:
                            return "NVMe Version 1.4";
                    }
                    break;
                case 2:
                    switch(MNR) {
                        case 0:
                            return "NVMe Version 2.0";
                        case 1:
                            return "NVMe Version 2.1";
                    }
                    break;
            }
            return "Unknown device.";
        }

        private static BAR_NVMe bar_nvme;

        public NVMe() {
            PCIDevice dev = PCI.GetDeviceClass(ClassID.MassStorageController,SubclassID.NVMController);
            if (dev.DeviceExists) {
                Message.Send_Log("Found NVMe device.");
                Message.Send_Log($"{dev.bus}:{dev.DeviceID}.{dev.function} Non-Volatile memory controller: {PCIDevice.DeviceClass.GetDeviceString(dev)}");
            } else {
                Message.Send_Error("No NVMe device was found!");
                return;
            }
            dev.EnableDevice();
            dev.EnableBusMaster(true);
            dev.EnableMemory(true);
            BaseAddr = (dev.BaseAddressBar[1].BaseAddress << 32) | (dev.BAR0 & 0xFFFFFFF0);
            var nvme_cap = (BaseAddr >> 12) &0xf;

            StringBuilder builder =new();
            builder.AppendFormat("NVMBase: {0}\nNVMECap: {1}\n", BaseAddr, nvme_cap);
            builder.AppendFormat("Interrupt Line: {0}\n", dev.InterruptLine);
            Console.WriteLine(builder.ToString());

            INTs.SetIrqHandler(dev.InterruptLine, NVMe_Handler);
            INTs.SetIRQMaskState(dev.InterruptLine,true);

            //Read NVMe Base Addr
            bl = new(BaseAddr,0x38);
            //FrConsole.WriteLine($"NVMe version: {Convert.ToString(ReadRegisters(0x08))}");

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
            var MQES = bar_nvme.ControllerCap & 0x0ffff; // get first 15 bit or value
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

            FrConsole.WriteLine($"NVMe Version: {Convert.ToString(bar_nvme.Version)}");
            byte TER = (byte)(bar_nvme.Version & 0xff); // get the first 8 bits (byte)
            var MNR = (bar_nvme.Version >> 8) & 0xff;
            var MJR = (bar_nvme.Version >> 16) & 0xff;
            FrConsole.WriteLine($"TER: {Convert.ToString(TER)}");
            FrConsole.WriteLine($"MNR: {Convert.ToString(MNR)}");
            FrConsole.WriteLine($"MJR: {Convert.ToString(MJR)}");
            FrConsole.WriteLine(PrintNVMeVersion(MJR,MNR,TER));


            var Enable = bar_nvme.ControllerConfig & 1; // get the first bit
            FrConsole.WriteLine($"Control Config: {Convert.ToString(bar_nvme.ControllerConfig)}");
            FrConsole.WriteLine($"Enable: {Convert.ToString(Enable)}");
            
            uint CRIME = (bar_nvme.ControllerConfig >> 24) & 0xf & 1;
            FrConsole.WriteLine($"CRIME: {Convert.ToString(CRIME)}");

            FrConsole.WriteLine($"Controller Status: {Convert.ToString(bar_nvme.ControllerStatus)}");

            FrConsole.WriteLine($"Ready: {Convert.ToString(bar_nvme.ControllerStatus & 1)}");
            //Testing NVM SubSystem Reset
            WriteRegisters(0x20, 0x4E564D65);
            FrConsole.WriteLine($"ASQ: {Convert.ToString(bar_nvme.AdminSubmissionQueue)}");
            FrConsole.WriteLine($"ASQB: {Convert.ToString(bar_nvme.AdminSubmissionQueue >> 12)}");
        }
    }
}