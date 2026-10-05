.intel_syntax noprefix

test_ring3:
    jmp test_ring3

.global jump2ring3
jump2ring3:
    mov rcx, test_ring3
    mov r11, 0x202
    sysretq
