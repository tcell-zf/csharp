using System.Text;
using System.IO.Ports;

namespace TCell.Entities.Communication
{
    public class SerialPortParam : EntityBase
    {
        public string PortName { get; set; }
        public uint BaudRate { get; set; }
        public uint DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public Encoding Encoding { get; set; }
        public uint ReadTimeout { get; set; }
        public uint WriteTimeout { get; set; }
    }
}
