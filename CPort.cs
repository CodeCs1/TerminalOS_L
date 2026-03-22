using System.Runtime;
using Cosmos.Kernel.Core.Memory;
namespace TerminalOS_Lgen3 {
    public partial class CPorts {
        // Copy from original source code since this function is private.
        // https://github.com/valentinbreiz/nativeaot-patcher/blob/1992ce15c1abd6a34f6d310b635797066d329866/src/Cosmos.Kernel.Core/Memory/PageAllocator.cs#L10

        [RuntimeExport("Virt2Phys")]
        private static ulong VirtualToPhysical(ulong virtualAddress)
        {
            // Higher Half Kernel mapping: virtual addresses start with 0xFFFF8000
            // Remove the higher half offset to get physical address
            const ulong HigherHalfOffset = 0xFFFF800000000000UL;
            if (virtualAddress >= HigherHalfOffset)
            {
                return virtualAddress - HigherHalfOffset;
            }
            // If not in higher half mapping, assume it's already physical
            return virtualAddress;
        }
    }
}