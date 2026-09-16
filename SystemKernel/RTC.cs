using Cosmos.Build.API.Attributes;
using Cosmos.Kernel.System.Diagnostics;

namespace TerminalOS_Lgen3.SystemKernel;

[Plug(TargetName = "Cosmos.Kernel.HAL.X64.Devices.Clock.RTC")]
public class RTCImpl
{
    [PlugMember]
    public unsafe void Initialize()
    {
        Log.WriteString("[BOOT] Custom RTC Initialization!");
    }
}
