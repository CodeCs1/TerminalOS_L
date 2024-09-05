using System.Text;
using TerminalOS_L.FrameBuffer;

namespace TerminalOS_L {
    public class Echo : Command {
        public Echo(string name):base(name) {}
        public override string Execute(string[] args)
        {
            StringBuilder b=new();
            foreach(string text in args) {
                b.Append(text);
                b.Append(' ');
            }
            b.Length -=1;
            FrConsole.WriteLine(b.ToString());
            return "";
        }

    }
}