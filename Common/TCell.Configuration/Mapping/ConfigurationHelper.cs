using System.Linq;
using System.Configuration;
using System.Collections.Generic;

using TCell.Configuration.Mapping;

namespace Mexxum.Configuration
{
    public static partial class ConfigurationHelper
    {
        public static List<MappingConfigItem> GetMappingConfiguration()
        {
            List<MappingConfigItem> mappings = null;
            MappingCollection collection = GetMappingsConfig().MappingsConfigItem;
            if (collection != null && collection.Count > 0)
            {
                mappings = collection.Cast<MappingConfigItem>().ToList();
            }
            return mappings;
        }

        public static List<MappingConfigItem> GetMappingConfiguration(string category)
        {
            if (string.IsNullOrEmpty(category))
                return null;

            List<MappingConfigItem> items = GetMappingConfiguration();
            if (items == null || items.Count == 0)
                return null;

            return (from item in items
                    where string.Compare(category, item.Category, true) == 0
                    select item).ToList();
        }

        private static MappingsSection GetMappingsConfig()
        {
            return ConfigurationManager.GetSection(MappingsSection.MappingConfigurationElement) as MappingsSection;
        }
    }
}
