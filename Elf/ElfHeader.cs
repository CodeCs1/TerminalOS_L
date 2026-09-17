using System.Text.Json;
using System.Text.Json.Serialization;

namespace TerminalOS_Lgen3.Elf;

public enum ElfCpuType : byte
{
    x32 = 1,
    x64 = 2,
}

public enum ElfType : ushort
{
    relocatable = 1,
    executable = 2,
    shared = 3,
    core = 4
}
public enum ElfEndian : byte
{
    little = 1,
    big = 2,
}

public enum ElfInstructionSet : ushort
{
    None = 0x00,
    Sparc = 0x02,
    x86 = 0x03,
    MIPS = 0x08,
    PA_RISC = 0x0F,
    PowerPC32 = 0x14,
    PowerPC64 = 0x15,
    S390 = 0x16,
    ARM = 0x28,
    Alpha = 0x29,
    SuperH = 0x2A,
    IA_64 = 0x32,
    x86_64 = 0x3E,
    AArch64 = 0xB7,
    RISC_V = 0xF3,
}

public enum ElfFlags : uint
{
    Executable = 1,
    Writable = 2,
    Reloadable = 4
}

[JsonConverter(typeof(ElfHeaderConverter))]
public unsafe struct ElfHeader
{
    [JsonPropertyName("Signature")]
    public fixed byte signature[4];
    public ElfCpuType CpuArchitecture;
    public ElfEndian Endian;
    public byte ElfHeaderVersion;
    public byte OSABI;
    public ulong reserved;
    public ElfType type;
    public ElfInstructionSet InstructionSet;
    public ushort ELfVersion;
    public ulong program_entry_offset;
    public ulong program_header_offset;
    public ulong section_header_offset;
    public uint flags;
    public ushort elf_header_size;
    public ushort size_of_prog_entries;
    public ushort num_of_prog_entries;
    public ushort size_of_sect_entries;
    public ushort num_of_sect_entries;
    public ushort section_idx;
    public override readonly string ToString()
    {
        return JsonSerializer.Serialize(this, ElfCtx.Default.ElfHeader);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields =true)]
[JsonSerializable(typeof(ElfHeader))]
public partial class ElfCtx : JsonSerializerContext {}


public sealed class ElfHeaderConverter : JsonConverter<ElfHeader>
{
    public override ElfHeader Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ElfHeader hdr = default;
        return hdr;
    }

    public override unsafe void Write(Utf8JsonWriter writer, ElfHeader value, JsonSerializerOptions options)
    {
        byte* hdr = value.signature;
        ReadOnlySpan<byte> sourceSpan = new(hdr, 4);
        string json = $$"""
            {
               "signature": "{{Convert.ToBase64String(sourceSpan)}}",
               "architecture": "{{value.CpuArchitecture}}",
               "header_version": {{value.ElfHeaderVersion}},
               "os_abi": "{{value.OSABI}}",
               "prog_entry_offset": "0x{{value.program_entry_offset:x}}",
               "prog_hdr_offset": "0x{{value.program_header_offset:x}}",
               "section_hdr_offset": "0x{{value.section_header_offset:x}}",
               "size_of_prog_entry": {{value.size_of_prog_entries}},
               "size_of_sect_entry": {{value.size_of_sect_entries}}
            }
        """;
        writer.WriteRawValue(json);
    }
}
