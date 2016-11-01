using System;
using System.Text;
using System.Diagnostics;

using TCell.Net;
using TCell.Mapping;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TCell.WindowsServicePlugins.TcpCommand
{
    public class Receiver : IReceivable, IStringCommandReceivable
    {
        #region properties
        private TcpServerCommander tcpSvr = null;

        public string Id
        {
            get { return "TcpCommandReceiver"; }
        }

        public Action<string, string> StringCommandReceivedHandler { get; set; }
        #endregion

        #region public functions
        public bool StartReceiver()
        {
            bool execResult = false;
            try
            {
                EndPoint ep = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointConfiguration("uniServiceTcpLocal"));
                if (ep != null)
                {
                    tcpSvr = new TcpServerCommander(new EndpointPair()
                    {
                        LocalEndPoint = ep,
                        RemoteEndPoint = null
                    });

                    tcpSvr.SetDatagramReceivedHandler(OnDatagramReceived);
                    execResult = tcpSvr.Start();
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

        public bool StopReceiver()
        {
            bool execResult = false;
            if (tcpSvr != null)
            {
                execResult = tcpSvr.Stop();
                tcpSvr = null;
            }

            if (execResult)
                PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            else
                PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} failed!");

            return execResult;
        }

        public bool Send(string response)
        {
            if (tcpSvr == null || string.IsNullOrEmpty(response))
                return false;

            try
            {
                return tcpSvr.Send(Encoding.UTF8.GetBytes(response));
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
            if (dgram == null || dgram.Length == 0 || StringCommandReceivedHandler == null)
                return;

            StringCommandReceivedHandler(Id, Encoding.UTF8.GetString(dgram));
        }
        #endregion
    }
}
