using System;
using System.Text;
using System.IO.Ports;

namespace TCell.Entities.Communication
{
    public class SerialPortParam : EntityBase
    {
        public string PortName { get; set; }
        public uint BaudRate { get; set; }

        private uint dataBits;
        public uint DataBits
        {
            get { return this.dataBits; }
            set
            {
                if (!(5 <= value && value <= 8))
                    throw new ArgumentOutOfRangeException(nameof(DataBits), $"DataBits value should be between 5 to 8.");

                this.dataBits = value;
            }
        }

        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public Encoding Encoding { get; set; }
        public uint ReadTimeout { get; set; }
        public uint WriteTimeout { get; set; }
    }
}
