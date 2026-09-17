.intel_syntax noprefix

.global read_cr3
read_cr3:
    mov rax, cr3
    ret
