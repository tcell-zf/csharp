using System;
using IO = System.IO.Ports;

using TCell.Abstraction;
using TCell.Entities.Communication;

namespace TCell.SerialPort
{
    public class SerialPortCommander : Loggable
    {
        public Action<string> OnDataReceived = null;

        private IO.SerialPort port = null;
        private SerialPortParam config = null;

        public bool IsOpen
        {
            get { return (port != null && port.IsOpen); }
        }

        public SerialPortCommander(SerialPortParam config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            this.config = config;
        }

        public bool Start()
        {
            if (IsOpen)
                return true;

            if (port == null)
            {
                port = new IO.SerialPort()
                {
                    PortName = config.PortName,
                    BaudRate = (int)config.BaudRate,
                    DataBits = (int)config.DataBits,
                    Parity = config.Parity,
                    StopBits = config.StopBits,
                    Encoding = config.Encoding,
                    ReadTimeout = (int)config.ReadTimeout,
                    WriteTimeout = (int)config.WriteTimeout
                };

                port.DataReceived += Port_DataReceived;
            }
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    port = null;
                    SerialPortCommander.LogException($"Open serial port failed: {ex.Message}", ex);
                    return false;
                }
            }

            return true;
        }

        public bool Send(string text)
        {
            if (!Start())
                return false;

            try
            {
                port.Write(text);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Send(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return false;
            if (!Start())
                return false;

            try
            {
                port.Write(bytes, 0, bytes.Length);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void Stop()
        {
            if (port != null)
            {
                port.Close();
                port = null;
            }
        }

        private void Port_DataReceived(object sender, IO.SerialDataReceivedEventArgs e)
        {
            if (OnDataReceived != null)
            {
                try
                {
                    OnDataReceived(port.ReadExisting());
                }
                catch { }
            }
        }
    }
}
