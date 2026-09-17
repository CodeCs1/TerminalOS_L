/* ELena™ File executable :D */
using System.Runtime.InteropServices;
using Cosmos.Kernel.System.Diagnostics;

namespace TerminalOS_Lgen3.Elf
{
    public class Elf {
        private ElfHeader hdr;
        private readonly MemoryStream memoryStream;
        public Elf(string file_path)
        {
            memoryStream = new();
            using FileStream fileStream = File.OpenRead(file_path);
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            byte[] elf_hdr = new byte[64];
            memoryStream.Read(elf_hdr, 0, 64);
            ReadOnlySpan<ElfHeader> elfHeaders = MemoryMarshal.Cast<byte, ElfHeader>(elf_hdr);
            hdr = elfHeaders[0];
            Log.Write($"[ELF] ELF Content:\n{hdr}\n");
        }
        public List<ElfProgramHeader> GetProgramHdr()
        {
            var elf_list = new List<ElfProgramHeader>();
            byte[] elf_prog = new byte[hdr.size_of_prog_entries];
            memoryStream.Position = (long)hdr.program_header_offset;
            for (uint i = 0; i < hdr.num_of_prog_entries; i++)
            {
                memoryStream.Read(elf_prog, 0, hdr.size_of_prog_entries);
                ReadOnlySpan<ElfProgramHeader> elfProgramHeaders = MemoryMarshal.Cast<byte, ElfProgramHeader>(elf_prog);
                elf_list.Add(elfProgramHeaders[0]);
            }
            return elf_list;
        }
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        delegate void Execute();
        public void Run() {
            foreach (var data in GetProgramHdr())
            {
                if (data.SegmentType == ElfPH_SegmentType.Load)
                {
                    //TODO: Need to map memory to page, after setting up user mode.
                }
            }

            var run = Marshal.GetDelegateForFunctionPointer<Execute>(
                new IntPtr((int)hdr.program_entry_offset));
            run?.Invoke();
        }
        public List<ElfSegment> GetSegments() {
            var elf_list = new List<ElfSegment>();

            byte[] elf_seg = new byte[hdr.size_of_sect_entries];
            memoryStream.Position = (long)hdr.section_header_offset;

            for (uint i = 0; i < hdr.num_of_prog_entries; i++)
            {
                memoryStream.Read(elf_seg, 0, hdr.size_of_prog_entries);
                ReadOnlySpan<ElfSegment> elfSegments = MemoryMarshal.Cast<byte, ElfSegment>(elf_seg);
                elf_list.Add(elfSegments[0]);
            }
            return elf_list;
        }
        public unsafe bool ValidateHeader()
        {
            fixed (byte* sig = hdr.signature)
            {
                ReadOnlySpan<byte> span = new(sig, 4);
                Log.Write($"[ELF] Got header: 0x{span[0]:X}, 0x{span[1]:X}, 0x{span[2]:X} 0x{span[3]:X}\n");
                return span is [0x7f, 0x45, 0x4c, 0x46]; // 0x7f and ELF
            }
        }
    }
}
/* Fun fact: Elena is a character from Trickcal: Chibi Go! */
