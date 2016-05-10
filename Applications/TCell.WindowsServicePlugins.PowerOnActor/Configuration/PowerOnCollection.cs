using System.Configuration;

namespace TCell.WindowsServicePlugins.PowerOnActor.Configuration
{
    public class PowerOnCollection : ConfigurationElementCollection
    {
        public PowerOnMachineConfigItem this[int index]
        {
            get { return base.BaseGet(index) as PowerOnMachineConfigItem; }
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
            return new PowerOnMachineConfigItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PowerOnMachineConfigItem)element).ElementInformation;
        }
    }
}
