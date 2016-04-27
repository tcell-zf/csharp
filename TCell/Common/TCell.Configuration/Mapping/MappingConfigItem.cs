using System.Configuration;

namespace TCell.Configuration.Mapping
{
    public class MappingConfigItem : ConfigurationElement
    {
        private const string IdAttribute = "Id";
        [ConfigurationProperty(IdAttribute, IsRequired = false)]
        public string Id
        {
            get { return (string)this[IdAttribute]; }
            set { base[IdAttribute] = value; }
        }

        private const string ValueAttribute = "Value";
        [ConfigurationProperty(ValueAttribute, IsRequired = true)]
        public string Value
        {
            get { return (string)this[ValueAttribute]; }
            set { base[ValueAttribute] = value; }
        }

        private const string CategoryAttribute = "Category";
        [ConfigurationProperty(CategoryAttribute, IsRequired = false)]
        public string Category
        {
            get { return (string)this[CategoryAttribute]; }
            set { base[CategoryAttribute] = value; }
        }
    }
}
