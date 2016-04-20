using System.Configuration;

namespace TCell.Configuration.IPEndPoint
{
    public class IPEndPointsSection : ConfigurationSection
    {
        public const string IPEndPointsConfiguration = "ipEndPointsConfiguration";
        private const string IPEndPoints = "ipEndPoints";

        [ConfigurationProperty(IPEndPoints, IsDefaultCollection = true, IsRequired = true)]
        public IPEndPointCollection IPEndPointsConfigItem
        {
            get { return (IPEndPointCollection)this[IPEndPoints]; }
            set { base[IPEndPoints] = value; }
        }
    }
}
