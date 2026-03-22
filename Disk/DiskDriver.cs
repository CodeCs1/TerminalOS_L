namespace TerminalOS_Lgen3.Disk {
    public class DiskDriver {
        /*
            Sample disk id: ide0, ide1, ahci0, ahci1, nvme0, nvme1
        */

        public struct DiskInfo
        {
            public uint AvailableDisk;
            public List<IdentifyDevice?> disks;
        }

        private static Dictionary<IDisk, DiskInfo>? disks;
        public static bool AddDisk(IDisk? d) {
            if (d == null) return false;
            disks ??= [];

            uint ad = d.AvailableDisk;
            List<IdentifyDevice?> identifyDevices = [];
            for (uint i=0,ad1=ad;ad1 > 0;ad1>>=1,i++)
            {
                if ( (ad & (1u<<(int)i)) == 1 ) {
                    identifyDevices.Add(d.Identify(i));
                }
            }

            DiskInfo info=new() {AvailableDisk=ad,disks=identifyDevices};

            disks.Add(d,info);

            return true;
        }

        public static void GetPartition(string disk_id)
        {
            string name = disk_id[..^1];
            int id = disk_id[^1]-'0';


        }
        public static bool Dump() {
            if (disks == null){
                return false;
            }
            Console.WriteLine("Disk Interface Dump\n----------");
            foreach(var d in disks) {
                for (uint i=0,ad=d.Value.AvailableDisk;ad>0;ad>>=1,i++)
                {
                    var diskIdx = d.Value.disks[(int)i];
                    if (!diskIdx.HasValue) continue;

                    Console.WriteLine($"{d.Key.Id}{i}: {diskIdx.Value.GetModelName}");
                }
            }
            return true;
        }
    }
}