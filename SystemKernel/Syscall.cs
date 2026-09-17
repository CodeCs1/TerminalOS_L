
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using Cosmos.Kernel.System.Diagnostics;

namespace TerminalOS_Lgen3.SystemKernel;

public partial class Syscall
{
    [LibraryImport("*", EntryPoint = "rdmsr")]
    private static partial ulong rdmsr(uint msr);

    [LibraryImport("*", EntryPoint = "wrmsr")]
    private static partial void wrmsr(uint msr, ulong value);

    [LibraryImport("*", EntryPoint = "load_syscall")]
    private static partial void load_syscall();

    public static bool IsSupport
    {
        get
        {
            var (eax, _, _, _) = X86Base.CpuId(unchecked((int)0x80000000), 0);
            Log.WriteString($"[SYSCALL] eax: 0x{(uint)eax:x86}\n");
            if ((uint)eax < 0x80000001) return false;
            var (_, _, _, edx) = X86Base.CpuId(unchecked((int)0x80000001), 0);
            const int SysCallBitMask = 1 << 11;
            return (edx & SysCallBitMask) != 0;
        }
    }

    public static bool Init() {
        if (!IsSupport) return false;

        ulong efer = rdmsr(0xC0000080);
        wrmsr(0xC0000080, efer | (1u << 0));

        ulong star = ((ulong)0x30 << 48) | ((ulong)0x28 << 32);
        wrmsr(0xC0000081, star);

        load_syscall();

        return true;
    }

}
