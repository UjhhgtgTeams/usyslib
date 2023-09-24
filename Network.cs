using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace usyslib;
public class Network
{
    public static List<IPAddress> GetAllIPByHostname(string hostname)
    {
        return Dns.GetHostAddresses(hostname).Where(ip => !IPAddress.IsLoopback(ip)).ToList();
    }

    public static List<IPAddress> GetAllCurrentIP()
    {
        return Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => !IPAddress.IsLoopback(ip)).ToList();
    }

    public static string GetMacByIP(string ip)
    {
        int ldest = inet_addr(ip);
        string mac = "";
        try
        {
            long macInfo = 0;
            int length = 6;
            int result = SendARP(ldest, 0, ref macInfo, ref length);
            mac = Convert.ToString(macInfo, 16);
        }
        catch { }
        mac = "000000000000" + mac;
        mac = mac[^12..];
        return mac.Substring(10, 2).ToUpper() + "-" + mac.Substring(8, 2).ToUpper() + "-"
            + mac.Substring(6, 2).ToUpper() + "-" + mac.Substring(4, 2).ToUpper() + "-"
            + mac.Substring(2, 2).ToUpper() + "-" + mac[..2].ToUpper();
    }

    public static string GetMacByIP(IPAddress ip)
    {
        int ldest = inet_addr(ip.ToString());
        string mac = "";
        try
        {
            long macInfo = 0;
            int length = 6;
            int result = SendARP(ldest, 0, ref macInfo, ref length);
            mac = Convert.ToString(macInfo, 16);
        }
        catch { }
        mac = "000000000000" + mac;
        mac = mac[^12..];
        return mac.Substring(10, 2).ToUpper() + "-" + mac.Substring(8, 2).ToUpper() + "-"
            + mac.Substring(6, 2).ToUpper() + "-" + mac.Substring(4, 2).ToUpper() + "-"
            + mac.Substring(2, 2).ToUpper() + "-" + mac[..2].ToUpper();
    }

    public static string? GetHostnameByIP(string ip)
    {
        string hostname;
        try
        {
            hostname = Dns.GetHostEntry(ip).HostName;
        }
        catch { return null; }
        return hostname;
    }

    public static string? GetHostnameByIP(IPAddress ip)
    {
        string hostname;
        try
        {
            hostname = Dns.GetHostEntry(ip).HostName;
        }
        catch { return null; }
        return hostname;
    }

    public static bool IsPortInUse(int port)
    {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] endpoints = ipProperties.GetActiveTcpListeners();
        foreach (IPEndPoint endpoint in endpoints)
        {
            if (endpoint.Port == port)
            {
                return true;
            }
        }
        return false;
    }

    #region internal used functions
    internal static IPAddress? GetIPFromString(string stringIp)
    {
        if (IPAddress.TryParse(stringIp, out IPAddress ip))
        {
            return ip;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region system dll imports
    [DllImport("Iphlpapi.dll")]
    private static extern int SendARP(int dest, int host, ref long mac, ref int length);

    [DllImport("Ws2_32.dll")]
    private static extern int inet_addr(string ip);
    #endregion
    #region for system dlls
    #endregion
}