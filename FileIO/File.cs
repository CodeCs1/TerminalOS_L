using Cosmos.Build.API.Attributes;

namespace TerminalOS_Lgen3.Plugs {
    [Plug(typeof(File))]
    public class FilePLugs {
        [PlugMember]
        public static byte[] ReadAllBytes(string path) {
            return [1];
        }
        [PlugMember]
        public static bool Exists(string? path) {
            return true;
        }
    }
}