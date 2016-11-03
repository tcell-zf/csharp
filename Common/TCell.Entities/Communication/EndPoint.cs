using System;
using System.Net;
using System.Net.Sockets;

namespace TCell.Entities.Communication
{
    public class EndPoint : EntityBase
    {
        private ProtocolType protocol = ProtocolType.Udp;
        public ProtocolType Protocol
        {
            get { return protocol; }
            set
            {
                if (value != ProtocolType.Tcp && value != ProtocolType.Udp)
                    throw new OutOfMemoryException($"Support valid network protocols are {ProtocolType.Tcp} or {ProtocolType.Udp}.");

                protocol = value;
            }
        }

        public IPAddress IP { get; set; }

        private uint port = 10000;
        public uint Port
        {
            get { return port; }
            set
            {
                //if (value < 10000 || value > 65535)
                if (value < 0 || value > 65535)
                    throw new IndexOutOfRangeException("Support valid port number from 0 to 65535.");

                port = value;
            }
        }

        public uint? ReadTimeout { get; set; }
        public uint? WriteTimeout { get; set; }

        public uint? BufferLength { get; set; }
    }
}
