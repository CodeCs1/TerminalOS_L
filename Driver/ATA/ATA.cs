using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL.BlockDevice;
/* ATA Driver, The ReCook version of Cosmos ATA! */
namespace TerminalOS_L.Driver {

    public class ATAPI {
        public bool IsMaster;

        public ATAPI(ushort Port, bool IsMaster) {
            this.IsMaster = IsMaster;
            _ = new ATARegisters(Port);
        }
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

            ATARegisters.CommandRegisters = 0xA1; //ATAPI Packet Identify Command

            while ((ATARegisters.StatusRegisters & 0x80) != 0);

            if ((ATARegisters.StatusRegisters & 0x01) != 0) {
                Message.Send_Error("Error while identify atapi disk");
                return 0;
            }
            ushort[] buffer = new ushort[256];
            for (ushort i=0;i<256;i++) {
                buffer[i] = ATARegisters.DataRegister;
            }

            Message.Send_Log("Got Status: "+ATARegisters.StatusRegisters);
            string Serial = ATA.GetInfo(buffer, 10, 20);
            string Firmware = ATA.GetInfo(buffer, 23, 8);
            string Model = ATA.GetInfo(buffer, 27, 40);
            uint blockc = ((uint)buffer[61] << 16 | buffer[60]) - 1;
            Message.Send_Log("Got SerialNo: "+Serial);
            Message.Send_Log("Got FirmwareRev: "+Firmware);
            Message.Send_Log("Got Model: "+Model);
            Message.Send_Log("Got blockCount: "+blockc);
            Message.Send_Log("Getting device name: /dev/sr0");

            return ATARegisters.StatusRegisters;
        }
        public void Read28(int LBA, int Count,ref byte[] data) {
            byte[] Read_cmd = new byte[12] {
                0xA8, 0, (byte)((LBA>>0x18)&0xff),
                (byte)((LBA>>0x10)&0xff),
                (byte)((LBA>>8)&0xff),
                (byte)((LBA>>0)&0xff),
                (byte)((Count>>0x18)&0xff),
                (byte)((Count>>0x10)&0xff),
                (byte)((Count>>8)&0xff),
                (byte)((Count>>0)&0xff),
                0,0
            };
            ATARegisters.DriveRegisters = (byte)(0xA0 & ((IsMaster ? 0 : 1)<<4));
            ATA.Delay40NS();
            ATARegisters.FeaturesRegister = 0x00;
            ATARegisters.LBAMid = 2048 & 0xff;
            ATARegisters.LBAHi = 2048 >> 8;
            ATARegisters.CommandRegisters = 0xA0;
            ATA.Delay40NS();
            while(true) {
                byte status = ATARegisters.StatusRegisters;
                if ((status & 0x01) == 1) {
                    Message.Send_Error("Error when reading ATAPI device!");
                    return ;
                }
                if ((status & 0x80) == 0 && (status & 0x80) != 0) {
                    break;
                }
                ATA.Delay40NS();
            }
            byte[] array = new byte[2048];
            ushort[] buffer = new ushort[1024];
            for (int i=0;i<buffer.Length;i++) {
                var b1 = (byte)buffer[i];
                var b2 = (byte)buffer[i+1];
                var Data = BitConverter.ToUInt16(new byte[] {b1, b2}, 0);
                ATARegisters.DataRegister = Data;
                i++;
            }
            var t = ATARegisters.LBAMid << 8;
            var sz = t | ATARegisters.LBALow;
            if (sz != 2048) {
                Message.Send_Error("Size Mismatch!");
                return ;
            }
            if (sz != 0) {
                for (int i=0;i<1024;i++) {
                    buffer[i] = ATARegisters.DataRegister;
                }
            }
            /*
            unsafe {
                fixed(byte* ptr = array) {
                    fixed (ushort* ptr2 = buffer) {
                        byte* ptr3 = (byte*)ptr2;
                        MemoryOperations.Copy(ptr, ptr3, 2048);
                    }
                }
            }*/
            data = array;

        }
    }

    public class ATA {
        public bool IsMaster;
        public static bool IsCDRom=false;
        public ATA(ushort Port, bool IsMaster) {
            this.IsMaster = IsMaster;
            _= new ATARegisters(Port);
        }

        private static byte GetBitsSet(byte bytes, int location) {
            return (byte)(bytes & (1 << location));
        }        

        public static string GetInfo(ushort[] buffer, int start, int size) {
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
                //The device is ATAPI device
                new ATAPI(ATARegisters.Port, IsMaster).Identify();
                //Message.Send_Error("ATAPI device is not supported yet!");
                IsCDRom=true;
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


        public void Read28(int LBA, int Count,ref byte[] data) {
            if (IsCDRom) {
                new ATAPI(ATARegisters.Port, IsMaster).Read28(LBA, 1,ref data);
                return;
            }
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

        public void Write28(int LBA, int Count,ref byte[] data) {
            if (IsCDRom) {
                Message.Send_Warning("Can't write to CD-Rom!");
                return;
            }
            ATARegisters.DriveRegisters = (byte)((IsMaster ? 0xe0 : 0xf0) | ((IsMaster ? 0 : 1) << 4) | ((LBA & 0x0f) >> 24));
            ATARegisters.FeaturesRegister = 0x00;
            ATARegisters.SectorCountRegister = 1;
            ATARegisters.LBALow = (byte)(LBA & 0xff);
            ATARegisters.LBAMid = (byte)((LBA >> 8)& 0xff);
            ATARegisters.LBAHi = (byte)((LBA >> 16) & 0xff);
            ATARegisters.CommandRegisters =0x30;
            while((ATARegisters.StatusRegisters & 0x80) != 0);
            if ((ATARegisters.StatusRegisters & 0x01) != 0) {
                Message.Send_Error("Error while write sectors!");
                return;
            } else if (GetBitsSet(ATARegisters.StatusRegisters,5) != 0) {
                Message.Send_Error("Drive Fault.");
                return;
            } else if (GetBitsSet(ATARegisters.StatusRegisters, 3) == 0) {
                Message.Send_Error("The device is not accept PIO data!");
                return;
            }
            Message.Send("Writing to LBA...");

            for (int i=0;i<Count;i++) {
                ushort wdata = data[i];
                if (i+1<Count) 
                    wdata|= (ushort)(data[i+1]<<8);
                ATARegisters.DataRegister = wdata;
                i++;
            }
            Message.Send_Log("Completed!");
            Flush(); // Don't forget to flush the device!
            Message.Send_Log("Completed in flushing device!");
        }
        ///<summary>
        /// Flushes the ATA device by sending a flush command and waiting for the operation to complete.
        /// </summary>

        private void Flush() {
            ATARegisters.DriveRegisters = (byte)((IsMaster ? 0xe0 : 0xf0) | ((IsMaster ? 0 : 1) << 4));
            ATARegisters.FeaturesRegister = 0x00;
            ATARegisters.CommandRegisters = 0xe7;
            if (ATARegisters.StatusRegisters == 0x00) {
                return;
            }

            Message.Send_Log("Flushing...");
            while ((ATARegisters.StatusRegisters & 0x80) != 0);

            if ((ATARegisters.StatusRegisters &0x01) != 0) {
                Message.Send_Error("Flush failed!");
                return;
            }

            Message.Send_Log("Flush succeeded!");
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