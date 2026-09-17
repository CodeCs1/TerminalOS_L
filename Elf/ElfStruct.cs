namespace TerminalOS_Lgen3.Elf;

public enum ElfPH_SegmentType : uint
{
    Null = 0,
    Load = 1,
    Dynamic = 2,
    Interp = 3,
    Note = 4,
    Shlib = 5,
    Phdr = 6,
    Tls = 7,
    Num = 8,
    Loos = 0x60000000,
    GnuEhFrame = 0x6474e550,
    GnuStack = 0x6474e551,
    GnuRelro = 0x6474e552,
    GnuProperty = 0x6474e553,
    GnuSFrame = 0x6474e554,
    LowSunw = 0x6ffffffa,
    SunwBSS = 0x6ffffffa,
    SunwStk = 0x6ffffffb,
    HiSunw = 0x6fffffff,
    Hios = 0x6fffffff,
    LoProc = 0x70000000,
    HiProc = 0x7fffffff
}

public struct ElfProgramHeader
{
    public ElfPH_SegmentType SegmentType;
    public ElfFlags flags;
    public ulong p_offset;
    public ulong p_vaddr;
    public ulong p_paddr;
    public ulong p_filesz;
    public ulong p_memsz;
    public ulong alignment;
}

public struct ElfSegment
{
    public uint Name;
    public uint Type;
    public ulong Flags;
    public ulong Addr;
    public ulong Offset;
    public ulong Size;
    public uint Link;
    public uint Info;
    public ulong AddrAlign;
    public ulong EntriesSize;
}

public struct ELfSym {
    public uint Name;
    public byte Info;
    public byte Other;
    public ushort Shndx;
    public ulong Value;
    public ulong Size;
}
