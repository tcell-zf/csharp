using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using TCell.Net;

namespace TCell.UniversalMediaPlayer.ShellCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Not enough parameters!");
                return;
            }

            UmpArgument argument = null;
            foreach (string arg in args)
            {
                // parse protocol
                ProtocolType protocol = ParseProtocol(arg);
                if (protocol != ProtocolType.Unknown)
                {
                    if (argument == null)
                        argument = new UmpArgument();
                    if (argument.EndPoint == null)
                        argument.EndPoint = new Entities.Communication.EndPoint();

                    argument.EndPoint.Protocol = protocol;
                }
                else
                {
                    Entities.Communication.EndPoint ep = ParseEndPoint(arg);
                    if (ep != null)
                    {
                        if (argument == null)
                            argument = new UmpArgument();
                        if (argument.EndPoint == null)
                            argument.EndPoint = new Entities.Communication.EndPoint();

                        argument.EndPoint.IP = ep.IP;
                        argument.EndPoint.Port = ep.Port;
                    }
                    else
                    {
                        string cmd = ParseCommand(arg);
                        if (!string.IsNullOrEmpty(cmd))
                        {
                            if (argument == null)
                                argument = new UmpArgument();

                            argument.Command = cmd;
                        }
                    }
                }
            }

            if (argument == null)
            {
                Console.WriteLine("Invalid parameters!");
                return;
            }

            try
            {
                INetClient client = StartNetClient(argument.EndPoint);
                if (client != null)
                {
                    if (client.Start())
                    {
                        client.Send(Encoding.UTF8.GetBytes(argument.Command));

                        client.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred when execute command, {ex.Message}");
            }

            //Thread.Sleep(5000);
            //Console.WriteLine("Press any key to continue ...");
            //Console.Read();
        }

        static private ProtocolType ParseProtocol(string arg)
        {
            ProtocolType type = ProtocolType.Unknown;

            if (string.IsNullOrEmpty(arg))
                return type;

            string[] strArr = arg.ToLower().Split(new char[] { '/' });
            if (strArr == null || strArr.Length != 2)
                return type;

            if (string.Compare(strArr[0], "protocol") != 0)
                return type;

            switch (strArr[1])
            {
                case "tcp":
                    type = ProtocolType.Tcp;
                    break;
                case "udp":
                    type = ProtocolType.Udp;
                    break;
                default:
                    break;
            }

            return type;
        }

        static private Entities.Communication.EndPoint ParseEndPoint(string arg)
        {
            Entities.Communication.EndPoint ep = null;

            if (string.IsNullOrEmpty(arg))
                return ep;

            string[] strArr = arg.ToLower().Split(new char[] { '/' });
            if (strArr == null || strArr.Length != 2)
                return ep;

            if (string.Compare(strArr[0], "ip") != 0)
                return ep;

            strArr = strArr[1].Split(new char[] { ':' });
            if (strArr == null || strArr.Length != 2)
                return ep;

            IPAddress ip;
            uint port;
            if (IPAddress.TryParse(strArr[0], out ip)
                && uint.TryParse(strArr[1], out port))
            {
                ep = new Entities.Communication.EndPoint()
                {
                    IP = ip,
                    Port = port
                };
            }

            return ep;
        }

        static private string ParseCommand(string arg)
        {
            string cmd = string.Empty;

            if (string.IsNullOrEmpty(arg))
                return cmd;

            string[] strArr = arg.Split(new char[] { '/' });
            if (strArr == null || strArr.Length != 2)
                return cmd;

            if (string.Compare(strArr[0], "cmd", true) != 0)
                return cmd;

            cmd = strArr[1];

            return cmd;
        }

        static private INetClient StartNetClient(Entities.Communication.EndPoint ep)
        {
            if (ep == null)
                return null;

            INetClient client = null;
            switch (ep.Protocol)
            {
                case ProtocolType.Tcp:
                    client = new TcpClientCommander(new EndpointPair()
                    {
                        RemoteEndPoint = ep
                    });
                    ((TcpClientCommander)client).HandleDatagramReceived = HandleDatagramReceived;
                    break;
                case ProtocolType.Udp:
                    client = new UdpClientCommander(new EndpointPair()
                    {
                        RemoteEndPoint = ep
                    });
                    break;
                default:
                    break;
            }

            return client;
        }

        static private void HandleDatagramReceived(byte[] dgram)
        {
            if (dgram == null || dgram.Length == 0)
                return;

            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss")}: Datagram received, {Encoding.UTF8.GetString(dgram)}");
        }
    }

    internal class UmpArgument
    {
        public Entities.Communication.EndPoint EndPoint { get; set; }
        public string Command { get; set; }

    }
}
