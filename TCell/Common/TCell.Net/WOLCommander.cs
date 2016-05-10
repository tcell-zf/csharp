using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCell.Text;
using TCell.Entities.Communication;

namespace TCell.Net
{
    static public class WOLCommander
    {
        static public bool WakeUp(string macString)
        {
            return WakeUp(null, macString);
        }

        static public bool WakeUp(int? outboundPort, string macString)
        {
            byte[] mac = NumericTextParser.ParseMAC(macString);
            if (mac == null || mac.Length != 6)
                return false;

            return WakeUp(outboundPort, mac);
        }

        static public bool WakeUp(byte[] mac)
        {
            return WakeUp(null, mac);
        }

        static public bool WakeUp(int? outboundPort, byte[] mac)
        {
            byte[] cmd = new byte[102];
            cmd[0] = 0xFF;
            cmd[1] = 0xFF;
            cmd[2] = 0xFF;
            cmd[3] = 0xFF;
            cmd[4] = 0xFF;
            cmd[5] = 0xFF;

            for (int i = 1; i < 17; i++)
            {
                for (int j = 0; j < 6; j++)
                    cmd[i * 6 + j] = mac[j];
            }

            UdpClientCommander udpClient = null;
            try
            {
                udpClient = new UdpClientCommander(new EndpointPair()
                {
                    LocalEndPoint = (outboundPort == null) ? null : new EndPoint()
                    {
                        IP = System.Net.IPAddress.Any,
                        Port = (uint)outboundPort.Value,
                        Protocol = System.Net.Sockets.ProtocolType.Udp
                    },
                    RemoteEndPoint = new EndPoint()
                    {
                        IP = System.Net.IPAddress.Parse("255.255.255.255"),
                        Port = 0x2FFF,
                        Protocol = System.Net.Sockets.ProtocolType.Udp
                    }
                });

                udpClient.Send(cmd);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (udpClient != null)
                    udpClient.Stop();
            }

            return true;
        }
    }
}
