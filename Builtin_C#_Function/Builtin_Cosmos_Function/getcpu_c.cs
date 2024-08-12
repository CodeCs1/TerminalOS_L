namespace TerminalOS_L {
    namespace CsRunner {
        public class GetCPU_C : AliasRun {
            public GetCPU_C(string name):base(name){}
            public override object ARun(string[] args)
            {                
                return Cosmos.Core.CPU.GetCPUBrandString();
            }
        }
    }
}