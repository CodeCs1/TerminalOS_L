using System;
using System.Collections.Generic;
using IL2CPU.API.Attribs;
using XSharp;
using XSharp.Assembler;
using XSharp.Assembler.x86;
using XSharp.x86;

namespace TerminalOS_L.CosmosPort {
    class LoadPageAsm : AssemblerMethod {
        public override void AssembleNew(Assembler aAssembler, object aMethodInfo)
        {
            _ = new Push
            {
                DestinationReg = RegistersEnum.EBP
            };
            XS.Set(XSRegisters.EAX, XSRegisters.EBP, sourceDisplacement: 8);
            XS.Set(XSRegisters.CR3, XSRegisters.EAX);
            XS.Set(XSRegisters.ESP, XSRegisters.EBP);
            _ = new Pop {
                DestinationReg = RegistersEnum.EBP
            };
        }
    }
    /*        
    push %ebp
    mov %esp, %ebp
    mov %cr0, %eax
    or $0x80000000, %eax
    mov %eax, %cr0
    mov %ebp, %esp
    pop %ebp
    */
    class EnableAsm: AssemblerMethod {
        public override void AssembleNew(Assembler aAssembler, object aMethodInfo)
        {
            _ = new Push {
                DestinationReg=RegistersEnum.EBP
            };
            XS.Set(XSRegisters.EBP, XSRegisters.ESP);
            XS.Set(XSRegisters.EAX, XSRegisters.CR0);
            _ = new Or {
                DestinationReg = RegistersEnum.EAX,
                SourceValue = 0x80000000,
            };
            XS.Set(XSRegisters.CR0, XSRegisters.EAX);
            XS.Set(XSRegisters.ESP, XSRegisters.EBP);
            _ = new Pop {
                DestinationReg = RegistersEnum.EBP
            };
        }
    }
    [Plug(Target = typeof(Paging))]
    public unsafe class PagingImpl {
        internal static void Enable() {
            throw new NotImplementedException("Why not ?");
        }
        internal static void LoadPage(uint* addr) {
            throw new NotImplementedException("Why not ?");
        }
    }
    public unsafe class Paging {
        public unsafe struct PagingAlt {
            public fixed uint PagingDir[1024];
            public fixed uint FirstPage[1024];
        }
        public static void Init() {
            PagingAlt alt = new();
            for (int i=0;i<1024;i++) {
                alt.PagingDir[i] = 0x00000002;
            }
            for (uint i=0;i<1024;i++) {
                alt.FirstPage[i] = (i * 0x1000) | 3;
            }
            alt.PagingDir[0] = ((uint)alt.FirstPage) | 3;
            LoadPage(alt.PagingDir);
            Enable();

        }

        /// <summary>
        /// Load a Page directory.
        /// Assembly Plugged.
        /// </summary>

        internal static void LoadPage(uint* addr) {
            throw new NotImplementedException();
        }
        internal static void Enable() {
            throw new NotImplementedException();
        }
    }
}