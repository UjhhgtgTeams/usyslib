using System.Management;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace usyslib;
public class Process
{

    public enum With : int{
        Native = 0,
        Nt = 1,
        Ntsd = 2,
        User32 = 3,
        All = 999
    }

    public static System.Diagnostics.Process Get(string name)
    {
        return GetAll(name)[0];
    }

    public static System.Diagnostics.Process Get(int id)
    {
        return System.Diagnostics.Process.GetProcessById(id);
    }

    public static System.Diagnostics.Process? Get(IntPtr handle)
    {
        foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses()) {
            if (process.Handle == handle) {
                return process;
            }
        }

        return null;
    }

    public static List<System.Diagnostics.Process> GetAll(string name)
    {
        return System.Diagnostics.Process.GetProcessesByName(name).ToList();
    }

    public static void TerminateAll(With with, string name)
    {
        foreach (System.Diagnostics.Process process in GetAll(name))
        {
            if (with == With.Native)
            {
                process.Kill();
            }
            else if (with == With.Nt)
            {
                IntPtr handle = IntPtr.Zero;
                try
                {
                    handle = OpenProcess(ProcessAccess.Terminate, false, process.Id);
                    if (handle != IntPtr.Zero)
                    {
                        NtTerminateProcess(handle);
                    }
                }
                finally
                {
                    if (handle != IntPtr.Zero)
                    {
                        CloseHandle(handle);
                    }
                }
            }
            else if (with == With.Ntsd)
            {
                throw new NotImplementedException();
            }
            else if (with == With.User32)
            {
                try
                {
                    EndTask(process.Handle, true, true);
                }
                catch { }
            }
        }
    }

    public static void Terminate(With with, int id)
    {
        System.Diagnostics.Process process = Get(id);
        if (with == With.Native)
        {
            process.Kill();
        }
        else if (with == With.Nt)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = OpenProcess(ProcessAccess.Terminate, false, process.Id);
                if (handle != IntPtr.Zero)
                {
                    NtTerminateProcess(handle);
                }
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                }
            }
        }
        else if (with == With.Ntsd)
        {
            throw new NotImplementedException();
        }
        else if (with == With.User32)
        {
            try
            {
                EndTask(process.Handle, true, true);
            }
            catch { }
        }
    }

    public static void TerminateAll(string name)
    {
        foreach (System.Diagnostics.Process process in GetAll(name))
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
            else
            {
                continue;
            }
            if (!process.HasExited)
            {
                IntPtr handle = IntPtr.Zero;
                try
                {
                    handle = OpenProcess(ProcessAccess.Terminate, false, process.Id);
                    if (handle != IntPtr.Zero)
                    {
                        NtTerminateProcess(handle);
                    }
                }
                finally
                {
                    if (handle != IntPtr.Zero)
                    {
                        CloseHandle(handle);
                    }
                }
            }
            else
            {
                continue;
            }
            // TODO: implement ntsd
            if (!process.HasExited)
            {
                try
                {
                    EndTask(process.Handle, true, true);
                }
                catch { }
            }
            else
            {
                continue;
            }
        }
    }

    public static void Terminate(int id)
    {
        System.Diagnostics.Process process = Get(id);
        if (!process.HasExited)
        {
            process.Kill();
        }
        if (!process.HasExited)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = OpenProcess(ProcessAccess.Terminate, false, process.Id);
                if (handle != IntPtr.Zero)
                {
                    NtTerminateProcess(handle);
                }
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                }
            }
        }
        // TODO: implement ntsd
        if (!process.HasExited)
        {
            try
            {
                EndTask(process.Handle, true, true);
            }
            catch { }
        }
    }

    // the name of this function looks scary to me ...
    public static void TerminateChildren(int parentID)
    {
        ManagementObjectSearcher mos = new("Select * From Win32_Process Where ParentProcessID = " + parentID);
        ManagementObjectCollection moc = mos.Get();
        foreach (ManagementObject mo in moc.Cast<ManagementObject>())
        {
            Terminate(With.All, Convert.ToInt32(mo["ProcessID"]));
        }
    }

    public static void SuspendAll(string name)
    {
        foreach (System.Diagnostics.Process process in GetAll(name))
        {
            Suspend(process);
        }
    }

    public static void Suspend(int id)
    {
        IntPtr handle = IntPtr.Zero;
        try
        {
            handle = OpenProcess(ProcessAccess.SuspendResume, false, id);
            if (handle != IntPtr.Zero)
            {
                NtSuspendProcess(handle);
            }
        }
        finally
        {
            if (handle != IntPtr.Zero)
            {
                CloseHandle(handle);
            }
        }
    }

    public static void Suspend(System.Diagnostics.Process process)
    {
        IntPtr handle = IntPtr.Zero;
        try
        {
            handle = OpenProcess(ProcessAccess.SuspendResume, false, process.Id);
            if (handle != IntPtr.Zero)
            {
                NtSuspendProcess(handle);
            }
        }
        finally
        {
            if (handle != IntPtr.Zero)
            {
                CloseHandle(handle);
            }
        }
    }

    public static void ResumeAll(string name)
    {
        foreach (System.Diagnostics.Process process in GetAll(name))
        {
            Resume(process);
        }
    }

    public static void Resume(int id)
    {
        IntPtr handle = IntPtr.Zero;
        try
        {
            handle = OpenProcess(ProcessAccess.SuspendResume, false, id);
            if (handle != IntPtr.Zero)
            {
                NtResumeProcess(handle);
            }
        }
        finally
        {
            if (handle != IntPtr.Zero)
            {
                CloseHandle(handle);
            }
        }
    }

    public static void Resume(System.Diagnostics.Process process)
    {
        IntPtr handle = IntPtr.Zero;
        try
        {
            handle = OpenProcess(ProcessAccess.SuspendResume, false, process.Id);
            if (handle != IntPtr.Zero)
            {
                NtResumeProcess(handle);
            }
        }
        finally
        {
            if (handle != IntPtr.Zero)
            {
                CloseHandle(handle);
            }
        }
    }

    public static System.Diagnostics.Process Run(string path, string arguments, ProcessWindowStyle style = ProcessWindowStyle.Normal, bool block = true)
    {
        System.Diagnostics.Process process = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = path,
                Arguments = arguments,
                WindowStyle = style
            }
        };
        process.Start();
        if (block)
        {
            process.WaitForExit();
        }
        return process;
    }

    public static string[]? GetArgs(int id)
    {
        string argsString;
        using (ManagementObjectSearcher mos = new("Select CommandLine From Win32_Process Where ProcessId = " + id))
        {
            using ManagementObjectCollection moc = mos.Get();
            ManagementBaseObject @object = moc.Cast<ManagementBaseObject>().SingleOrDefault();
            argsString = @object?["CommandLine"]?.ToString() ?? "";
        }
        IntPtr argsRaw = CommandLineToArgvW(argsString, out var argc);
        if (argsRaw == IntPtr.Zero)
        {
            return null;
        }
        try
        {
            string[] argsArray = new string[argc];
            for (var i = 0; i < argsArray.Length; i++)
            {
                IntPtr p = Marshal.ReadIntPtr(argsRaw, i * IntPtr.Size);
                argsArray[i] = Marshal.PtrToStringUni(p);
            }
            return argsArray;
        }
        finally
        {
            Marshal.FreeHGlobal(argsRaw);
        }
    }

    #region system dll imports
    [DllImport("ntdll.dll")]
    private static extern uint NtResumeProcess([In] IntPtr processHandle);

    [DllImport("ntdll.dll")]
    private static extern uint NtSuspendProcess([In] IntPtr processHandle);

    [DllImport("ntdll.dll")]
    private static extern uint NtTerminateProcess([In] IntPtr processHandle);

    [DllImport("user32.dll")]
    public static extern bool EndTask(IntPtr hWnd, bool fShutDown, bool fForce);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(
        ProcessAccess desiredAccess,
        bool inheritHandle,
        int processId
    );

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle([In] IntPtr handle);

    [DllImport("shell32.dll")]
    private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
    #endregion
    #region for system dlls
    [Flags]
    public enum ProcessAccess : uint
    {
        Terminate = 0x1,
        CreateThread = 0x2,
        SetSessionId = 0x4,
        VmOperation = 0x8,
        VmRead = 0x10,
        VmWrite = 0x20,
        DupHandle = 0x40,
        CreateProcess = 0x80,
        SetQuota = 0x100,
        SetInformation = 0x200,
        QueryInformation = 0x400,
        SetPort = 0x800,
        SuspendResume = 0x800,
        QueryLimitedInformation = 0x1000,
        Synchronize = 0x100000
    }
    #endregion
}
