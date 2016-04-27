using System.Linq;
using System.Configuration;
using System.Collections.Generic;

using TCell.Configuration.SerialPort;

namespace TCell.Configuration
{
    public static partial class ConfigurationHelper
    {
        public static List<SerialPortConfigItem> GetSerialPortsConfiguration()
        {
            List<SerialPortConfigItem> sps = null;
            SerialPortCollection collection = GetSerialPortsConfig().SerialPortConfigItem;
            if (collection != null && collection.Count > 0)
            {
                sps = collection.Cast<SerialPortConfigItem>().ToList();
            }
            return sps;
        }

        public static SerialPortConfigItem GetSerialPortConfiguration(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            List<SerialPortConfigItem> items = GetSerialPortsConfiguration();
            if (items == null || items.Count == 0)
                return null;

            return (from item in items
                    where string.Compare(id, item.Id, true) == 0
                    select item).SingleOrDefault();
        }

        private static SerialPortsSection GetSerialPortsConfig()
        {
            return ConfigurationManager.GetSection(SerialPortsSection.SerialPortsConfiguration) as SerialPortsSection;
        }
    }
}
