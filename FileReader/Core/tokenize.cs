using System;
using System.Collections.Generic;
using System.Linq;
using XSharp.Assembler.x86;

namespace TerminalOS_L {
    namespace CFGRunner {
    namespace Core {
            public class Tokenize {
                private readonly string code_based;
                public struct Token_sct {
                    public Type type;
                    public string value;
                }
                private readonly List<Token_sct> token;
                public enum Type {
                    LEFT_PAREN,
                    RIGHT_PAREN,
                    EOF
                };
                public Tokenize(string code) {
                    token = new List<Token_sct>();
                    code_based = code;
                }
                public void Add(string str, Type t) {
                    Console.WriteLine("Added");
                    Token_sct sct;
                    sct.type=t;
                    sct.value=str;
                    
                    token.Add(sct);

                    Console.WriteLine("Passed");
                }
                public void Token() {

                    string[] spl = code_based.Split('\n');

                    foreach (string code in spl) {
                        string token_str="";
                        foreach(char c in code_based) {
                            switch(c) {
                                case '(':
                                    Console.WriteLine("OK");
                                    token_str = (string)token_str.Append(c);
                                    Add(token_str,Type.LEFT_PAREN);
                                    break;
                                case ')':
                                    Console.WriteLine("OK2");
                                    token_str = (string)token_str.Append(c);
                                    Add(token_str,Type.RIGHT_PAREN);
                                    break;
                            }
                        }
                    }
                    Add(null,Type.EOF);

                    foreach(Token_sct t in token) {
                        Console.WriteLine($"{t.type} {t.value} null");
                    }
                }
            }
        }
    }
}