using System;

namespace TerminalOS_L {
    namespace CsRunner {
        public class ClearScr:AliasRun {
            public ClearScr(string name):base(name){}
            public override object ARun(string[] args)
            {
                Console.Clear();
                return null;
            }
        }

    }
}