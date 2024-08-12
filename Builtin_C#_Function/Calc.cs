using System;

namespace TerminalOS_L {
    namespace CsRunner {
        public class Calc : AliasRun {
            public Calc(string name):base(name) {}
            public override object ARun(string[] args)
            {
                string[] argsspl = args;
                if (args.Length == 0) {
                    argsspl=args[0].Split(' ');
                }

                int p1 = Convert.ToInt16(argsspl[0]);
                int p2 = Convert.ToInt16(argsspl[2]);

                Int64 res=0;

                switch(argsspl[1]) {
                    case "+":
                        res = p1+p2;
                        break;
                    case "-":
                        res = p1-p2;
                        break;
                    case "*":
                        res = p1*p2;
                        break;
                    case "/":
                        if (p2 == 0) {
                            return null; // handle the null one
                        }
                        res = p1/p2;
                        break;
                    default:
                        break;
                }

                return Convert.ToString(res);
            }
        }
    }
}