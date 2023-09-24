using System.Runtime.InteropServices;

namespace usyslib;
public class Window
{
    public static IntPtr Get(string className, string windowName)
    {
        return FindWindow(className, windowName);
    }

    public static bool Move(IntPtr handle, int left, int top, int width, int height)
    {
        return MoveWindow(handle, left, top, width, height, true);
    }

    public static int GetLongPtr(IntPtr handle, int index)
    {
        if (IntPtr.Size == 8)
        {
            return GetWindowLongPtr64(handle, index);
        }
        else
        {
            return GetWindowLongPtr32(handle, index);
        }
    }

    public static void ShowTitleBar(IntPtr handle)
    {
        int style = GetLongPtr(handle, (int)GWL.STYLE);
        style |= (int)WS.CAPTION;
        SetWindowLong(handle, (int)GWL.STYLE, style);
        SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, (uint)(SWP.NOMOVE | SWP.NOSIZE | SWP.SHOWWINDOW));
    }

    public static void HideTitleBar(IntPtr handle)
    {
        int style = GetLongPtr(handle, (int)GWL.STYLE);
        style &= ~(int)WS.CAPTION;
        SetWindowLong(handle, (int)GWL.STYLE, style);
        SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, (uint)SWP.FRAMECHANGED);
    }

    public static List<int> GetInfo(IntPtr handle)
    {
        Rect rect = new();
        GetWindowRect(handle, ref rect);
        int left = rect.Left;
        int top = rect.Top;
        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;
        return new List<int> { left, top, width, height };
    }

    #region system dll imports
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

    private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
    #endregion
    #region for system dlls
    public enum GWL
    {
        WNDPROC = -4,
        HINSTANCE = -6,
        HWNDPARENT = -8,
        STYLE = -16,
        EXSTYLE = -20,
        USERDATA = -21,
        ID = -12
    }

    public enum WS : uint
    {
        OVERLAPPED = 0,
        POPUP = 0x80000000,
        CHILD = 0x40000000,
        MINIMIZE = 0x20000000,
        VISIBLE = 0x10000000,
        DISABLED = 0x8000000,
        CLIPSIBLINGS = 0x4000000,
        CLIPCHILDREN = 0x2000000,
        MAXIMIZE = 0x1000000,
        CAPTION = 0xC00000,
        BORDER = 0x800000,
        DLGFRAME = 0x400000,
        VSCROLL = 0x200000,
        HSCROLL = 0x100000,
        SYSMENU = 0x80000,
        THICKFRAME = 0x40000,
        GROUP = 0x20000,
        TABSTOP = 0x10000,
        MINIMIZEBOX = 0x20000,
        MAXIMIZEBOX = 0x10000,
        TILED = OVERLAPPED,
        ICONIC = MINIMIZE,
        SIZEBOX = THICKFRAME
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static class WndPos
    {
        public static IntPtr
        NoTopMost = new(-2),
        TopMost = new(-1),
        Top = new(0),
        Bottom = new(1);
    }

    [Flags]
    public enum SWP : uint
    {
        ASYNCWINDOWPOS = 0x4000,
        DEFERERASE = 0x2000,
        DRAWFRAME = 0x0020,
        FRAMECHANGED = 0x0020,
        HIDEWINDOW = 0x0080,
        NOACTIVATE = 0x0010,
        NOCOPYBITS = 0x0100,
        NOMOVE = 0x0002,
        NOOWNERZORDER = 0x0200,
        NOREDRAW = 0x0008,
        NOREPOSITION = 0x0200,
        NOSENDCHANGING = 0x0400,
        SHOWWINDOW = 0x0040,
        NOSIZE = 0x0001,
        NOZORDER = 0x0004,
    }
    #endregion
}
