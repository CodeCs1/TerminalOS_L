using System;
using System.Drawing;
using Cosmos.System.Graphics;

namespace TerminalOS_L {
    namespace Graphics {
        public static class Graphics {
            public static Cosmos.HAL.Drivers.Video.SVGAII.VMWareSVGAII canvas;
            public static void Init(uint width,uint height,int r,int g,int b,uint colord) {
                canvas = new Cosmos.HAL.Drivers.Video.SVGAII.VMWareSVGAII();
                canvas.SetMode(width,height,colord);
            }
        }
    }
}