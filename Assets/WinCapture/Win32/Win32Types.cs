using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace WinCapture
{
    // Much of this is from pinvoke.net
    public class Win32Types
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            private int _Left;
            private int _Top;
            private int _Right;
            private int _Bottom;

            public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
            {

            }
            public RECT(int Left, int Top, int Right, int Bottom)
            {
                _Left = Left;
                _Top = Top;
                _Right = Right;
                _Bottom = Bottom;
            }

            public int X
            {
                get { return _Left; }
                set { _Left = value; }
            }
            public int Y
            {
                get { return _Top; }
                set { _Top = value; }
            }
            public int Left
            {
                get { return _Left; }
                set { _Left = value; }
            }
            public int Top
            {
                get { return _Top; }
                set { _Top = value; }
            }
            public int Right
            {
                get { return _Right; }
                set { _Right = value; }
            }
            public int Bottom
            {
                get { return _Bottom; }
                set { _Bottom = value; }
            }
            public int Height
            {
                get { return _Bottom - _Top; }
                set { _Bottom = value + _Top; }
            }
            public int Width
            {
                get { return _Right - _Left; }
                set { _Right = value + _Left; }
            }
            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set
                {
                    _Left = value.X;
                    _Top = value.Y;
                }
            }
            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set
                {
                    _Right = value.Width + _Left;
                    _Bottom = value.Height + _Top;
                }
            }

            public static implicit operator System.Drawing.Rectangle(RECT Rectangle)
            {
                return new System.Drawing.Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
            }
            public static implicit operator RECT(System.Drawing.Rectangle Rectangle)
            {
                return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
            }
            public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
            {
                return Rectangle1.Equals(Rectangle2);
            }
            public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
            {
                return !Rectangle1.Equals(Rectangle2);
            }

            public override string ToString()
            {
                return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(RECT Rectangle)
            {
                return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
            }

            public override bool Equals(object Object)
            {
                if (Object is RECT)
                {
                    return Equals((RECT)Object);
                }
                else if (Object is System.Drawing.Rectangle)
                {
                    return Equals(new RECT((System.Drawing.Rectangle)Object));
                }

                return false;
            }

            public bool Contains(System.Drawing.Rectangle rect)
            {
                return rect.Left >= Left && rect.Right <= Right && rect.Top >= Top && rect.Bottom <= Bottom;
            }

            public bool Contains(RECT rect)
            {
                return rect.Left >= Left && rect.Right <= Right && rect.Top >= Top && rect.Bottom <= Bottom;
            }

            public bool Contains(PointL pt)
            {
                return pt.x >= Left && pt.x <= Right && pt.y >= Top && pt.y <= Bottom;
            }

            public bool Contains(System.Drawing.Point pt)
            {
                return pt.X >= Left && pt.X <= Right && pt.Y >= Top && pt.Y <= Bottom;
            }

            public bool Contains(int x, int y)
            {
                return x >= Left && x <= Right && y >= Top && y <= Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BitmapInfoHeader
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public Win32Consts.BitmapCompressionMode biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;

            public void Init()
            {
                biSize = (uint)Marshal.SizeOf(this);
            }
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct BitmapInfo
        {
            /// <summary>
            /// A BITMAPINFOHEADER structure that contains information about the dimensions of color format.
            /// </summary>
            public BitmapInfoHeader bmiHeader;

            /// <summary>
            /// An array of RGBQUAD. The elements of the array that make up the color table.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
            public RGBQuad[] bmiColors;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQuad
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }


        public struct PointL
        {
            public int x;
            public int y;
        }

        struct DevMode
        {
            public const int CCHDEVICENAME = 32;
            public const int CCHFORMNAME = 32;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MonitorInfoEx
        {
            /// <summary>
            /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function. 
            /// Doing so lets the function determine the type of structure you are passing to it.
            /// </summary>
            public int Size;

            /// <summary>
            /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. 
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public Win32Types.RECT Monitor;

            /// <summary>
            /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications, 
            /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor. 
            /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars. 
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public Win32Types.RECT WorkArea;

            /// <summary>
            /// The attributes of the display monitor.
            /// 
            /// This member can be the following value:
            ///   1 : MONITORINFOF_PRIMARY
            /// </summary>
            public uint Flags;

            /// <summary>
            /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name, 
            /// and so can save some bytes by using a MONITORINFO structure.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DevMode.CCHDEVICENAME)]
            public string DeviceName;

            public void Init()
            {
                this.Size = 40 + 2 * DevMode.CCHDEVICENAME;
                this.DeviceName = string.Empty;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfo
        {
            public int cbSize;
            public Win32Types.RECT rcMonitor;
            public Win32Types.RECT rcWork;
            public uint dwFlags;
        }


        /// 
        /// <summary>
        /// The struct that contains the display information
        /// </summary>
        public class DisplayInfo
        {
            public string Availability { get; set; }
            public string ScreenHeight { get; set; }
            public string ScreenWidth { get; set; }
            public Win32Types.RECT MonitorArea { get; set; }
            public Win32Types.RECT WorkArea { get; set; }
            public IntPtr hwnd { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CursorInfo
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
                                        // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
                                        //    0             The cursor is hidden.
                                        //    CURSOR_SHOWING    The cursor is showing.
            public IntPtr hCursor;          // Handle to the cursor. 
            public PointL ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        public struct IconInfo
        {
            /// <summary>
            /// Specifies whether this structure defines an icon or a cursor.
            /// A value of TRUE specifies an icon; FALSE specifies a cursor
            /// </summary>
            public bool fIcon;
            /// <summary>
            /// The x-coordinate of a cursor's hot spot
            /// </summary>
            public Int32 xHotspot;
            /// <summary>
            /// The y-coordinate of a cursor's hot spot
            /// </summary>
            public Int32 yHotspot;
            /// <summary>
            /// The icon bitmask bitmap
            /// </summary>
            public IntPtr hbmMask;
            /// <summary>
            /// A handle to the icon color bitmap. 
            /// </summary>
            public IntPtr hbmColor;
        }

        /**/
        // From https://github.com/akfish/MwLib/blob/master/Native/WindowInfo.cs
        /// <summary>
        /// Get the given window's information.
        /// <para>Useage: </para>
        /// <para>WindowInfo wi = new WindowInfo(wndHandle); </para>
        /// <para>Rectangle rect = wi.WindowRect;</para>
        /// <para>String title = wi.WindowText</para>
        /// </summary>
        /// TODO: Add more window info
        public class WindowInfo
        {
            public IntPtr hwnd;

            public RECT windowRect;

            public string title;

            public string className;

            public WindowInfo(IntPtr wndHandle)
            {
                hwnd = wndHandle;
                GetRect();
                GetTitle();
                GetClassName();
            }

            public void GetRect()
            {
                Win32Funcs.GetWindowRect(hwnd, out windowRect);
            }

            public void GetTitle()
            {
                int len = Win32Funcs.GetWindowTextLength(hwnd);
                StringBuilder sb = new StringBuilder(len + 1);
                Win32Funcs.GetWindowText(hwnd, sb, sb.Capacity);

                title = sb.ToString();
            }

            public void GetClassName()
            {
                StringBuilder sb = new StringBuilder(255);
                Win32Funcs.GetClassName(hwnd, sb, sb.Capacity);

                className = sb.ToString();
            }
        }

        public struct command
        {
            public const int WM_CHAR = 0x0102; //usado pra passar um char do teclado pra janela
            public const int WM_MOUSEMOVE = 0x0200;
            public const int WM_LBUTTONDOWN = 0x0201;
            public const int WM_LBUTTONUP = 0x0202;
            public const int WM_KEYDOWN = 0x0100;
            public const int WM_KEYUP = 0x0101;
            public const int WM_NCHITTEST = 0x0084;
            public const int WM_PARENTNOTIFY = 0x0210;
            public const int WM_MOUSEACTIVATE = 0x0021;
            public const int WM_CLOSE = 0x0010;
        }

        public static readonly Dictionary<char, int> VirtualKeyCode = new Dictionary<char, int>
        {
            {'a', 0x41 },
            {'A', 0x41 },
            {'b', 0x42 },
            {'B', 0x42 },
            {'c', 0x43 },
            {'C', 0x43 },
            {'d', 0x44 },
            {'D', 0x44 },
            {'e', 0x45 },
            {'E', 0x45 },
            {'f', 0x46 },
            {'F', 0x46 },
            {'g', 0x47 },
            {'G', 0x47 },
            {'h', 0x48 },
            {'H', 0x48 },
            {'i', 0x49 },
            {'I', 0x49 },
            {'j', 0x4A },
            {'J', 0x4A },
            {'k', 0x4B },
            {'K', 0x4B },
            {'l', 0x4C },
            {'L', 0x4C },
            {'m', 0x4D },
            {'M', 0x4D },
            {'n', 0x4E },
            {'N', 0x4E },
            {'o', 0x4F },
            {'O', 0x4F },
            {'p', 0x50 },
            {'P', 0x50 },
            {'q', 0x51 },
            {'Q', 0x51 },
            {'r', 0x52 },
            {'R', 0x52 },
            {'s', 0x53 },
            {'S', 0x53 },
            {'t', 0x54 },
            {'T', 0x54 },
            {'u', 0x55 },
            {'U', 0x55 },
            {'v', 0x56 },
            {'V', 0x56 },
            {'w', 0x57 },
            {'W', 0x57 },
            {'x', 0x58 },
            {'X', 0x58 },
            {'y', 0x59 },
            {'Y', 0x59 },
            {'z', 0x5A },
            {'Z', 0x5A },

            {'1', 0x31 },
            {'2', 0x32 },
            {'3', 0x33 },
            {'4', 0x34 },
            {'5', 0x35 },
            {'6', 0x36 },
            {'7', 0x37 },
            {'8', 0x38 },
            {'9', 0x39 },
            {'0', 0x30 },
        };
    }
}
