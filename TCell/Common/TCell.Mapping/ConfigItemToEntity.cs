using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using TCell.Abstraction;
using ENTITIES = TCell.Entities.Net;
using TCell.Configuration.IPEndPoint;

namespace TCell.Mapping
{
    public class ConfigItemToEntity : Loggable
    {
        static public ENTITIES.EndPoint MapNetEndpoint(IPEndPointConfigItem configItem)
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

            uint timeout;
            uint? readTimeout = null;
            if (!string.IsNullOrEmpty(configItem.ReadTimeout))
            {
                if (uint.TryParse(configItem.ReadTimeout, out timeout))
                    readTimeout = timeout;
            }

            uint? writeTimeout = null;
            if (!string.IsNullOrEmpty(configItem.WriteTimeout))
            {
                if (uint.TryParse(configItem.WriteTimeout, out timeout))
                    writeTimeout = timeout;
            }

            ENTITIES.EndPoint endPoint = null;
            try
            {
                endPoint = new ENTITIES.EndPoint()
                {
                    Id = configItem.Id,
                    Protocol = protocol,
                    IP = ip,
                    Port = port,
                    ReadTimeout = readTimeout,
                    WriteTimeout = writeTimeout
                };
            }
            catch (Exception ex)
            {
                LogException($"Failed to map [{configItem}] to {nameof(ENTITIES.EndPoint)}.", ex);
            }
            return endPoint;
        }

        static public List<ENTITIES.EndPoint> MapNetEndpoints(List<IPEndPointConfigItem> configItems)
        {
            if (configItems == null || configItems.Count == 0)
                return null;

            List<ENTITIES.EndPoint> endPoints = new List<ENTITIES.EndPoint>();
            foreach (IPEndPointConfigItem configItem in configItems)
            {
                ENTITIES.EndPoint endPoint = null;
                try
                {
                    endPoint = MapNetEndpoint(configItem);
                }
                catch (Exception ex)
                {
                    LogException($"Failed to map [{configItem}] to {nameof(ENTITIES.EndPoint)} in endpoint list.", ex);
                }

                if (endPoint != null)
                    endPoints.Add(endPoint);
            }
            return endPoints;
        }
    }
}
