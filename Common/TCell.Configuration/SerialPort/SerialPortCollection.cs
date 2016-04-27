using System.Configuration;

namespace TCell.Configuration.SerialPort
{
    public class SerialPortCollection : ConfigurationElementCollection
    {
        public SerialPortConfigItem this[int index]
        {
            get { return base.BaseGet(index) as SerialPortConfigItem; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SerialPortConfigItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SerialPortConfigItem)element).ElementInformation;
        }
    }
}
