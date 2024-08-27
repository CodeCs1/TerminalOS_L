#!/bin/sh

dotnet build
if [ $? -ne 0 ]; then
    exit 1
fi

pushd ./bin/cosmos/Debug/net6.0
if [[ ! -d "iso" ]]; then
	mkdir -p iso/boot/grub
fi
cp ./TerminalOS_L.bin ./iso/boot/TerminalOS_L.bin

echo "set default = 0" > iso/boot/grub/grub.cfg
echo "set timeout = 0" > iso/boot/grub/grub.cfg
echo "menuentry \"TerminalOS_L C#\" {" > iso/boot/grub/grub.cfg
echo "	multiboot2 /boot/TerminalOS_L.bin" > iso/boot/grub/grub.cfg
echo "	boot" > iso/boot/grub/grub.cfg
echo "}" > iso/boot/grub/grub.cfg

if [ $? -ne 0 ]; then
    exit 1
fi
grub-mkrescue -o TerminalOS_L.grub.iso ./iso
if [ $? -ne 0 ]; then
    exit 1
fi
popd
