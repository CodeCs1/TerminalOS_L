This folder contain custom nativeaot-patcher build packages.
Since the nativeaot-patcher use efi `GetTime()` function if the Kernel detect UEFI (real) system, which could get PageFault.
(limine doesn't let user touch to some reserved memory address after boot into kernel file)
