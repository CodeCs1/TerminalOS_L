#!/bin/sh
dotnet build
if [ $? -ne 0 ]; then
    exit 1
fi

pushd ./bin/cosmos/Debug/net6.0
cp ./TerminalOS_L.bin ./iso/boot/TerminalOS_L.bin
if [ $? -ne 0 ]; then
    exit 1
fi
grub-mkrescue -o TerminalOS_L.grub.iso ./iso
if [ $? -ne 0 ]; then
    exit 1
fi
popd