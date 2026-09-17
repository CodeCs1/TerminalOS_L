#include "include/types.h"
#include "include/tss.h"

struct gdt_entry {
    u16 limit_low;
    u16 base_low;
    u8  base_middle;
    u8  access;
    u8  granularity;
    u8  base_high;
}__attribute__((packed)) ;

struct gdt_ptr {
    u16 limit;
    u64 base;
}__attribute__((packed));

static u64 gdt[11];
static struct gdt_ptr   gdt_pointer = {
    .limit = (sizeof(gdt) - 1),
    .base = (u64)&gdt[0],
};



#define GDT_ENTRY(base, limit, access, flags)                              \
    ( (u64)((limit)  & 0xFFFFull)                                     \
        | (u64)(((base)  & 0xFFFFFFull)      << 16)                       \
        | (u64)(((access)& 0xFFull)          << 40)                       \
        | (u64)((((limit)>> 16) & 0xFull)    << 48)                       \
        | (u64)(((flags) & 0xFull)           << 52)                       \
        | (u64)((((base) >> 24) & 0xFFull)   << 56) )

#define TSS_DESC_LOW(base, limit, access, flags) \
    GDT_ENTRY((u32)(base), (limit), (access), (flags))
#define TSS_DESC_HIGH(base) \
        ((u64)(((u64)(base)) >> 32))

void gdt_flush() {
    __asm__ volatile ("lgdt (%0)" : : "r"(&gdt_pointer) : "memory");
    __asm__ volatile (
        "mov    %[ds], %%ax\n\t"
        "mov    %%ax, %%ds\n\t"
        "mov    %%ax, %%es\n\t"
        "mov    %%ax, %%fs\n\t"
        "mov    %%ax, %%gs\n\t"
        "mov    %%ax, %%ss\n\t"
        "pushq  %[cs]\n\t"
        "lea    1f(%%rip), %%rax\n\t"
        "pushq  %%rax\n\t"
        "lretq\n\t"
        "1:\n\t"
        :
        : [cs] "i" (0x28), [ds] "i" (0x30)
        : "rax", "memory"
    );
}


#define KSTACK0_SIZE 16384
#define IST1_SIZE    16384
static u8 __attribute__((aligned(16))) g_kstack0[KSTACK0_SIZE];
static u8 __attribute__((aligned(16))) g_ist1[IST1_SIZE];
static tss_t tss;


typedef struct cpu_local_t {
    u64 kernel_rsp;
    u64 user_rsp;
}__attribute__((packed)) cpu_local;
cpu_local cpu_0;

//GDT_ENTRY(base, limit, access, flags)
void load_gdt() {


    for (u64 i = 0; i < sizeof(tss_t); i++) ((u8 *)&tss)[i] = 0;
    tss.rsp0 = (u64)(g_kstack0+KSTACK0_SIZE);
    tss.ist1 = (u64)(g_ist1+IST1_SIZE);
    tss.iopb_offset = sizeof(tss_t);

    cpu_0.kernel_rsp = tss.rsp0;
    cpu_0.user_rsp   = 0;

    /* Limine default GDT */
    gdt[0] = GDT_ENTRY(0, 0,        0x00, 0x0); /* null              */
    gdt[1] = GDT_ENTRY(0, 0xFFFF,   0x9A, 0x0); /* 16-bit kernel code (0x08)*/
    gdt[2] = GDT_ENTRY(0, 0xFFFF,   0x92, 0x0); /* 16-bit kernel data (0x10)*/
    gdt[3] = GDT_ENTRY(0, 0xffffffff,  0x9A, 0xC); /* 32-bit kernel code (0x18)*/
    gdt[4] = GDT_ENTRY(0, 0xffffffff,  0x92, 0xC); /* 32-bit kernel data (0x20)*/
    gdt[5] = GDT_ENTRY(0, 0xffffffff,  0x9A, 0xA); /* 64-bit kernel code (0x28)*/
    gdt[6] = GDT_ENTRY(0, 0xffffffff,  0x92, 0xC); /* 64-bit kernel data (0x30)*/

    /* User GDT */
    gdt[7] = GDT_ENTRY(0, 0xffffffff,  0xf2, 0xc); /* 64-bit user code (0x28)*/
    gdt[8] = GDT_ENTRY(0, 0xffffffff,  0xfa, 0xa); /* 64-bit user data (0x30)*/

    gdt[9] = TSS_DESC_LOW((u64)&(tss), (sizeof(tss)-1),0x89,0);
    gdt[10] = TSS_DESC_HIGH((u64)&(tss));

    __asm__("cli");
    gdt_flush();
    __asm__ volatile ("ltr %%ax" : : "a"(0x48) : "memory");
    __asm__("sti");
}
