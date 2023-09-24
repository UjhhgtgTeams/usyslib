using System.Runtime.InteropServices;

namespace usyslib;
public class HotKey
{
    #region system dll imports
    // FIXME: doesn't work since 'System.Windows.Forms.Keys' is unavailable on platforms other than Windows
    // [DllImport("user32.dll")]
    // public static extern bool RegisterHotKey(
    //     IntPtr hWnd,
    //     int hotkeyId,
    //     KeyModifiers keyModifiers,
    //     Keys keys
    // );

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(
        IntPtr hWnd,
        int hotkeyId
    );

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        Windows = 8
    }
    #endregion
    #region for system dlls

    #endregion
}
