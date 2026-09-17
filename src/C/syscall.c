#include "include/types.h"

u64 rdmsr(u32 msr) {
    u32 lo, hi;
    __asm__ volatile ("rdmsr" : "=a"(lo), "=d"(hi) : "c"(msr));
    return ((u64)hi << 32) | lo;
}

void wrmsr(u32 msr, u64 value) {
    u32 lo = (u32)value;
    u32 hi = (u32)(value >> 32);
    __asm__ volatile ("wrmsr" : : "a"(lo), "d"(hi), "c"(msr) : "memory");
}

extern void syscall_entry_stub(void);

extern struct cpu_local_t cpu_0;

void load_syscall() {
    wrmsr(0xC0000082, (u64)&syscall_entry_stub);
    wrmsr(0xC0000083, (u64)&syscall_entry_stub);
    wrmsr(0xC0000084, (1u<<8) | (1u<<9) | (1u<<10) | (1u<<18));

    wrmsr(0xC0000101,0);
    wrmsr(0xC0000102,(u64)&cpu_0);
}

struct syscall_regs {
    u64 r15, r14, r13, r12, r10, r9, r8, rbp;
    u64 rdi, rsi, rdx, rbx, rax, r11, rcx;
};
extern void __cosmos_serial_write(const char* str);

void syscall_dispatch(struct syscall_regs *regs) {
    switch (regs->rax) {
        case 5:
            __cosmos_serial_write("\n\nPrint 2 screen\n");
            break;
        default:
            __cosmos_serial_write("\n\nSyscall Exit!\n");
            break;
    }
}
