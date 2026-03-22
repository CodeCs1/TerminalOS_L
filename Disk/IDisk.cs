namespace TerminalOS_Lgen3.Disk {
    public interface IDisk {
        public string Id {get;}
        public uint AvailableDisk {get;}
        public  IdentifyDevice? Identify(uint DiskNo);
        public Span<byte> Read(uint DiskNo,uint sectorNo, uint size);
        public void Write(uint DiskNo,uint sectorNo, uint size, byte[] data);
    }
}