using System;
using System.Net.Sockets;

namespace TCell.Net
{
    public class UdpClientCommander : NetCommander, INetCommander, INetClient
    {
        public UdpClientCommander(EndpointPair endpoints)
            : base(endpoints) { }

        public bool IsConnected
        {
            get { return true; }
        }

        public bool Start()
        {
            if (EndPoints == null)
                return false;
            if (EndPoints.RemoteEndPoint == null || EndPoints.RemoteEndPoint.Protocol != ProtocolType.Udp)
                return false;
            if (EndPoints.LocalEndPoint != null && EndPoints.LocalEndPoint.Protocol != ProtocolType.Udp)
                return false;

            return true;
        }

        public bool Stop()
        {
            return true;
        }

        public bool Send(byte[] dgram)
        {
            if (dgram == null || dgram.Length == 0)
                throw new ArgumentNullException(nameof(dgram));

            int sent = 0;
            try
            {
                using (UdpClient udpClient = (EndPoints.LocalEndPoint == null) ? new UdpClient() : new UdpClient((int)EndPoints.LocalEndPoint.Port))
                {
                    udpClient.Connect(EndPoints.RemoteEndPoint.IP, (int)EndPoints.RemoteEndPoint.Port);
                    sent = udpClient.Send(dgram, dgram.Length);
                    udpClient.Close();
                }
            }
            catch (Exception ex)
            {
                UdpClientCommander.LogException($"Send UDP datagram failed: {ex.Message}", ex);
                return false;
            }
            return (sent == dgram.Length);
        }
    }
}
