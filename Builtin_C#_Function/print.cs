using System;
using System.Text;

namespace TerminalOS_L {
    namespace CsRunner {
        public class Print : AliasRun {
            public Print(string name):base(name) {}
            public override object ARun(string[] args)
            {
                StringBuilder b=new StringBuilder();
                foreach(string text in args) {
                    b.Append(text);
                    
                    b.Append(' ');
                }
                b.Length -=1;
                Console.Write(b);
                return 0;
            }
        }
    }
}