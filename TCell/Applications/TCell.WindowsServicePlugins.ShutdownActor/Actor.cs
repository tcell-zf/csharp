using System;
using System.Text;
using System.Diagnostics;

using TCell.Net;
using TCell.Text;
using TCell.Mapping;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TCell.WindowsServicePlugins.ShutdownActor
{
    public class Actor : IServiceActor
    {
        #region properties
        public string Id
        {
            get { return "ShutdownActor"; }
        }

        private EndPoint BroadcastEndpoint { get; set; }
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
            if (string.IsNullOrEmpty(commandText) || BroadcastEndpoint == null)
                return false;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd == null)
                return false;

            bool execResult = false;
            if (cmd.Name == TextCommand.CommandName.Shutdown)
            {
                UdpClientCommander udpClient = new UdpClientCommander(new EndpointPair()
                {
                    LocalEndPoint = null,
                    RemoteEndPoint = BroadcastEndpoint
                });

                try
                {
                    if (udpClient.Start())
                    {
                        udpClient.Send(Encoding.UTF8.GetBytes(cmd.ToString()));
                        udpClient.Stop();

                        execResult = false;
                    }
                }
                catch (Exception ex)
                {
                    PlayerHelper.LogException($"Exception occurred when {Id} send shutdown command, {ex.Message}", ex);
                    execResult = false;
                }
            }

            return execResult;
        }
        #endregion

        #region private functions
        private bool LoadConfiguration()
        {
            BroadcastEndpoint = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointsConfiguration("uniServiceUdpBroadcast"));

            return (BroadcastEndpoint != null);
        }
        #endregion
    }
}
