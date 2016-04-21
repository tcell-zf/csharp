using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

using TCell.Abstraction;

namespace TCell.Net
{
    public interface INetCommander
    {
        bool Start();
        bool Stop();
    }

    public interface INetClient
    {
        bool IsConnected { get; }
        bool Send(byte[] dgram);
    }

    public interface INetServer
    {

    }

    abstract public class NetCommander : Loggable
    {
        public NetCommander(EndpointPair endpoints)
        {
            if (endpoints == null)
                throw new ArgumentNullException(nameof(endpoints));

            EndPoints = endpoints;
        }

        protected EndpointPair EndPoints { get; set; }
    }

    abstract public class NetServerCommander : NetCommander
    {
        public Action<byte[]> HandleDatagramReceived = null;

        public NetServerCommander(EndpointPair endpoints)
            : base(endpoints) { }

        protected Action ListeningHandler = null;
        private Task task = null;

        private CancellationTokenSource cancelToken = null;
        protected bool IsCancellationRequested
        {
            get { return (cancelToken == null) ? true : cancelToken.IsCancellationRequested; }
        }

        protected bool IsLocalPortInUse
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

                return isInUse;
            }
        }

        public bool Start()
        {
            cancelToken = new CancellationTokenSource();

            if (task != null)
                Stop();

            task = Task.Factory.StartNew(ListeningHandler);
            return (task != null);
        }

        public bool Stop()
        {
            cancelToken.Cancel();
            task = null;
            return true;
        }
    }
}
