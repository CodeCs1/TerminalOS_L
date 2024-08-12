using System;

namespace TerminalOS_L {
    namespace CsRunner {
        public class ReadLine:AliasRun {
            public ReadLine(string name) :base(name) {}
            public override object ARun(string[] args)
            {
                string res = Console.ReadLine();
                return res;
            }
        }
    }
}