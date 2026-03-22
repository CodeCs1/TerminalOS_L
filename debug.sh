qemu-system-x86_64 -cdrom \
    bin/Debug/net10.0/linux-x64/cosmos/TerminalOS_Lgen3.iso \
    -drive id=disk,file=test.qcow2,if=none \
    -device ahci,id=ahci \
    -device ide-hd,drive=disk,bus=ahci.0 \
    -boot d \
    -serial file:output.log \
    -S -s &

gdb -x gdb_debug.txt bin/Debug/net10.0/linux-x64/TerminalOS_Lgen3.elf

killall qemu-system-x86_64
