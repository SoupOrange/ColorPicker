using ColorPicker.Commad;
using ColorPicker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace ColorPicker.ViewModel
{
  public  class MainVM: NotifyBase
    {

        private SolidColorBrush _clickColor =new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66BB6A"));

        public SolidColorBrush ClickColor
        {
            get { 
                return _clickColor;
            }
            set { 
                _clickColor = value;
                DoNotify(); 
            }
        }


        MouseHook mh;
        private CmdBase startPickCmd;
        private CmdBase _StopPickCmd;

        public CmdBase StopPickCmd
        {
            get { return _StopPickCmd; }
            set { _StopPickCmd = value; }
        }

        public CmdBase StartPickCmd
        {
            get { return startPickCmd; }
            set { startPickCmd = value; }
        }

        public MainVM()
        {
            Init();
        }
        public void Init()
        {
            startPickCmd = new CmdBase();
            startPickCmd.DoCanExecute += new Func<object, bool>((o) => true);
            startPickCmd.DoExecute += DoPick;
            _StopPickCmd = new CmdBase();
            _StopPickCmd.DoCanExecute += new Func<object, bool>((o) => true);
            _StopPickCmd.DoExecute += DoStopPick;
        }
        public void DoStopPick(object parm)
        {
            mh?.UnHook();
 
        }
        public void DoPick(object parm) {
            if (mh==null)
            {
                mh = new MouseHook();
                mh.MouseDownEvent += mh_MouseDownEvent;
                mh.MouseUpEvent += mh_MouseUpEvent;
            }
            mh.SetHook();
        
        }

        //按下鼠标键触发的事件
        private void mh_MouseDownEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {

                    ClickColor = new SolidColorBrush(ColorPickerManager.GetColor(e.X, e.Y)); ;
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }

        }
        //松开鼠标键触发的事件
        private void mh_MouseUpEvent(object sender, MouseEventArgs e)
        {
        }
 

    }


    /// <summary>
    /// 获取当前光标处颜色，win8下wpf测试成功
    /// </summary>
    public class ColorPickerManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">鼠标相对于显示器的坐标X</param>
        /// <param name="y">鼠标相对于显示器的坐标Y</param>
        /// <returns></returns>
        public static Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb(255, (byte)(pixel & 0x000000FF), (byte)((pixel & 0x0000FF00) >> 8), (byte)((pixel & 0x00FF0000) >> 16));

            return color;
        }

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
        IntPtr hdcDest, // 目标设备的句柄 
        int nXDest, // 目标对象的左上角的X坐标 
        int nYDest, // 目标对象的左上角的X坐标 
        int nWidth, // 目标对象的矩形的宽度 
        int nHeight, // 目标对象的矩形的长度 
        IntPtr hdcSrc, // 源设备的句柄 
        int nXSrc, // 源对象的左上角的X坐标 
        int nYSrc, // 源对象的左上角的X坐标 
        int dwRop // 光栅的操作值 
        );

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(
        string lpszDriver, // 驱动名称 
        string lpszDevice, // 设备名称 
        string lpszOutput, // 无用，可以设定位"NULL" 
        IntPtr lpInitData // 任意的打印机数据 
        );

        /// <summary>
        /// 获取指定窗口的设备场景
        /// </summary>
        /// <param name="hwnd">将获取其设备场景的窗口的句柄。若为0，则要获取整个屏幕的


        /// <returns>指定窗口的设备场景句柄，出错则为0</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>
        /// 释放由调用GetDC函数获取的指定设备场景
        /// </summary>
        /// <param name="hwnd">要释放的设备场景相关的窗口句柄</param>
        /// <param name="hdc">要释放的设备场景句柄</param>
        /// <returns>执行成功为1，否则为0</returns>
        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>
        /// 在指定的设备场景中取得一个像素的RGB值
        /// </summary>
        /// <param name="hdc">一个设备场景的句柄</param>
        /// <param name="nXPos">逻辑坐标中要检查的横坐标</param>
        /// <param name="nYPos">逻辑坐标中要检查的纵坐标</param>
        /// <returns>指定点的颜色</returns>
        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
    }
    /// <summary>
    /// win8下wpf程序测试成功
    /// </summary>
    public class CursorPointManager
    {
        #region 得到光标在屏幕上的位置
        [DllImport("user32")]
        private static extern bool GetCursorPos(out Point lpPoint);

        /// <summary>
        /// 获取光标相对于显示器的位置
        /// </summary>
        /// <returns></returns>
        public static Point GetCursorPosition()
        {
            Point showPoint = new Point();
            GetCursorPos(out showPoint);
            return showPoint;
        }
        #endregion

    }


    class Win32Api
    {
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        //安装钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        //调用下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
    }
    class MouseHook
    {
        private Point point;
        private Point Point
        {
            get { return point; }
            set
            {
                if (point != value)
                {
                    point = value;
                    if (MouseMoveEvent != null)
                    {
                        var e = new MouseEventArgs(MouseButtons.None, 0, (int)point.X, (int)point.Y, 0);
                        MouseMoveEvent(this, e);
                    }
                }
            }
        }
        private int hHook;
        private static int hMouseHook = 0;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;

        public const int WH_MOUSE_LL = 14;
        public Win32Api.HookProc hProc;
        public MouseHook()
        {
            this.Point = new Point();
        }
        public int SetHook()
        {
            hProc = new Win32Api.HookProc(MouseHookProc);
            hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
            return hHook;
        }
        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }
        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Win32Api.MouseHookStruct MyMouseHookStruct = (Win32Api.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.MouseHookStruct));
            if (nCode < 0)
            {
                return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
                switch ((Int32)wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        MouseDownEvent(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        MouseDownEvent(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_MBUTTONDOWN:
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        MouseDownEvent(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        MouseUpEvent(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        MouseUpEvent(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_MBUTTONUP:
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        MouseUpEvent(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                }

                this.Point = new Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);
                return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);
        public event MouseMoveHandler MouseMoveEvent;

        public delegate void MouseClickHandler(object sender, MouseEventArgs e);
        public event MouseClickHandler MouseClickEvent;

        public delegate void MouseDownHandler(object sender, MouseEventArgs e);
        public event MouseDownHandler MouseDownEvent;

        public delegate void MouseUpHandler(object sender, MouseEventArgs e);
        public event MouseUpHandler MouseUpEvent;
    }
 
}
