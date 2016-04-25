using System;
using System.Windows.Forms;

using TCell.Net;
using TCell.Mapping;
using TCell.Logging;
using TCell.Abstraction;
using TCell.Entities.Net;
using TCell.Configuration;

namespace TcpUdpClient
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();
        }

        private EndPoint tcpEp = null;
        private EndPoint udpEp = null;

        INetClient client = null;

        private void FormClient_Load(object sender, System.EventArgs e)
        {
            tcpEp = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointsConfiguration("TcpClient"));
            udpEp = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointsConfiguration("UdpClient"));

            ChangePortValue();
            groupBoxProtocol.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            Loggable.SetLogHandler(Log);
        }

        private void FormClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            ChangePortValue();
        }

        private void Stop()
        {
            if (client != null)
            {
                client.Stop();
                client = null;
            }
        }

        private void Log(string msg, Exception ex)
        {
            Logger.LoggerInstance.Log(msg, ex);
        }

        private void ChangePortValue()
        {
            if (radioButtonTcp.Checked)
            {
                if (tcpEp != null)
                {
                    textBoxIp.Text = tcpEp.IP.ToString();
                    numericUpDownPort.Value = tcpEp.Port;
                }
            }
            else
            {
                if (udpEp != null)
                {
                    textBoxIp.Text = udpEp.IP.ToString();
                    numericUpDownPort.Value = udpEp.Port;
                }
            }
        }
    }
}
