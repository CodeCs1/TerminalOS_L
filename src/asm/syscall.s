.global syscall_entry_stub
.align 16
syscall_entry_stub:
    swapgs
    mov    %rsp, %gs:8
    mov    %gs:0, %rsp
    push   %rcx
    push   %r11
    push   %rax
    push   %rbx
    push   %rdx
    push   %rsi
    push   %rdi
    push   %rbp
    push   %r8
    push   %r9
    push   %r10
    push   %r12
    push   %r13
    push   %r14
    push   %r15
    mov    %rsp, %rdi

    call   syscall_dispatch
    pop    %r15
    pop    %r14
    pop    %r13
    pop    %r12
    pop    %r10
    pop    %r9
    pop    %r8
    pop    %rbp
    pop    %rdi
    pop    %rsi
    pop    %rdx
    pop    %rbx
    pop    %rax
    pop    %r11
    pop    %rcx
    mov    %gs:8, %rsp
    swapgs
    sysretq


.global test_syscall
test_syscall:
    mov $5, %rax
    syscall
    ret
