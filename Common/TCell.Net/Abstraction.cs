using System;

using TCell.Abstraction;
using TCell.Entities.Net;

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
        protected EndpointPair EndPoints { get; set; }

        public NetCommander(EndpointPair endpoints)
        {
            if (endpoints == null)
                throw new ArgumentNullException(nameof(endpoints));

            EndPoints = endpoints;
        }
    }
}
