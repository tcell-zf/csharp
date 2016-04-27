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

            //int baudRate, dataBits;
            //Parity parity;
            //StopBits stopBits;
            //Encoding encoding;
            //int readTimeout, writeTimeout;
            //List<SerialPortSettings> settings = new List<SerialPortSettings>();
            //foreach (SerialPortConfigItem item in GetSerialPortsConfig().SerialPortConfigItem)
            //{
            //    if (!int.TryParse(item.BaudRate, out baudRate))
            //        baudRate = 9600;
            //    if (!int.TryParse(item.DataBits, out dataBits))
            //        dataBits = 8;

            //    if (!Enum.TryParse<Parity>(item.Parity, true, out parity))
            //        parity = Parity.None;

            //    if (!Enum.TryParse<StopBits>(item.StopBits, true, out stopBits))
            //        stopBits = StopBits.None;

            //    encoding = Encoding.Default;
            //    switch (item.Encoding)
            //    {
            //        case "ASCII":
            //            encoding = Encoding.ASCII;
            //            break;
            //        case "UTF8":
            //            encoding = Encoding.UTF8;
            //            break;
            //        case "UTF7":
            //            encoding = Encoding.UTF7;
            //            break;
            //        case "UTF32":
            //            encoding = Encoding.UTF32;
            //            break;
            //        case "Unicode":
            //            encoding = Encoding.Unicode;
            //            break;
            //        case "BigEndianUnicode":
            //            encoding = Encoding.BigEndianUnicode;
            //            break;
            //        default:
            //            break;
            //    }

            //    if (!int.TryParse(item.ReadTimeout, out readTimeout))
            //        readTimeout = 0;
            //    if (!int.TryParse(item.WriteTimeout, out writeTimeout))
            //        writeTimeout = 0;

            //    settings.Add(new SerialPortSettings()
            //    {
            //        Name = item.Name,
            //        PortName = item.PortName,
            //        BaudRate = baudRate,
            //        DataBits = dataBits,
            //        Parity = parity,
            //        StopBits = stopBits,
            //        Encoding = encoding,
            //        ReadTimeout = readTimeout,
            //        WriteTimeout = writeTimeout
            //    });
            //}
            //return settings;
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
