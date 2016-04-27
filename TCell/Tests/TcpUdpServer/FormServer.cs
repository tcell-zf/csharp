using System;
using System.Text;
using System.Windows.Forms;

using TCell.Net;
using TCell.Mapping;
using TCell.Logging;
using TCell.Abstraction;
using TCell.Configuration;
using TCell.Entities.Communication;

namespace TcpUdpServer
{
    public partial class FormServer : Form
    {
        private delegate void InvokeDelegate(string str);

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
            groupBoxProtocol.Enabled = true;
            groupBoxMessage.Enabled = false;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

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
            server.SetDatagramReceivedHandler(HandleDatagramReceived);
            try
            {
                if (server.Start())
                {
                    groupBoxProtocol.Enabled = false;
                    if (server is TcpServerCommander)
                        groupBoxMessage.Enabled = true;
                    else
                        groupBoxMessage.Enabled = false;
                    buttonStart.Enabled = false;
                    buttonStop.Enabled = true;
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Server started.{Environment.NewLine}{textBoxResults.Text}";
                }
                else
                {
                    groupBoxProtocol.Enabled = true;
                    groupBoxMessage.Enabled = false;
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                    textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Cannot started server!{Environment.NewLine}{textBoxResults.Text}";
                }
            }
            catch (Exception ex)
            {
                textBoxResults.Text = $"{DateTime.Now.ToString("hh:mm:ss")}: Start server exception {ex.Message}. {Environment.NewLine}{textBoxResults.Text}";
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
            if (server == null)
                return;
            if (!(server is TcpServerCommander))
                return;

            TcpServerCommander svr = server as TcpServerCommander;
            try
            {
                if (svr.Send(Encoding.UTF8.GetBytes(textBoxMessage.Text)))
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
