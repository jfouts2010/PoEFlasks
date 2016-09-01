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
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using System.Diagnostics;

namespace External_Overlay
{
    public partial class Form1 : Form, IMessageFilter
    {
        float xResolution = 1920;
        float yResolution = 1080;
        bool WCCD = false;
        float WCT = 2;
        bool showWC = false;
        double globalPercentage = 0;
        float flaskAlpha = 1.0f;
        float xAdjustment = 0;
        List<Flask> Flasks = new List<Flask>();
        List<double[]> locMods = new List<double[]>();
        private MouseKeyboardActivityMonitor.KeyboardHookListener keyboardListener;
        private MouseKeyboardActivityMonitor.MouseHookListener mouseListener;
        private WindowRenderTarget device;
        private HwndRenderTargetProperties renderProperties;
        private SolidColorBrush solidColorBrush;
        private Factory factory;
        System.Drawing.Bitmap screenPixel = new System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //text fonts
        private TextFormat font, fontSmall;
        private FontFactory fontFactory;
        private const string fontFamily = "Arial";//you can edit this of course
        private const float fontSize = 12.0f;
        private const float fontSizeSmall = 10.0f;

        private IntPtr handle;
        private Thread sDX = null;
        //DllImports
        /*[DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);*/

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

        public Form1(List<Flask> importedFlasks, int _globalPercentage, float _flaskAlpha, int _xAdjustment, float _xResolution, float _yResolution, bool _showWC)
        {
            showWC = _showWC;
            xResolution = _xResolution;
            yResolution = _yResolution;
            xAdjustment = _xAdjustment;
            flaskAlpha = _flaskAlpha;
            globalPercentage = _globalPercentage;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;
            Flasks = importedFlasks;
            Application.AddMessageFilter(this);
            this.handle = Handle;
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            OnResize(null);
            keyboardListener = new KeyboardHookListener(new GlobalHooker());
            keyboardListener.Enabled = true;
            keyboardListener.KeyPress += KeyboardListener_KeyDown;
            mouseListener = new MouseHookListener(new GlobalHooker());
            mouseListener.Enabled = true;
            mouseListener.MouseDown += MouseListener_KeyDown;
            InitializeComponent();

            locMods.Add(new double[2] { 0.1734375, 0.97407407 });
            locMods.Add(new double[2] { 0.197395833, 0.97407407 });
            locMods.Add(new double[2] { 0.2213541666, 0.97407407 });
            locMods.Add(new double[2] { 0.2453125, 0.97407407 });
            locMods.Add(new double[2] { 0.269270833, 0.97407407 });
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }
        private void KeyboardListener_KeyDown(object sender, KeyPressEventArgs e)
        {
            int q = e.KeyChar;
            if (q == (int)Keys2.q && WCCD == false)
            {
                WCCD = true;
                WCT = 2f;
            }
            for (int i = 0; i <= Flasks.Count() - 1; i++)
            {
                if (Flasks[i].visible == true)
                {

                    int s1 = e.KeyChar;
                    // int s2 = (int)Flasks[i].key;
                    if (s1 == (int)Flasks[i].key)
                    {
                        Point p = new Point((int)(Math.Round(locMods[i][0] * xResolution)), (int)(Math.Round(locMods[i][1] * yResolution)));
                        Color pixelColor = GetColorFromScreen(p);
                        if (pixelColor.R > 50 || pixelColor.G > 50 || pixelColor.B > 50)
                        {
                            //do flask stuff
                            Flasks[i].inUse = true;
                            Flasks[i].useDuration = (float)Flasks[i].baseDuration * (1 + (float)Flasks[i].qual / 100 + (float)globalPercentage / 100);
                        }
                    }

                }
            }

        }
        private void MouseListener_KeyDown(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Middle" || e.Button.ToString() == "XButton1" || e.Button.ToString() == "XButton2")
            {
                int s1 = 0;
                if (e.Button.ToString() == "Middle")
                    s1 = 4;
                else if (e.Button.ToString() == "XButton1")
                    s1 = 5;
                else if (e.Button.ToString() == "XButton2")
                    s1 = 6;
                for (int i = 0; i <= Flasks.Count() - 1; i++)
                {
                    if (Flasks[i].visible == true)
                    {

                        // int s2 = (int)Flasks[i].key;
                        if (s1 == (int)Flasks[i].key)
                        {
                            Point p = new Point((int)(Math.Round(locMods[i][0] * xResolution)), (int)(Math.Round(locMods[i][1] * yResolution)));
                            Color pixelColor = GetColorFromScreen(p);
                            if (pixelColor.R > 50 || pixelColor.G > 50 || pixelColor.B > 50)
                            {
                                //do flask stuff
                                Flasks[i].inUse = true;
                                Flasks[i].useDuration = (float)Flasks[i].baseDuration * (1 + (float)Flasks[i].qual / 100 + (float)globalPercentage / 100);
                            }
                        }

                    }
                }
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {


        }
        public bool PreFilterMessage(ref Message m)
        {
            /*//here you can specify  which key you need to filter
            for (int i = 0; i <= Flasks.Count() - 1; i++)
            {
                if (Flasks[i].visible == true && Flasks[i].usable == true)
                {
                    if (m.Msg == 0x0100 && (Keys)m.WParam.ToInt32() == Flasks[i].key)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }*/
            return false;
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool createdNew;
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "CF2D4313-33DE-489D-9721-6AFF69841DEA", out createdNew);
            while (true)
            {
                DateTime d1 = DateTime.Now;
                Thread.Sleep(250);
               
                
                for (int i = 0; i <= Flasks.Count() - 1; i++)
                {
                    if (Flasks[i].inUse)
                    {
                        if (Flasks[i].useDuration > 0)
                        {
                            DateTime d2 = DateTime.Now;
                            TimeSpan ts = d2 - d1;
                            Flasks[i].useDuration = Flasks[i].useDuration - (float)ts.Milliseconds / 1000;
                            //draw
                        }
                        else
                            Flasks[i].inUse = false;
                    }
                }
                if (WCCD)
                {
                    if (WCT > 0)
                    {
                        DateTime dwc = DateTime.Now;
                        TimeSpan t = dwc - d1;
                        WCT = WCT - (float)t.Milliseconds / 1000;
                    }
                    else
                        WCCD = false;
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
                bmpScreenCapture = new System.Drawing.Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
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
            this.Width = (int)xResolution;// set your own size
            this.Height = (int)yResolution;
            WindowState = FormWindowState.Maximized;
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
                PixelSize = new Size2((int)xResolution, (int)yResolution),
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
            SharpDX.Mathematics.Interop.RawRectangleF rec = new SharpDX.Mathematics.Interop.RawRectangleF();
            rec.Bottom = yResolution;
            rec.Top = 0;
            rec.Right = xResolution;
            rec.Left = 0;
            SharpDX.Mathematics.Interop.RawRectangleF rec1 = new SharpDX.Mathematics.Interop.RawRectangleF();
            rec1.Bottom = 187f / 1080f * yResolution;
            rec1.Top = 31f / 1080f * yResolution;
            rec1.Right = (1636f - 78f * 4f + xAdjustment) / 1920f * xResolution;
            rec1.Left = (1559f - 78f * 4f + xAdjustment) / 1920f * xResolution;
            SharpDX.Mathematics.Interop.RawRectangleF rec2 = new SharpDX.Mathematics.Interop.RawRectangleF();
            rec2.Bottom = 187f / 1080f * yResolution;
            rec2.Top = 31f / 1080f * yResolution;
            rec2.Right = (1636f - 78f * 3f + xAdjustment) / 1920f * xResolution;
            rec2.Left = (1559f - 78f * 3f + xAdjustment) / 1920f * xResolution;
            SharpDX.Mathematics.Interop.RawRectangleF rec3 = new SharpDX.Mathematics.Interop.RawRectangleF();
            rec3.Bottom = 187f / 1080f * yResolution;
            rec3.Top = 31f / 1080f * yResolution;
            rec3.Right = (1636f - 78f * 2f + xAdjustment) / 1920f * xResolution;
            rec3.Left = (1559f - 78f * 2f + xAdjustment) / 1920f * xResolution;
            SharpDX.Mathematics.Interop.RawRectangleF rec4 = new SharpDX.Mathematics.Interop.RawRectangleF();
            rec4.Bottom = 187f / 1080f * yResolution;
            rec4.Top = 31f / 1080f * yResolution;
            rec4.Right = (1636f - 78f + xAdjustment) / 1920f * xResolution;
            rec4.Left = (1559f - 78f + xAdjustment) / 1920f * xResolution;
            SharpDX.Mathematics.Interop.RawRectangleF rec5 = new SharpDX.Mathematics.Interop.RawRectangleF();
            rec5.Bottom = 187f / 1080f * yResolution;
            rec5.Top = 31f / 1080f * yResolution;
            rec5.Right = (1636f + xAdjustment) / 1920f * xResolution;
            rec5.Left = (1559f + xAdjustment) / 1920f * xResolution;
            SharpDX.Mathematics.Interop.RawRectangleF recwc = new SharpDX.Mathematics.Interop.RawRectangleF();
            recwc.Bottom = (276f / 1080) * yResolution;
            recwc.Top = (224f / 1080) * yResolution;
            recwc.Right = (1641f / 1920f) * xResolution;
            recwc.Left = (1589f / 1920f) * xResolution;
            List<SharpDX.Mathematics.Interop.RawRectangleF> rectangles = new List<SharpDX.Mathematics.Interop.RawRectangleF>();
            rectangles.Add(rec1);
            rectangles.Add(rec2);
            rectangles.Add(rec3);
            rectangles.Add(rec4);
            rectangles.Add(rec5);
            SharpDX.Mathematics.Interop.RawColor4 r4color2 = new SharpDX.Mathematics.Interop.RawColor4();
            r4color2.A = 255;
            r4color2.R = 125;
            r4color2.G = 0;
            r4color2.B = 0;
            while (true)
            {
                device.BeginDraw();
                SharpDX.Mathematics.Interop.RawColor4 transparent = new SharpDX.Mathematics.Interop.RawColor4();
                transparent.A = 0;
                transparent.R = 255;
                transparent.G = 255;
                transparent.B = 255;
                device.Clear(transparent);
                solidColorBrush.Color = r4color2;
                for (int i = 0; i <= Flasks.Count() - 1; i++)
                {
                    if (Flasks[i].inUse && Flasks[i].visible)
                    {
                        if (Flasks[i].useDuration > 0)
                        {
                            device.DrawBitmap(LoadFromFile(device, Flasks[i].flaskImageLocation), rectangles[i], flaskAlpha / 100, BitmapInterpolationMode.Linear, rec);
                        }
                    }
                }
                if (WCCD && showWC)
                {
                    device.DrawBitmap(LoadFromFile(device, "FlaskImages\\WC.png"), recwc, 1.0f, BitmapInterpolationMode.Linear, rec);
                }
                device.EndDraw();
                Thread.Sleep(200);
            }

            //whatever you want
        }
        public static SharpDX.Direct2D1.Bitmap LoadFromFile(RenderTarget renderTarget, string file)
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file))
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;

                    return new SharpDX.Direct2D1.Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
                }
            }
        }
    }
}
