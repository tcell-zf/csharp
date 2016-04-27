using System.Text;
using System.IO.Ports;

namespace TCell.Entities.Communication
{
    public class SerialPortParam : EntityBase
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public Encoding Encoding { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
    }
}
