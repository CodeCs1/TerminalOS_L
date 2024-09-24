using System;
using Cosmos.System.Graphics;
using TerminalOS_L.FrameBuffer;
using TerminalOS_L.Windows;

namespace TerminalOS_L {
    public class MSExe : ApplicationForm {
        public MSExe(Canvas root,
            string title,int width, int height,int x=0, int y=0):
        base(root,title,width,height,x,y) {
            
        }


        private static void Click1() {
            Cosmos.System.Power.Reboot();
        }

        public override void Update()
        {
            WMButton b=new("Test1",4,5,30,30);
            b.click?.Invoke();
        }
    }
}