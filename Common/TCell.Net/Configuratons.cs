using TCell.Entities.Communication;

namespace TCell.Net
{
    public class EndpointPair
    {
        public EndPoint LocalEndPoint { get; set; }
        public EndPoint RemoteEndPoint { get; set; }

        public System.Net.IPEndPoint GetLocalNetEndPoint()
        {
            return GetNetEndPoint(LocalEndPoint);
        }

        public System.Net.IPEndPoint GetRemoteNetEndPoint()
        {
            return GetNetEndPoint(RemoteEndPoint);
        }

        private System.Net.IPEndPoint GetNetEndPoint(EndPoint ep)
        {
            if (ep == null)
                return null;

            return new System.Net.IPEndPoint(ep.IP, (int)ep.Port);
        }
    }
}
