using System.Configuration;

namespace TCell.WindowsServicePlugins.PowerOnActor.Configuration
{
    public class PowerOnConfigItem : ConfigurationElement
    {
        private const string NaryAttribute = "N-Nary";
        [ConfigurationProperty(NaryAttribute, IsRequired = false)]
        public string Nary
        {
            get { return (string)this[NaryAttribute]; }
            set { base[NaryAttribute] = value; }
        }
    }

    public class PowerOnMachineConfigItem : ConfigurationElement
    {
        private const string NameAttribute = "Name";
        [ConfigurationProperty(NameAttribute, IsRequired = true)]
        public string Name
        {
            get { return (string)this[NameAttribute]; }
            set { base[NameAttribute] = value; }
        }

        private const string MacAttribute = "Mac";
        [ConfigurationProperty(MacAttribute, IsRequired = true)]
        public string Mac
        {
            get { return (string)this[MacAttribute]; }
            set { base[MacAttribute] = value; }
        }
    }
}
