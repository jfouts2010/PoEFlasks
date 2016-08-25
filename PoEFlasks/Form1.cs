using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace PoEFlasks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Task.Factory.StartNew(() => Start());
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //code to check color at a position
                  this.Invoke((MethodInvoker)delegate {
                     this.Cursor = new Cursor(Cursor.Current.Handle);
                     //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
                     this.label2.Text = Cursor.Position.ToString();
                     this.label1.Text = GetColorFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y)).ToString();
                 });
               /* if (GetColorFromScreen(new Point(472, 1014)).R > 50)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.label1.Text = "flask full";
                    });
                }
                else if (GetColorFromScreen(new Point(456, 1037)).R > 80)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.label1.Text = "flask half full";
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.label1.Text = "flask empty";
                    });
                }*/
            }
        }
        public void Start()
        {
            while (true)
            {
                //code to check color at a position
                /*  this.Invoke((MethodInvoker)delegate {
                     this.Cursor = new Cursor(Cursor.Current.Handle);
                     //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
                     this.label2.Text = Cursor.Position.ToString();
                     this.label1.Text = GetColorFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y)).ToString();
                 });*/
                if (GetColorFromScreen(new Point(472, 1014)).R > 50)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.label1.Text = "flask full";
                    });
                }
                else if (GetColorFromScreen(new Point(456, 1037)).R > 80)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.label1.Text = "flask half full";
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.label1.Text = "flask empty";
                    });
                }
                if (ModifierKeys != Keys.None)
                {
                    this.Invoke((MethodInvoker)delegate {
                        this.label2.Text = "used flask";
                    });
                }
            }
        }
        static Color GetColorFromScreen(Point p)
        {
            Rectangle rect = new Rectangle(p, new Size(2, 2));

            Bitmap map = CaptureFromScreen(rect);

            Color c = map.GetPixel(0, 0);

            map.Dispose();

            return c;
        }
        static Bitmap CaptureFromScreen(Rectangle rect)
        {
            Bitmap bmpScreenCapture = null;

            if (rect == Rectangle.Empty)//capture the whole screen
            {
                bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            }
            else // just the rect
            {
                bmpScreenCapture = new Bitmap(rect.Width, rect.Height);
            }

            Graphics p = Graphics.FromImage(bmpScreenCapture);

            if (rect == Rectangle.Empty)
            { // captuer the whole screen
                p.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                         Screen.PrimaryScreen.Bounds.Y,
                                         0, 0,
                                         bmpScreenCapture.Size,
                                         CopyPixelOperation.SourceCopy);

            }
            else // cut a spacific rectangle
            {
                p.CopyFromScreen(rect.X,
                         rect.Y,
                         0, 0,
                         rect.Size,
                         CopyPixelOperation.SourceCopy);

            }

            return bmpScreenCapture;
        }
    }
}
