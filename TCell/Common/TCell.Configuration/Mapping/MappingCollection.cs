using System.Configuration;

namespace TCell.Configuration.Mapping
{
    public class MappingCollection : ConfigurationElementCollection
    {
        public MappingConfigItem this[int index]
        {
            get { return base.BaseGet(index) as MappingConfigItem; }
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
            return new MappingConfigItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MappingConfigItem)element).ElementInformation;
        }
    }
}
