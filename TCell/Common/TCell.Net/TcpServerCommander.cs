using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;

using TCell.Entities.Threading;

namespace TCell.Net
{
    public class TcpServerCommander : NetServerCommander
    {
        public TcpServerCommander(EndpointPair endpoints)
            : base(endpoints)
        {
            ListeningHandler += DoListeningWork;
        }

        private TcpListener tcpListener = null;
        private Dictionary<TcpClient, TaskFacility> tcpClientWorkers = null;

        protected override bool IsLocalPortInUse
        {
            get
            {
                bool isInUse = false;

                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                System.Net.IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

                foreach (System.Net.IPEndPoint endPoint in ipEndPoints)
                {
                    if (endPoint.Port == EndPoints.LocalEndPoint.Port)
                    {
                        isInUse = true;
                        break;
                    }
                }

                if (isInUse)
                    TcpServerCommander.LogException($"TCP listener port {EndPoints.LocalEndPoint.Port} is in use.", null);

                return isInUse;
            }
        }

        public override bool Start()
        {
            if (IsLocalPortInUse)
                return false;

            tcpListener = new TcpListener(EndPoints.GetLocalNetEndPoint());
            return base.Start();
        }

        public override bool Stop()
        {
            ListeningHandler = null;

            if (tcpClientWorkers != null && tcpClientWorkers.Count > 0)
            {
                foreach (KeyValuePair<TcpClient, TaskFacility> kv in tcpClientWorkers)
                {
                    kv.Value.CancelTask();
                    kv.Key.Close();
                }

                tcpClientWorkers.Clear();
                tcpClientWorkers = null;
            }

            if (tcpListener != null)
                tcpListener.Stop();

            return base.Stop();
        }

        private void DoListeningWork()
        {
            try
            {
                tcpListener.Start();

                while (!IsListeningCancellationRequested)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    if (EndPoints.LocalEndPoint.ReadTimeout != null)
                        tcpClient.ReceiveTimeout = (int)EndPoints.LocalEndPoint.ReadTimeout;
                    if (EndPoints.LocalEndPoint.WriteTimeout != null)
                        tcpClient.SendTimeout = (int)EndPoints.LocalEndPoint.WriteTimeout;
                    if (EndPoints.LocalEndPoint.BufferLength != null)
                        tcpClient.ReceiveBufferSize = tcpClient.SendBufferSize = (int)EndPoints.LocalEndPoint.BufferLength;

                    DoTcpClientWork(tcpClient);
                }
            }
            catch (Exception ex)
            {
                TcpServerCommander.LogException($"Start TCP listener failed: {ex.Message}", ex);
            }
        }

        private void DoTcpClientWork(TcpClient tcpClient)
        {
            TaskFacility tf = new TaskFacility();
            tf.CancellationToken = new CancellationTokenSource();
            tf.TaskInstance = Task.Factory.StartNew((c) =>
            {
                TcpClient client = c as TcpClient;
                if (client == null)
                    return;

                while (!tf.IsCancellationRequested)
                {
                    if (!client.Connected)
                        continue;

                    try
                    {
                        NetworkStream stream = client.GetStream();
                        int readLength;
                        byte[] rawData = EndPoints.LocalEndPoint.BufferLength == null ? new byte[512] : new byte[(int)EndPoints.LocalEndPoint.BufferLength];
                        while ((readLength = stream.Read(rawData, 0, rawData.Length)) != 0)
                        {
                            byte[] receivedDgram = new byte[readLength];
                            for (int i = 0; i < readLength; i++)
                                receivedDgram[i] = rawData[i];

                            HandleDatagramReceived?.Invoke(receivedDgram);
                        }
                    }
                    catch (Exception ex)
                    {
                        TcpServerCommander.LogException($"Read TCP datagram failed: {ex.Message}", ex);
                    }
                }
            }, tcpClient);

            if (tcpClientWorkers == null)
                tcpClientWorkers = new Dictionary<TcpClient, TaskFacility>();
            tcpClientWorkers.Add(tcpClient, tf);
        }
    }
}
