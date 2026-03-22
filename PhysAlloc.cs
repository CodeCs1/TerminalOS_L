using Cosmos.Kernel.Core.Memory;
namespace TerminalOS_Lgen3
{
    public unsafe class PhysAlloc {
        public static byte* AllocPhys(ulong size) {
            void* p = PageAllocator.AllocPages(PageType.Unmanaged, size, true);
            return PageAllocator.GetPagePtr(p)-0xFFFF800000000000UL;
        }

        public static void FreePhys(void* ptr) {
            PageAllocator.Free((void*)((ulong)ptr+0xFFFF800000000000UL));
        }
    }
}