using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace TCell.Net
{
    public class UdpServerCommander : NetServerCommander
    {
        public UdpServerCommander(EndpointPair endpoints)
            : base(endpoints)
        {
            ListeningHandler += DoListeningWork;
        }

        protected override bool IsLocalPortInUse
        {
            get
            {
                bool isInUse = false;

                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                System.Net.IPEndPoint[] ipEndPoints = ipProperties.GetActiveUdpListeners();

                foreach (System.Net.IPEndPoint endPoint in ipEndPoints)
                {
                    if (endPoint.Port == EndPoints.LocalEndPoint.Port)
                    {
                        isInUse = true;
                        break;
                    }
                }

                if (isInUse)
                    TcpServerCommander.LogException($"UDP listener port {EndPoints.LocalEndPoint.Port} is in use.", null);

                return isInUse;
            }
        }

        public override bool Start()
        {
            if (IsLocalPortInUse)
                return false;

            return base.Start();
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

                while (!IsListeningCancellationRequested)
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
