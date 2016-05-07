using System;
using System.Net;
using System.Management;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TCell.IO
{
    public class Management
    {
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, byte[] mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        static public Dictionary<string, bool> GetLocalHostMac()
        {
            Dictionary<string, bool> macs = null;
            ManagementClass mcMAC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection mocMAC = mcMAC.GetInstances();
            if (mocMAC == null)
                return macs;

            foreach (ManagementObject m in mocMAC)
            {
                if (macs == null)
                    macs = new Dictionary<string, bool>();

                if (m["MacAddress"] != null)
                    macs.Add(m["MacAddress"].ToString(), (bool)m["IPEnabled"]);
            }
            return macs;
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

        static public List<string> GetCPUProcessorIds()
        {
            List<string> procIds = null;
            ManagementClass mcCpu = new ManagementClass("Win32_Processor");
            ManagementObjectCollection mocCpu = mcCpu.GetInstances();
            if (mocCpu == null)
                return procIds;

            foreach (ManagementObject m in mocCpu)
            {
                if (procIds == null)
                    procIds = new List<string>();

                procIds.Add(m["ProcessorID"].ToString());
            }
            return procIds;
        }

        static public List<string> GetHDDSerialNumbers()
        {
            List<string> sns = null;
            ManagementClass mcHD = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection mocHD = mcHD.GetInstances();
            if (mocHD == null)
                return sns;

            foreach (ManagementObject m in mocHD)
            {
                if (sns == null)
                    sns = new List<string>();

                sns.Add(m["Model"].ToString());
            }
            //if ((mocHD != null) && (mocHD.Count > 0))
            //{
            //    ManagementObject[] moArr = new ManagementObject[mocHD.Count];
            //    mocHD.CopyTo(moArr, 0);
            //    sn = moArr[0]["Model"].ToString();
            //}
            return sns;
        }
    }
}
