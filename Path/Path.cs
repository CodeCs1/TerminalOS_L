using Cosmos.Kernel.System.Vfs;
using PathResult = TerminalOS_Lgen3.Result<System.Action, string>;
namespace TerminalOS_Lgen3.Path
{
    public class Path
    {
        private static string Root = "/root";
        public static string CurrentPath = "/";
        private static readonly string AbsolutePath = Root + CurrentPath;
        public static string Format(string path)
        {
            if (path == "/") return Root;
            if (path == ".") return AbsolutePath;
            else if (path.StartsWith('/') && path.Length > 1) return Root + "/" + path;
            else return AbsolutePath + "/" + path;
        }
        public static PathResult ChangeDirectory(string path) {
            if (path.StartsWith('/')) CurrentPath = path;
            else CurrentPath += path;

            if (!Directory.Exists(AbsolutePath)) return PathResult.Error("Directory not exist!");

            return PathResult.Ok(() => { });
        }
        public static PathResult ChangeRoot(string new_path)
        {
            if (new_path == "/") return PathResult.Error("Invaild path");
            if (VfsManager.Mounts.FirstOrDefault(x => x.MountPoint == new_path) == null)
                return PathResult.Error($"Path `{new_path}` not mounted yet!");
            Root = new_path;
            return PathResult.Ok(() => { });
        }
    }
}
