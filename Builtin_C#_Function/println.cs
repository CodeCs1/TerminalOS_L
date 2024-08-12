using System;
using System.Text;

namespace TerminalOS_L {
    namespace CsRunner {
        public class Println : AliasRun {
            public Println(string name):base(name) {}
            public override object ARun(string[] args)
            {
                if (args.Length == 0) {
                    Console.WriteLine();
                    return 0;
                }
                StringBuilder b=new StringBuilder();
                foreach(string text in args) {
                    b.Append(text);
                    
                    b.Append(' ');
                }
                b.Length -=1;
                Console.WriteLine(b);
                return 0;
            }
        }
    }
}