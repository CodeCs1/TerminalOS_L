using System.Runtime.InteropServices;
namespace TerminalOS_Lgen3.SystemKernel;

public partial class UserMode {
    [LibraryImport("*", EntryPoint = "jump2ring3")]
    private static partial void JumpToRing3();
    public static void ToRing3() {
        JumpToRing3();
    }
}
