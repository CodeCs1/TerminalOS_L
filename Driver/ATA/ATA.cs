using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL.BlockDevice;
/* ATA Driver, The ReCook version of Cosmos ATA! */
namespace TerminalOS_L.Driver {
    public class ATA {
        public bool IsMaster;
        public ATA(ushort Port, bool IsMaster) {
            this.IsMaster = IsMaster;
            _= new ATARegisters(Port);
        }

        private static byte GetBitsSet(byte bytes, int location) {
            return (byte)(bytes & (1 << location));
        }        

        private static string GetInfo(ushort[] buffer, int start, int size) {
            byte[] array = new byte[size];
            int counter = 0;
            for (int i=start;i<start+size/2;i++) {
                var it = buffer[i];
                var bytes = BitConverter.GetBytes(it);
                array[counter++] = bytes[1];
                array[counter++] = bytes[0];
            }
            return Encoding.ASCII.GetString(array);
        }

        public uint BlockCount;
        public static int DriveNumber;
        public static List<string> DeviceName = new();

        /// <summary>
        ///     Identify the disk.
        /// </summary>
        /// <returns>ATA PIO Status</returns>

        public int Identify() {
            ATARegisters.DriveRegisters = (byte)(IsMaster ? 0xa0 : 0xb0);
            ATARegisters.SectorCountRegister = 0;
            ATARegisters.LBALow = 0;
            ATARegisters.LBAMid = 0;
            ATARegisters.LBAHi = 0;


            if (ATARegisters.StatusRegisters == 0XFF) {
                Message.Send_Error("Floating Bus was found. No Drive found!");
                return 0;
            }
            
            ATARegisters.CommandRegisters = 0xEC;
            if (ATARegisters.StatusRegisters == 0) {
                Message.Send_Error("No devices were found!");
                return 0;
            }

            int drvnum=GetBitsSet(ATARegisters.DriveRegisters, 4);
            Message.Send_Log("Selected Drive Number: "+ drvnum);
            DriveNumber=drvnum;

            Message.Send_Log("Polling Device...");
            //Polling unstil Staus Registers is clear
            while ((ATARegisters.StatusRegisters & 0x80) != 0);
            if (ATARegisters.LBAMid != 0 && ATARegisters.LBAHi != 0) {
                Message.Send_Error("ATAPI, not ATA.");
                return 0;
            }
            Message.Send_Log("Testing Device...");
            while((ATARegisters.StatusRegisters & 0x08) == 0) {
                if ((ATARegisters.StatusRegisters & 0x01) != 0) {
                    if (GetBitsSet(ATARegisters.ErrorRegister,0) != 0) 
                        Message.Send_Error("Address mark not found.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,1) != 0) 
                       Message.Send_Error("Track zero not found.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,2) != 0) 
                       Message.Send_Error("Aborted command.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,3) != 0) 
                       Message.Send_Error("Media change request.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,4) != 0) 
                       Message.Send_Error("ID not found.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,5) != 0) 
                       Message.Send_Error("Media changed.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,6) != 0) 
                       Message.Send_Error("Uncorrectable data error.");
                    else if (GetBitsSet(ATARegisters.ErrorRegister,7) != 0) 
                       Message.Send_Error("Bad Block detected.");
                    else
                        Message.Send_Error("Unknown Error");
                    return 0;
                }
            }
            ushort[] buffer = new ushort[256];
            for (ushort i=0;i<256;i++) {
                buffer[i] = ATARegisters.DataRegister;
            }

            Message.Send_Log("Got Status: "+ATARegisters.StatusRegisters);
            string Serial = GetInfo(buffer, 10, 20);
            string Firmware = GetInfo(buffer, 23, 8);
            string Model = GetInfo(buffer, 27, 40);
            uint blockc = ((uint)buffer[61] << 16 | buffer[60]) - 1;
            Message.Send_Log("Got SerialNo: "+Serial);
            Message.Send_Log("Got FirmwareRev: "+Firmware);
            Message.Send_Log("Got Model: "+Model);
            Message.Send_Log("Got blockCount: "+blockc);
            Message.Send_Log("Getting device name: /dev/sd"+(char)(97+drvnum));
            DeviceName.Add("sd"+(char)(97+drvnum));

            Message.Send_Log("Is device support LBA48 (0: No, 1: Yes): " + GetBitsSet(ATARegisters.StatusRegisters,10));


            return ATARegisters.StatusRegisters;
        }


        private void Insw(ushort data, ref byte[] buffer, int count) {
            for (ushort i=0;i<count;i++) {
                buffer[i] = (byte)IOPort.Read16(data);
            }
        }

        public void Read28(int LBA, int Count,ref byte[] data) {
            ATARegisters.DriveRegisters = (byte)((IsMaster ? 0xe0 : 0xf0) | ((IsMaster ? 0 : 1) << 4) | ((LBA >> 24) & 0x0F));
            ATARegisters.FeaturesRegister = 0x00;
            ATARegisters.SectorCountRegister = 1;
            ATARegisters.LBALow = (byte)(LBA & 0xff);
            ATARegisters.LBAMid = (byte)((LBA >> 8)& 0xff);
            ATARegisters.LBAHi = (byte)((LBA >> 16) & 0xff);
            ATARegisters.CommandRegisters =0x20;
            while((ATARegisters.StatusRegisters & 0x80) != 0);
            if ((ATARegisters.StatusRegisters & 0x01) != 0) {
                Message.Send_Error("Error while read sectors!");
                return;
            } else if (GetBitsSet(ATARegisters.StatusRegisters,5) != 0) {
                Message.Send_Error("Drive Fault.");
                return;
            } else if (GetBitsSet(ATARegisters.StatusRegisters, 3) == 0) {
                Message.Send_Error("The device is not accept PIO data!");
                return;
            }

            for (int i=0;i<Count;i++) {
                ushort wdata = ATARegisters.DataRegister;
                data[i] = (byte)(wdata & 0xff); // -> 71
                if (i+1 < Count) {
                    data[i+1] = (byte)(wdata>>8); // 5f
                }
                i++;
            }
            Delay40NS();
        }


        public void Write28(int LBA, int count, ref byte[] data) {
            ATARegisters.DriveRegisters = (byte)((IsMaster ? 0xe0 : 0xf0) | ((IsMaster ? 0 : 1) << 4) | ((LBA >> 24) & 0x0F));
            ATARegisters.SectorCountRegister = (byte)count;
            ATARegisters.LBALow = (byte)LBA;
            ATARegisters.LBAMid = (byte)(LBA >> 8);
            ATARegisters.LBAHi = (byte)(LBA >> 16);
            ATARegisters.CommandRegisters =0x30;
            while ((ATARegisters.StatusRegisters & 0x80) != 0);
            for (int i=0;i<count;i++) {
                ATARegisters.DataRegister = data[i];
            }
            Flush();
        }

        public void ReadSector28(int LBA, ref byte[] data) {
            ATARegisters.ControlRegisters = 0x02;
            while ((ATARegisters.StatusRegisters & 0x80) != 0);
            ATARegisters.DriveRegisters = (byte)((IsMaster ? 0xe0 : 0xf0) | ((IsMaster ? 0 : 1) << 4) | ((LBA & 0x0f000000) >> 24));
            ATARegisters.FeaturesRegister = 0x00;
            ATARegisters.SectorCountRegister = 1;
            ATARegisters.LBALow = (byte)((LBA & 0x000000ff) >> 0);
            ATARegisters.LBAMid = (byte)((LBA & 0x0000ff00) >> 8);
            ATARegisters.LBAHi = (byte)((LBA & 0x00ff0000) >> 16);

            ATARegisters.CommandRegisters =0x30;

            
            while ((ATARegisters.StatusRegisters & 0x80) != 0);
            if ((ATARegisters.StatusRegisters & 0x01) != 0) {
                Message.Send_Error("Error while read sectors!");
                return;
            } else if (GetBitsSet(ATARegisters.StatusRegisters,5) != 0) {
                Message.Send_Error("Drive Fault.");
                return;
            } else if (GetBitsSet(ATARegisters.StatusRegisters, 3) == 0) {
                Message.Send_Error("The device is not accept PIO data!");
                return;
            }

            for (ushort i=0;i<256;i++) {
                ATARegisters.DataRegister = data[i];
            }
            Delay40NS();
        }

        private void Flush() {
            ATARegisters.DriveRegisters = (byte)(IsMaster ? 0xe0:0xf0);
            ATARegisters.CommandRegisters = 0xe7;
            if (ATARegisters.StatusRegisters == 0x00) {
                return;
            }


            while ((ATARegisters.StatusRegisters & 0x80) != 0);
            while((ATARegisters.StatusRegisters & 0x01) == 0);

            if ((ATARegisters.StatusRegisters &0x01) != 0) {
                Message.Send_Error("Flush failed!");
                return;
            }
        }

        public static void Delay40NS() {
            int i=0;
            while (i < 4) {
                IOPort.Wait();
                i++;
            }
        }
    }
}