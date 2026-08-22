using Cosmos.Kernel.HAL.Interfaces.Devices;

namespace TerminalOS_Lgen3.Disk {
    internal sealed class MemoryBlockDevice(string name, ulong blockSize, ulong blockCount) : IBlockDevice
    {
        private readonly byte[] _storage = new byte[blockSize * blockCount];

        public string Name { get; } = name;
        public ulong BlockSize { get; } = blockSize;
        public ulong BlockCount { get; } = blockCount;

        public void ReadBlock(ulong blockNo, ulong blockCount, Span<byte> data)
            => _storage.AsSpan((int)(blockNo * BlockSize), (int)(blockCount * BlockSize)).CopyTo(data);

        public void WriteBlock(ulong blockNo, ulong blockCount, ReadOnlySpan<byte> data)
            => data[..(int)(blockCount * BlockSize)].CopyTo(_storage.AsSpan((int)(blockNo * BlockSize)));

        public void Flush() { }
    }
}
