#!/bin/sh
cosmos run --debug &
gdb -x gdb_debug.txt bin/Debug/net10.0/linux-x64/TerminalOS_Lgen3.elf
killall qemu-system-x86_64
