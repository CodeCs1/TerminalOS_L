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
        public static Canvas can;
        public ApplicationForm(Canvas Root,string title,
            int width, int height, int x=0,int y=0) {
                this.x=x;
                this.y=y;
                this.width=width;
                this.height=height;
                this.title=title;
                can = Root;
            }

        public class EventClick {
            public delegate void Click();
        }

        public class WMButton : Component {
            public int x,y,w,h;
            public string text;
            public WMButton(string text, int x, int y, int w, int h) {
                this.x=x;
                this.y=y;
                this.w=w;
                this.h=h;
                this.text = text;
            }
            public override void Draw()
            {
                can.DrawRectangle(Color.Black,x,y,w,h);
            }

            public EventClick.Click click;
        }

        ///<summary>
        /// Component for Application Form.
        ///</summary>

        public class Component {
            public virtual void Draw() {}
        }

        public void Init() {
            /*Window*/
            can.DrawFilledRectangle(Color.FromArgb(5592575),x,y,width,height-50);
            //can.DrawFilledRectangle(Color.FromArgb(16777045),0,20,width,20);
            can.DrawFilledRectangle(Color.White,x,y+20,width,height-70);
            /*Button Bar*/
            can.DrawFilledRectangle(Color.White,x,y,20,20);
            can.DrawFilledRectangle(Color.White,width-20,0,20,20);
            /*Title*/
            int TextLocation = (width/2)-title.Length;
            can.DrawString(title,PCScreenFont.Default,Color.White,TextLocation,y);
            /*Line*/ 
            can.DrawLine(Color.Black,x,y+20,width,20);
            Update();
        }

        public virtual void Update() {}
    }
}