using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using SharpDX;
using SharpDX.Direct2D1;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using Format = SharpDX.DXGI.Format;
using SharpDX.DirectWrite;
using System.Threading;
using PoEFlasks;

namespace External_Overlay
{
    public partial class Form1 : Form, IMessageFilter
    {
        int xResolution = 1920;
        int yResolution = 1080;
        List<Flask> Flasks = new List<Flask>();
        List<double[]> locMods = new List<double[]>();

        private WindowRenderTarget device;
        private HwndRenderTargetProperties renderProperties;
        private SolidColorBrush solidColorBrush;
        private Factory factory;

        //text fonts
        private TextFormat font, fontSmall;
        private FontFactory fontFactory;
        private const string fontFamily = "Arial";//you can edit this of course
        private const float fontSize = 12.0f;
        private const float fontSizeSmall = 10.0f;

        private IntPtr handle;
        private Thread sDX = null;
        //DllImports
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);
        //Styles
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        public static IntPtr HWND_TOPMOST = new IntPtr(-1);

        public Form1()
        {
            Application.AddMessageFilter(this);
            this.handle = Handle;
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            OnResize(null);

            InitializeComponent();
           
            locMods.Add(new double[2] { 0.22552083333, 0.95370370 });
            Flask f1 = new Flask("ToH", Keys.D1);
            Flasks.Add(f1);
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();

        }
        public bool PreFilterMessage(ref Message m)
        {
            //here you can specify  which key you need to filter
            for (int i = 0; i <= 5; i++)
            {
                if (Flasks[0].visible == true)
                {
                    if (m.Msg == 0x0100 && (Keys)m.WParam.ToInt32() == Flasks[0].key)
                    {
                        //do flask stuff
                        this.label2.Text = "flask used";
                        return true;
                    }
                    else
                    {
                        return false;
                        this.label2.Text = "flask not used";
                    }
                }
            }
            return false;
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //code to check color at a position
                /* this.Invoke((MethodInvoker)delegate {
                        this.Cursor = new Cursor(Cursor.Current.Handle);
                        this.label2.Text = Cursor.Position.ToString();
                        this.label1.Text = GetColorFromScreen(new Point(Cursor.Position.X, Cursor.Position.Y)).ToString();
                    });*/
                for (int i = 0; i <= 0; i++)
                {
                    if (Flasks[i].visible)
                    {
                        if (Flasks[i].name == "ToH")
                        {
                            //check to make sure its at least 1/2 full
                            Point p = new Point((int)(locMods[i][0] * xResolution), (int)(locMods[i][1] * yResolution));
                            if (GetColorFromScreen(p).R > 50 && GetColorFromScreen(p).G > 50 && GetColorFromScreen(p).B > 50)
                            {
                                Flasks[i].usable = true;
                            }
                            else
                                Flasks[i].usable = false;
                        }
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.label1.Text = Flasks[i].visible.ToString();
                        });
                    }
                }
            }
        }
        static Color GetColorFromScreen(Point p)
        {
            Rectangle rect = new Rectangle(p, new Size(2, 2));

            System.Drawing.Bitmap map = CaptureFromScreen(rect);

            Color c = map.GetPixel(0, 0);

            map.Dispose();

            return c;
        }
        static System.Drawing.Bitmap CaptureFromScreen(Rectangle rect)
        {
            System.Drawing.Bitmap bmpScreenCapture = null;

            if (rect == Rectangle.Empty)//capture the whole screen
            {
                bmpScreenCapture = new System.Drawing.Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            }
            else // just the rect
            {
                bmpScreenCapture = new System.Drawing.Bitmap(rect.Width, rect.Height);
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
        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.Width = 1920;// set your own size
            this.Height = 1080;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |// this reduce the flicker
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.Opaque |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            this.TopMost = true;
            this.Visible = true;

            factory = new Factory();
            fontFactory = new FontFactory();
            renderProperties = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(1920, 1080),
                PresentOptions = PresentOptions.None
            };

            //SetLayeredWindowAttributes(this.Handle, 0, 255, Managed.LWA_ALPHA);// caution directx error

            //Init DirectX
            device = new WindowRenderTarget(factory, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), renderProperties);
            SharpDX.Mathematics.Interop.RawColor4 r4color = new SharpDX.Mathematics.Interop.RawColor4();
            r4color.A = 255;
            r4color.R = 122;
            r4color.G = 122;
            r4color.B = 122;
            solidColorBrush = new SolidColorBrush(device, r4color);
            // Init font's
            font = new TextFormat(fontFactory, fontFamily, fontSize);
            fontSmall = new TextFormat(fontFactory, fontFamily, fontSizeSmall);
            //line = new device.DrawLine;

            sDX = new Thread(new ParameterizedThreadStart(sDXThread));

            sDX.Priority = ThreadPriority.Highest;
            sDX.IsBackground = true;
            sDX.Start();
        }
        protected override void OnPaint(PaintEventArgs e)// create the whole form
        {
            int[] marg = new int[] { 0, 0, Width, Height };
            DwmExtendFrameIntoClientArea(this.Handle, ref marg);
        }

        private void sDXThread(object sender)
        {
            while (true)
            {
                device.BeginDraw();
                SharpDX.Mathematics.Interop.RawColor4 r4color = new SharpDX.Mathematics.Interop.RawColor4();
                r4color.A = 0;
                r4color.R = 255;
                r4color.G = 255;
                r4color.B = 255;
                device.Clear(r4color);
                device.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Aliased;// you can set another text mode
                Ellipse el = new Ellipse();
                SharpDX.Mathematics.Interop.RawRectangleF rec = new SharpDX.Mathematics.Interop.RawRectangleF();
                rec.Bottom = 0;
                rec.Top = 1080;
                rec.Right = 1920;
                rec.Left = 0;
                SharpDX.Mathematics.Interop.RawColor4 r4color2 = new SharpDX.Mathematics.Interop.RawColor4();
                r4color2.R = 125;
                r4color2.G = 125;
                r4color2.B = 125;
                solidColorBrush.Color = r4color2;
                device.DrawText("asdf", font, rec, solidColorBrush);
                //place your rendering things here
                device.DrawText("ASDF", font, rec, solidColorBrush);
                device.EndDraw();
            }

            //whatever you want
        }
    }
}
