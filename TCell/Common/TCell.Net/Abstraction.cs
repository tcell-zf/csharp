using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

using TCell.Abstraction;
using TCell.Entities.Threading;

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

        private TaskFacility task = null;

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
        protected bool IsListeningCancellationRequested
        {
            get { return (task == null) ? true : task.IsCancellationRequested; }
        }

        virtual public bool Start()
        {
            task = new TaskFacility();
            task.CancellationToken = new CancellationTokenSource();

            if (task != null)
                Stop();

            task.TaskInstance = Task.Factory.StartNew(ListeningHandler);
            return (task.TaskInstance != null);
        }

        virtual public bool Stop()
        {
            if (task != null)
            {
                task.CancelTask();
                task = null;
            }
            return true;
        }
    }
}
