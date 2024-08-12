using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalOS_L.CsRunner;
namespace TerminalOS_L {
    namespace CFGRunner {
        public class CFGRunner {
            private readonly List<AliasRun> CsCode;
            public readonly List<object> Variable;
            private  bool loop_on=false;
            private int loop_time=0;
            public CFGRunner() {
                CsCode = new List<AliasRun>
                {
                    new ReadLine("readline"),
                    new Print("print"),
                    new Println("println"),
                    new Compare("compare"),
                    new ClearScr("clearscr"),
                    new Calc("_calc_"),
                    //Cosmos builtin
                    new GetCPU_C("cosmos_get_cpu"),
                    new GetMem_C("cosmos_get_mem"),
                    //Graphics
                    new GraphicsInit("grapsinit")
                };
                Variable = new List<object>();  
            }
            //n^2
            public object Run(string command) {
                string[] spl = command.Split(' ');
                bool savevar = false;
                List<string> args = new ();
                string functioncall;
                loop_time=0;
                switch (spl[0])
                {
                    case "retf":
                        functioncall = spl[1];
                        savevar = true;
                        break;
                    case "loop":
                        loop_time = Convert.ToInt32(spl[1]);
                        loop_on = true;
                        Console.WriteLine($"LOOP_TIME: {loop_time}");
                        return null;
                    default:
                        functioncall = spl[0];
                        break;
                }

                int count=0;
                foreach(string arg in spl) {
                    string repl = arg;
                    //check for string literial

                    if (arg[count] == '"') {
                        StringBuilder builder = new ();
                        count++;
                        Console.WriteLine(args[count]);
                        while(count < arg.Length) {
                            if (arg[count] == '"') break;
                            builder.Append(arg[count]);
                            count++;
                        }
                        if (count < arg.Length) {
                            Console.WriteLine("String: "+builder.ToString()+" !");
                        } else {
                            Console.WriteLine("ERROR: Unterminated string.");
                            return 12;
                        }
                    }

                    if (count != 0 && !savevar) {
                        if (Variable.ToArray().Length != 0) {
                            repl = repl.Replace("$!", Variable.Last().ToString());
                            repl = repl.Replace("$#", Variable.First().ToString());
                            for (int i=0;i<Variable.ToArray().Length;i++) {
                                if (repl.Contains($"${i}")) {
                                    repl=repl.Replace($"${i}",Variable[i].ToString());
                                }
                            }
                        }
                        args.Add(repl);
                    } else if (count > 1 && savevar) {
                        if (Variable.ToArray().Length != 0) {
                            repl = repl.Replace("$!", Variable.Last().ToString());
                            repl = repl.Replace("$#", Variable.First().ToString());
                            for (int i=0;i<Variable.ToArray().Length;i++) {
                                if (repl.Contains($"${i}")) {
                                    repl=repl.Replace($"${i}",Variable[i].ToString());
                                }
                            }
                        }
                        args.Add(repl);
                    }
                    ++count;
                }

                if (loop_on==true) {
                    int counter = 0;
                    while (counter < loop_time) {
                        foreach (AliasRun c in CsCode) {
                            //Console.WriteLine("FUNCTIONCALL: "+functioncall);
                            if (c.commandname == functioncall && !savevar) {
                                c.ARun(args.ToArray());
                            } else if (c.commandname == functioncall && savevar) {
                                Variable.Add(c.ARun(args.ToArray()));
                            }
                        }
                        counter++;
                    }

                    loop_on=false;
                } else {

                    //Console.WriteLine("NOTOK");
                    foreach (AliasRun c in CsCode) {
                        if (c.commandname == functioncall && !savevar) {
                            c.ARun(args.ToArray());
                        } else if (c.commandname == functioncall && savevar) {
                            Variable.Add(c.ARun(args.ToArray()));
                        }
                    }
                }
                return 127;
            }
        }
    };
}