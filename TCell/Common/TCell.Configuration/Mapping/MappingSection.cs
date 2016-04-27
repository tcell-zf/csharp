using System.Configuration;

namespace TCell.Configuration.Mapping
{
    public class MappingsSection : ConfigurationSection
    {
        public const string MappingConfigurationElement = "mappingsConfiguration";
        private const string MappingsElement = "mappings";

        [ConfigurationProperty(MappingsElement, IsDefaultCollection = true, IsRequired = true)]
        public MappingCollection MappingsConfigItem
        {
            get { return (MappingCollection)this[MappingsElement]; }
            set { base[MappingsElement] = value; }
        }
    }
}
