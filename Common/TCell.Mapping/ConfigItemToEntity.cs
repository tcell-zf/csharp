using System;
using System.Net;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;
using System.Collections.Generic;

using TCell.Abstraction;
using ENTITIES = TCell.Entities;
using TCell.Configuration.Mapping;
using TCell.Configuration.IPEndPoint;
using TCell.Configuration.SerialPort;

namespace TCell.Mapping
{
    public class ConfigItemToEntity : Loggable
    {
        static public ENTITIES.Communication.EndPoint MapNetEndpoint(IPEndPointConfigItem configItem)
        {
            ProtocolType protocol;
            if (!Enum.TryParse<ProtocolType>(configItem.Protocol, true, out protocol))
                protocol = ProtocolType.Udp;

            IPAddress ip = IPAddress.Any;
            if (!string.IsNullOrEmpty(configItem.IP))
            {
                if (!IPAddress.TryParse(configItem.IP, out ip))
                    ip = IPAddress.Any;
            }

            if (string.IsNullOrEmpty(configItem.Port))
                throw new ArgumentNullException(nameof(configItem.Port));
            uint port;
            if (!uint.TryParse(configItem.Port, out port))
                throw new ArgumentException($"{configItem.Port} cannot be parsed as uint.", nameof(configItem.Port));

            uint uintVal;
            uint? readTimeout = null;
            if (!string.IsNullOrEmpty(configItem.ReadTimeout))
            {
                if (uint.TryParse(configItem.ReadTimeout, out uintVal))
                    readTimeout = uintVal;
            }

            uint? writeTimeout = null;
            if (!string.IsNullOrEmpty(configItem.WriteTimeout))
            {
                if (uint.TryParse(configItem.WriteTimeout, out uintVal))
                    writeTimeout = uintVal;
            }

            uint? bufferLength = null;
            if (!string.IsNullOrEmpty(configItem.BufferLength))
            {
                if (uint.TryParse(configItem.BufferLength, out uintVal))
                    bufferLength = uintVal;
            }

            ENTITIES.Communication.EndPoint endPoint = null;
            try
            {
                endPoint = new ENTITIES.Communication.EndPoint()
                {
                    Id = configItem.Id,
                    Protocol = protocol,
                    IP = ip,
                    Port = port,
                    ReadTimeout = readTimeout,
                    WriteTimeout = writeTimeout,
                    BufferLength = bufferLength,
                    Category = configItem.Category
                };
            }
            catch (Exception ex)
            {
                LogException($"Failed to map [{configItem}] to {nameof(ENTITIES.Communication.EndPoint)}.", ex);
            }
            return endPoint;
        }

        static public List<ENTITIES.Communication.EndPoint> MapNetEndpoints(List<IPEndPointConfigItem> configItems)
        {
            if (configItems == null || configItems.Count == 0)
                return null;

            List<ENTITIES.Communication.EndPoint> endPoints = new List<ENTITIES.Communication.EndPoint>();
            foreach (IPEndPointConfigItem configItem in configItems)
            {
                ENTITIES.Communication.EndPoint endPoint = null;
                try
                {
                    endPoint = MapNetEndpoint(configItem);
                }
                catch (Exception ex)
                {
                    LogException($"Failed to map [{configItem}] to {nameof(ENTITIES.Communication.EndPoint)} in endpoint list.", ex);
                }

                if (endPoint != null)
                    endPoints.Add(endPoint);
            }
            return endPoints;
        }

        static public ENTITIES.Mapping MapMapping(MappingConfigItem configItem)
        {
            return new Entities.Mapping()
            {
                Id = configItem.Id,
                Value = configItem.Value,
                Category = configItem.Category
            };
        }

        static public List<ENTITIES.Mapping> MapMappings(List<MappingConfigItem> configItems)
        {
            if (configItems == null || configItems.Count == 0)
                return null;

            List<ENTITIES.Mapping> mappings = new List<Entities.Mapping>();
            foreach (MappingConfigItem configItem in configItems)
            {
                mappings.Add(MapMapping(configItem));
            }
            return mappings;
        }

        static public ENTITIES.Communication.SerialPortParam MapSerialPortParam(SerialPortConfigItem configItem)
        {
            uint baudRate, dataBits;
            if (!uint.TryParse(configItem.BaudRate, out baudRate))
                baudRate = 9600;
            if (!uint.TryParse(configItem.DataBits, out dataBits))
                dataBits = 8;

            Parity parity;
            if (!Enum.TryParse<Parity>(configItem.Parity, true, out parity))
                parity = Parity.None;

            StopBits stopBits;
            if (!Enum.TryParse<StopBits>(configItem.StopBits, true, out stopBits))
                stopBits = StopBits.None;

            Encoding encoding;
            encoding = Encoding.Default;
            switch (configItem.Encoding)
            {
                case "ASCII":
                    encoding = Encoding.ASCII;
                    break;
                case "UTF8":
                    encoding = Encoding.UTF8;
                    break;
                case "UTF7":
                    encoding = Encoding.UTF7;
                    break;
                case "UTF32":
                    encoding = Encoding.UTF32;
                    break;
                case "Unicode":
                    encoding = Encoding.Unicode;
                    break;
                case "BigEndianUnicode":
                    encoding = Encoding.BigEndianUnicode;
                    break;
                default:
                    break;
            }

            uint readTimeout, writeTimeout;
            if (!uint.TryParse(configItem.ReadTimeout, out readTimeout))
                readTimeout = 0;
            if (!uint.TryParse(configItem.WriteTimeout, out writeTimeout))
                writeTimeout = 0;

            return new ENTITIES.Communication.SerialPortParam()
            {
                Id = configItem.Id,
                PortName = configItem.PortName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                Encoding = encoding,
                ReadTimeout = readTimeout,
                WriteTimeout = writeTimeout,
                Category = configItem.Category
            };
        }
    }
}
