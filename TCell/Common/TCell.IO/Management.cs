using System;
using System.Net;
using System.Management;
using System.Runtime.InteropServices;

namespace TCell.IO
{
    public class Management
    {
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, byte[] mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        static public string GetLocalHostMac()
        {
            string mac = string.Empty;
            ManagementClass mcMAC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection mocMAC = mcMAC.GetInstances();
            foreach (ManagementObject m in mocMAC)
            {
                if ((bool)m["IPEnabled"])
                {
                    mac = m["MacAddress"].ToString();
                    break;
                }
            }
            return mac;
        }

        static public string GetRemoteHostMac(IPAddress ip)
        {
            string mac = string.Empty;
            byte[] byArr = new byte[6];
            Int32 len = 6;
            try
            {
                Int32 ldest = inet_addr(ip.ToString());
                int res = SendARP(ldest, 0, byArr, ref len);
                mac = BitConverter.ToString(byArr, 0, 6); ;
            }
            catch { }

            return mac;
        }

        static public string GetCPUProcessorId()
        {
            string procId = string.Empty;
            ManagementClass mcCpu = new ManagementClass("Win32_Processor");
            ManagementObjectCollection mocCpu = mcCpu.GetInstances();
            foreach (ManagementObject m in mocCpu)
            {
                procId = m["ProcessorID"].ToString();
            }
            return procId;
        }

        static public string GetHDDSerialNumber()
        {
            string sn = string.Empty;
            ManagementClass mcHD = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection mocHD = mcHD.GetInstances();
            if ((mocHD != null) && (mocHD.Count > 0))
            {
                ManagementObject[] moArr = new ManagementObject[mocHD.Count];
                mocHD.CopyTo(moArr, 0);
                sn = moArr[0]["Model"].ToString();
            }
            return sn;
        }
    }
}
