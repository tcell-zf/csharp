using System.Configuration;

namespace TCell.Configuration.IPEndPoint
{
    public class IPEndPointCollection : ConfigurationElementCollection
    {
        public IPEndPointConfigItem this[int index]
        {
            get { return base.BaseGet(index) as IPEndPointConfigItem; }
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
            return new IPEndPointConfigItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IPEndPointConfigItem)element).ElementInformation;
        }
    }
}
