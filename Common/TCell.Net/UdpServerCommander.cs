using System;
using System.Net.Sockets;

namespace TCell.Net
{
    public class UdpServerCommander : NetServerCommander, INetCommander, INetServer
    {
        public UdpServerCommander(EndpointPair endpoints)
            : base(endpoints)
        {
            ListeningHandler += DoListeningWork;
        }

        private void DoListeningWork()
        {
            using (UdpClient client = new UdpClient())
            {
                if (EndPoints.LocalEndPoint.ReadTimeout != null)
                    client.Client.ReceiveTimeout = (int)EndPoints.LocalEndPoint.ReadTimeout;

                System.Net.IPEndPoint endpoint = EndPoints.GetLocalNetEndPoint();
                try
                {
                    client.Client.Bind(endpoint);
                }
                catch { return; }

                while (!IsCancellationRequested)
                {
                    try
                    {
                        byte[] receivedDgram = client.Receive(ref endpoint);
                        HandleDatagramReceived?.Invoke(receivedDgram);
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(SocketException))
                        {
                            SocketException sex = (SocketException)ex;
                            if (sex.SocketErrorCode == SocketError.TimedOut)
                                continue;
                            else
                                client.Close();
                        }
                        else
                        {
                            client.Close();
                        }
                    }
                }

                client.Close();
            }
        }
    }
}
