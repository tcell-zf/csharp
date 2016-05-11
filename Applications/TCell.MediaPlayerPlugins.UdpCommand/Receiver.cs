using System;
using System.Text;
using System.Diagnostics;

using TCell.Net;
using TCell.Mapping;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TCell.MediaPlayerPlugins.UdpCommand
{
    public class Receiver : IReceivable
    {
        #region properties
        private UdpServerCommander udpSvr = null;

        public string Id
        {
            get { return "UdpCommandReceiver"; }
        }

        public Action<string, string> CommandReceivedHandler { get; set; }
        #endregion

        #region public functions
        public bool StartReceiver()
        {
            bool execResult = false;
            try
            {
                EndPoint ep = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointConfiguration("mediaPlayerUdpLocal"));
                if (ep != null)
                {
                    udpSvr = new UdpServerCommander(new EndpointPair()
                    {
                        LocalEndPoint = ep,
                        RemoteEndPoint = null
                    });

                    udpSvr.SetDatagramReceivedHandler(OnDatagramReceived);
                    execResult = udpSvr.Start();
                }
            }
            catch (Exception ex)
            {
                PlayerHelper.LogException($"Exception occurred when start {Id}, {ex.Message}", ex);
                execResult = false;
            }

            if (execResult)
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            else
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} failed!");

            return execResult;
        }

        public bool StopRrceiver()
        {
            bool execResult = false;
            if (udpSvr != null)
            {
                execResult = udpSvr.Stop();
                udpSvr = null;
            }

            if (execResult)
                PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            else
                PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} failed!");

            return execResult;
        }

        public bool Send(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            EndPoint ep = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointConfiguration("mediaPlayerUdpRemote"));
            if (ep == null)
                return false;
            try
            {
                UdpClientCommander udpClient = new UdpClientCommander(new EndpointPair()
                {
                    LocalEndPoint = null,
                    RemoteEndPoint = ep
                });
                if (!udpClient.Start())
                    return false;

                return udpClient.Send(Encoding.UTF8.GetBytes(response));
            }
            catch (Exception ex)
            {
                PlayerHelper.LogException($"Exception occurred when send {Id} datagram, {ex.Message}", ex);
                return false;
            }
        }
        #endregion

        #region private functions
        private void OnDatagramReceived(byte[] dgram)
        {
            if (dgram == null || dgram.Length == 0 || CommandReceivedHandler == null)
                return;

            CommandReceivedHandler(Id, Encoding.UTF8.GetString(dgram));
        }
        #endregion
    }
}
