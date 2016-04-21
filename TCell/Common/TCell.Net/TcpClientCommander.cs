﻿using System;
using System.Net.Sockets;

namespace TCell.Net
{
    public class TcpClientCommander : NetCommander, INetCommander, INetClient
    {
        public Action<byte[]> HandleDatagramReceived = null;

        public TcpClientCommander(EndpointPair endpoints)
            : base(endpoints) { }

        private TcpClient tcpClient;

        public bool IsConnected
        {
            get
            {
                bool connected = false;
                try
                {
                    if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                    {
                        /* pear to the documentation on Poll:
                         * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                         * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                         * -or- true if data is available for reading; 
                         * -or- true if the connection has been closed, reset, or terminated; 
                         * otherwise, returns false
                         */
                        // Detect if client disconnected
                        if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (tcpClient.Client.Receive(buff, SocketFlags.Peek) != 0)
                            {
                                connected = true;
                            }
                        }

                        connected = true;
                    }
                }
                catch
                {
                    connected = false;
                }
                return connected;
            }
        }

        public bool Start()
        {
            if (EndPoints == null)
                return false;
            if (EndPoints.RemoteEndPoint == null || EndPoints.RemoteEndPoint.Protocol != ProtocolType.Tcp)
                return false;
            if (EndPoints.LocalEndPoint != null && EndPoints.LocalEndPoint.Protocol != ProtocolType.Tcp)
                return false;

            return Connect();
        }

        public bool Stop()
        {
            if (IsConnected)
            {
                tcpClient.Close();
                tcpClient = null;
            }

            return true;
        }

        public bool Send(byte[] dgram)
        {
            if (dgram == null || dgram.Length == 0)
                throw new ArgumentNullException(nameof(dgram));

            if (!IsConnected && !Connect())
            {
                throw new SocketException();
            }

            bool sent = false;
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                //System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream);
                //System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
                //writer.Write(packet);
                //byte[] buf = new byte[BufferLength];
                //int len = reader.Read(buf, 0, BufferLength);
                //return true;

                if (stream.CanWrite)
                {
                    if (EndPoints.RemoteEndPoint.WriteTimeout != null)
                        stream.WriteTimeout = (int)EndPoints.RemoteEndPoint.WriteTimeout;
                    stream.Write(dgram, 0, dgram.Length);
                    stream.Flush();

                    if (stream.CanRead && stream.DataAvailable)
                    {
                        if (EndPoints.RemoteEndPoint.ReadTimeout != null)
                            stream.ReadTimeout = (int)EndPoints.RemoteEndPoint.ReadTimeout;
                        int bufLength = EndPoints.RemoteEndPoint.BufferLength == null ? 512 : (int)EndPoints.RemoteEndPoint.BufferLength.Value;
                        byte[] buf = new byte[bufLength];
                        int len = stream.Read(buf, 0, bufLength);
                        if (0 < len || len <= bufLength)
                        {
                            byte[] receivedDgram = new byte[len];
                            for (int i = 0; i < len; i++)
                            {
                                receivedDgram[i] = buf[i];
                            }
                            stream.Flush();

                            HandleDatagramReceived?.Invoke(receivedDgram);

                            sent = true;
                        }
                    }
                    else
                    {
                        sent = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TcpClientCommander.LogException($"Send TCP datagram failed: {ex.Message}", ex);
                sent = false;
            }
            return sent;
        }

        private bool Connect()
        {
            if (EndPoints.LocalEndPoint == null)
                tcpClient = new TcpClient();
            else
                tcpClient = new TcpClient(EndPoints.GetLocalNetEndPoint());

            tcpClient.Connect(EndPoints.GetRemoteNetEndPoint());
            return tcpClient.Connected;
        }
    }
}
