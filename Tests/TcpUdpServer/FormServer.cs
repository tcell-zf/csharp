using System;
using System.Windows.Forms;

using TCell.Net;
using TCell.Mapping;
using TCell.Logging;
using TCell.Abstraction;
using TCell.Entities.Net;
using TCell.Configuration;

namespace TcpUdpServer
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent();
        }

        private EndPoint tcpEp = null;
        private EndPoint udpEp = null;

        INetServer server = null;

        private void FormServer_Load(object sender, EventArgs e)
        {
            tcpEp = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointsConfiguration("TcpServer"));
            udpEp = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointsConfiguration("UdpServer"));

            ChangePortValue();

            Loggable.SetLogHandler(Log);
        }

        private void FormServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            ChangePortValue();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (server != null)
                server.Stop();

            if (radioButtonTcp.Checked)
            {
                tcpEp.Port = (uint)numericUpDownPort.Value;
                server = new TcpServerCommander(new EndpointPair()
                {
                    LocalEndPoint = tcpEp
                });
            }
            else
            {
                udpEp.Port = (uint)numericUpDownPort.Value;
                server = new UdpServerCommander(new EndpointPair()
                {
                    LocalEndPoint = udpEp
                });
            }
            server.Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void ChangePortValue()
        {
            if (radioButtonTcp.Checked)
            {
                if (tcpEp != null)
                    numericUpDownPort.Value = tcpEp.Port;
            }
            else
            {
                if (udpEp != null)
                    numericUpDownPort.Value = udpEp.Port;
            }
        }

        private void Stop()
        {
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }

        private void Log(string msg, Exception ex)
        {
            Logger.LoggerInstance.Log(msg, ex);
        }
    }
}
