using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Generic;

using TCell.Entities.Threading;

namespace TCell.Net
{
    public class TcpServerCommander : NetServerCommander, INetCommander, INetServer
    {
        public TcpServerCommander(EndpointPair endpoints)
            : base(endpoints)
        {
            ListeningHandler += DoListeningWork;
        }

        private Dictionary<TcpClient, TaskFacility> tcpClientWorkers = null;

        private void DoListeningWork()
        {
            TcpListener tcpListener = new TcpListener(EndPoints.GetLocalNetEndPoint());
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
            }, tcpClient);

            if (tcpClientWorkers == null)
                tcpClientWorkers = new Dictionary<TcpClient, TaskFacility>();
            tcpClientWorkers.Add(tcpClient, tf);
        }
    }
}
