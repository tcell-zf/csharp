using System.Configuration;

namespace TCell.Configuration.IPEndPoint
{
    public class IPEndPointConfigItem : ConfigurationElement
    {
        private const string IdAttribute = "Id";
        [ConfigurationProperty(IdAttribute, IsRequired = false)]
        public string Id
        {
            get { return (string)this[IdAttribute]; }
            set { base[IdAttribute] = value; }
        }

        private const string ProtocolAttribute = "Protocol";
        [ConfigurationProperty(ProtocolAttribute, IsRequired = true)]
        public string Protocol
        {
            get { return (string)this[ProtocolAttribute]; }
            set { base[ProtocolAttribute] = value; }
        }

        private const string IPAttribute = "IP";
        [ConfigurationProperty(IPAttribute, IsRequired = false)]
        public string IP
        {
            get { return (string)this[IPAttribute]; }
            set { base[IPAttribute] = value; }
        }

        private const string PortAttribute = "Port";
        [ConfigurationProperty(PortAttribute, IsRequired = true)]
        public string Port
        {
            get { return (string)this[PortAttribute]; }
            set { base[PortAttribute] = value; }
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

        private const string BufferLengthAttribute = "BufferLength";
        [ConfigurationProperty(BufferLengthAttribute, IsRequired = false)]
        public string BufferLength
        {
            get { return (string)this[BufferLengthAttribute]; }
            set { base[BufferLengthAttribute] = value; }
        }

        new public string ToString()
        {
            return $"IPEndPointConfigItem: {nameof(Id)}={Id},{nameof(Protocol)}={Protocol},{nameof(IP)}={IP},{nameof(Port)}={Port},{nameof(ReadTimeout)}={ReadTimeout},{nameof(WriteTimeout)}={WriteTimeout}";
        }
    }
}
