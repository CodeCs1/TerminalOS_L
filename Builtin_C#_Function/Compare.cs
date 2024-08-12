using System;
using System.Text;

namespace TerminalOS_L {
    namespace CsRunner {
        public class Compare : AliasRun {
            public Compare(string name) : base(name) {}
            private static bool IsDigit(string str) {
                foreach(char c in str) {
                    if (!Char.IsDigit(c)) {
                        return false;
                    }
                }
                return true;
            }
            private static bool IsAlpha(string str) {
                foreach (char c in str) {
                    if (!char.IsLetter(c)) {
                        return false;
                    }
                }
                return true;
            }
            private static bool StrToBool(string str) {
                switch(str) {
                    case "True":
                        return true;
                    case "False":
                        return false;
                    default:
                        break;
                }
                return false;   
            }
            public override object ARun(string[] args)
            {
                // args[0] -> value
                // args[1] -> Operators
                // args[2] -> value
                bool is_ok =false;
                if (IsDigit(args[0]) && IsDigit(args[2])) {
                    int first = Convert.ToInt32(args[0]);
                    int last = Convert.ToInt32(args[2]);
                    switch(args[1]) {
                        case "<":
                            is_ok=first<last;
                            break;
                        case ">":
                            is_ok = first>last;
                            break;
                        case "<=":
                            is_ok = first <= last;
                            break;
                        case ">=":
                            is_ok = first >= last;
                            break;
                        case "==":
                            is_ok = first == last;
                            break;
                    } 
                } else if (IsAlpha(args[0]) && IsAlpha(args[2])) {
                }

                return is_ok;
            }
        }
    }
}