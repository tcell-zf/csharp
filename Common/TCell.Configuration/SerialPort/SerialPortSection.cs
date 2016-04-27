using System.Configuration;

namespace TCell.Configuration.SerialPort
{
    public class SerialPortsSection : ConfigurationSection
    {
        public const string SerialPortsConfiguration = "serialPortsConfiguration";
        private const string SerialPorts = "serialPorts";

        [ConfigurationProperty(SerialPorts, IsDefaultCollection = true, IsRequired = true)]
        public SerialPortCollection SerialPortConfigItem
        {
            get { return (SerialPortCollection)this[SerialPorts]; }
            set { base[SerialPorts] = value; }
        }
    }
}
