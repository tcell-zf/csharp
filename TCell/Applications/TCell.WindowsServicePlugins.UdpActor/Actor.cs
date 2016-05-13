using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using TCell.Net;
using TCell.Text;
using TCell.Mapping;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TCell.WindowsServicePlugins.UdpActor
{
    public class Actor : IServiceActor
    {
        #region properties
        public string Id
        {
            get { return "UdpActor"; }
        }

        private List<EndPoint> UdpEndpoints { get; set; }
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
            PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            return true;
        }

        public bool ExecuteCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText) || UdpEndpoints == null || UdpEndpoints.Count == 0)
                return false;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd != null)
            {
                if (cmd.Name == TextCommand.CommandName.Shutdown)
                    return false;
            }

            bool execResult = true;
            foreach (EndPoint ep in UdpEndpoints)
            {
                if (ep == null || ep.Protocol != System.Net.Sockets.ProtocolType.Udp)
                    continue;

                UdpClientCommander udpClient = new UdpClientCommander(new EndpointPair()
                {
                    LocalEndPoint = null,
                    RemoteEndPoint = ep
                });

                try
                {
                    if (udpClient.Start())
                    {
                        udpClient.Send(Encoding.UTF8.GetBytes(commandText));
                        udpClient.Stop();
                    }
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
            UdpEndpoints = ConfigItemToEntity.MapNetEndpoints(ConfigurationHelper.GetIPEndPointsConfiguration("UdpActor"));
            return true;
        }
        #endregion
    }
}
