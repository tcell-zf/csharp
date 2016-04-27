using System.Configuration;

namespace TCell.Configuration.SerialPort
{
    public class SerialPortConfigItem : ConfigurationElement
    {
        private const string IdAttribute = "Id";
        [ConfigurationProperty(IdAttribute, IsRequired = false)]
        public string Id
        {
            get { return (string)this[IdAttribute]; }
            set { base[IdAttribute] = value; }
        }

        private const string PortNameAttribute = "PortName";
        [ConfigurationProperty(PortNameAttribute, IsRequired = true)]
        public string PortName
        {
            get { return (string)this[PortNameAttribute]; }
            set { base[PortNameAttribute] = value; }
        }

        private const string BaudRateAttribute = "BaudRate";
        [ConfigurationProperty(BaudRateAttribute, IsRequired = false)]
        public string BaudRate
        {
            get { return (string)this[BaudRateAttribute]; }
            set { base[BaudRateAttribute] = value; }
        }

        private const string DataBitsAttribute = "DataBits";
        [ConfigurationProperty(DataBitsAttribute, IsRequired = false)]
        public string DataBits
        {
            get { return (string)this[DataBitsAttribute]; }
            set { base[DataBitsAttribute] = value; }
        }

        private const string ParityAttribute = "Parity";
        [ConfigurationProperty(ParityAttribute, IsRequired = false)]
        public string Parity
        {
            get { return (string)this[ParityAttribute]; }
            set { base[ParityAttribute] = value; }
        }

        private const string StopBitsAttribute = "StopBits";
        [ConfigurationProperty(StopBitsAttribute, IsRequired = false)]
        public string StopBits
        {
            get { return (string)this[StopBitsAttribute]; }
            set { base[StopBitsAttribute] = value; }
        }

        private const string EncodingAttribute = "Encoding";
        [ConfigurationProperty(EncodingAttribute, IsRequired = false)]
        public string Encoding
        {
            get { return (string)this[EncodingAttribute]; }
            set { base[EncodingAttribute] = value; }
        }

        private const string ReadTimeoutAttribute = "ReadTimeout";
        [ConfigurationProperty(ReadTimeoutAttribute, IsRequired = false)]
        public string ReadTimeout
        {
            get { return (string)this[ReadTimeoutAttribute]; }
            set { base[ReadTimeoutAttribute] = value; }
        }

        private const string WriteTimeoutAttribute = "WriteTimeout";
        [ConfigurationProperty(WriteTimeoutAttribute, IsRequired = false)]
        public string WriteTimeout
        {
            get { return (string)this[WriteTimeoutAttribute]; }
            set { base[WriteTimeoutAttribute] = value; }
        }
    }
}
