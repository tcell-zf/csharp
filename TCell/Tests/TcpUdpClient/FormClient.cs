using System;
using System.Text;
using System.Windows.Forms;

using TCell.Net;
using TCell.Mapping;
using TCell.Logging;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TcpUdpClient
{
    public partial class FormClient : Form
    {
        private delegate void InvokeDelegate(string str);

        public FormClient()
        {
            InitializeComponent();
        }

        private EndPoint tcpEp = null;
        private EndPoint udpEp = null;

        INetClient client = null;

        private void FormClient_Load(object sender, System.EventArgs e)
        {
            tcpEp = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointConfiguration("TcpClient"));
            udpEp = ConfigItemToEntity.MapNetEndpoint(ConfigurationHelper.GetIPEndPointConfiguration("UdpClient"));

            ChangePortValue();
            groupBoxProtocol.Enabled = true;
            groupBoxMessage.Enabled = false;
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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (client != null)
                client.Stop();

            System.Net.IPAddress ip = null;
            if (!System.Net.IPAddress.TryParse(textBoxIp.Text, out ip))
                ip = null;

            if (radioButtonTcp.Checked)
            {
                if (ip != null)
                    tcpEp.IP = ip;
                tcpEp.Port = (uint)numericUpDownPort.Value;
                client = new TcpClientCommander(new EndpointPair()
                {
                    RemoteEndPoint = tcpEp
                });

                TcpClientCommander clt = client as TcpClientCommander;
                clt.HandleDatagramReceived = HandleDatagramReceived;
            }
            else
            {
                if (ip != null)
                    udpEp.IP = ip;
                udpEp.Port = (uint)numericUpDownPort.Value;
                client = new UdpClientCommander(new EndpointPair()
                {
                    RemoteEndPoint = udpEp
                });
            }

            try
            {
                if (client.Start())
                {
                    groupBoxProtocol.Enabled = false;
                    groupBoxMessage.Enabled = true;
                    buttonStart.Enabled = false;
                    buttonStop.Enabled = true;
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Client started.{Environment.NewLine}{textBoxResults.Text}";
                }
                else
                {
                    groupBoxProtocol.Enabled = true;
                    groupBoxMessage.Enabled = false;
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Cannot started client!{Environment.NewLine}{textBoxResults.Text}";
                }
            }
            catch (Exception ex)
            {
                textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Start client exception {ex.Message}.{Environment.NewLine}{textBoxResults.Text}";
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
            groupBoxProtocol.Enabled = true;
            groupBoxMessage.Enabled = false;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Server stopped.{Environment.NewLine}{textBoxResults.Text}";
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxMessage.Text))
                return;
            if (client == null)
                return;
            if (!client.IsConnected)
            {
                textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Connection lost!{Environment.NewLine}{textBoxResults.Text}";
                try
                {
                    if (!client.Start())
                        return;
                }
                catch (Exception ex)
                {
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Reconnect server exception, {ex.Message}.{Environment.NewLine}{textBoxResults.Text}";
                    return;
                }
            }

            try
            {
                if (client.Send(Encoding.UTF8.GetBytes(textBoxMessage.Text)))
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Datagram sent.{Environment.NewLine}{textBoxResults.Text}";
                else
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Datagram sent failed!{Environment.NewLine}{textBoxResults.Text}";
            }
            catch (Exception ex)
            {
                textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Send datagram exception {ex.Message}.{Environment.NewLine}{textBoxResults.Text}";
            }
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

        private void HandleDatagramReceived(byte[] dgram)
        {
            if (dgram == null || dgram.Length == 0)
                return;

            textBoxResults.Invoke(new InvokeDelegate(ShowResult), new object[] { Encoding.UTF8.GetString(dgram) });
        }

        private void ShowResult(string result)
        {
            textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Datagram received, {result}.{Environment.NewLine}{textBoxResults.Text}";
        }
    }
}
