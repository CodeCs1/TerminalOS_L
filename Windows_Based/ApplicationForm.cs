using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts; 
/*This Windows Form one is based on Windows 1.0 <Not Windows 10 or newer>
I want to keep thing simple.*/

namespace TerminalOS_L.Windows {
    public class ApplicationForm {
        public int x,y,width,height;
        public string title;
        public Canvas can;
        public ApplicationForm(Canvas Root,string title,
            int width, int height, int x=0,int y=0) {
                this.x=x;
                this.y=y;
                this.width=width;
                this.height=height;
                this.title=title;
                can = Root;
            }

        public void Init() {
            /*Window*/
            can.DrawFilledRectangle(Color.FromArgb(5592575),x,y,width,height-50);
            can.DrawFilledRectangle(Color.FromArgb(16777045),0,20,width,20);
            can.DrawFilledRectangle(Color.White,0,40,width,height-90);
            /*Button Bar*/
            can.DrawFilledRectangle(Color.White,0,0,20,20);
            can.DrawFilledRectangle(Color.White,width-20,0,20,20);
            /*Title*/
            for (int i=title.Length/2;i>0;i--) {
                can.DrawFilledRectangle(Color.Black,width/2-i,0,20,20);
            }
            for (int i=0;i<title.Length/2;i++) {
                can.DrawFilledRectangle(Color.Black,width/2+i,0,20,20);
            }
            /*Line*/ 
            can.DrawLine(Color.Black,0,20,width,20);
        }

        public virtual void Update() {}
    }
}