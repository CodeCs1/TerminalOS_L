namespace TerminalOS_L {
    namespace CsRunner {
        public class GetMem_C : AliasRun {
            public GetMem_C(string name):base(name){}
            public override object ARun(string[] args)
            {                
                return Cosmos.Core.CPU.GetAmountOfRAM()+2;
            }
        }
    }
}