using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using TCell.Net;
using TCell.Mapping;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TCell.WindowsServicePlugins.TcpActor
{
    public class Actor : IServiceActor
    {
        #region properties
        public string Id
        {
            get { return "TcpActor"; }
        }

        private List<TcpClientCommander> TcpClients { get; set; }
        #endregion

        #region public functions
        public bool StartActor()
        {
            bool execResult = LoadConfiguration();
            if (execResult)
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            else
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} failed!");

            return execResult;
        }

        public bool StopActor()
        {
            try
            {
                if (TcpClients != null && TcpClients.Count > 0)
                {
                    foreach (TcpClientCommander client in TcpClients)
                        client.Stop();
                }

                PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
                return true;
            }
            catch (Exception ex)
            {
                PlayerHelper.LogException($"Exception occured when {Id} stop TCP clients, {ex.Message}", ex);
                return false;
            }
        }

        public bool ExecuteCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText) || TcpClients == null || TcpClients.Count == 0)
                return false;

            bool execResult = true;
            foreach (TcpClientCommander client in TcpClients)
            {
                if (client == null)
                    continue;

                try
                {
                    if (!client.IsConnected)
                    {
                        if (!client.Start())
                            continue;
                    }
                    client.Send(Encoding.UTF8.GetBytes(commandText));
                }
                catch (Exception ex)
                {
                    PlayerHelper.LogException($"Exception occurred when {Id} send command, {ex.Message}", ex);
                    execResult = false;
                }
            }

            return execResult;
        }
        #endregion

        #region private functions
        private bool LoadConfiguration()
        {
            List<EndPoint> endPoints = ConfigItemToEntity.MapNetEndpoints(ConfigurationHelper.GetIPEndPointsConfiguration("TcpActor"));
            if (endPoints == null || endPoints.Count == 0)
                return false;

            foreach (EndPoint ep in endPoints)
            {
                if (ep == null || ep.Protocol != System.Net.Sockets.ProtocolType.Tcp)
                    continue;

                if (TcpClients == null)
                    TcpClients = new List<TcpClientCommander>();

                TcpClients.Add(new TcpClientCommander(new EndpointPair()
                {
                    LocalEndPoint = null,
                    RemoteEndPoint = ep
                }));
            }

            return (TcpClients != null && TcpClients.Count > 0);
        }
        #endregion
    }
}
