using System.Linq;
using System.Configuration;
using System.Collections.Generic;

using TCell.Configuration.IPEndPoint;

namespace TCell.Configuration
{
    public static partial class ConfigurationHelper
    {
        public static List<IPEndPointConfigItem> GetIPEndPointsConfiguration()
        {
            List<IPEndPointConfigItem> endPoints = null;
            IPEndPointCollection collection = GetIPEndPointsConfig().IPEndPointsConfigItem;
            if (collection != null && collection.Count > 0)
            {
                endPoints = collection.Cast<IPEndPointConfigItem>().ToList();
            }
            return endPoints;
        }

        public static IPEndPointConfigItem GetIPEndPointsConfiguration(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            List<IPEndPointConfigItem> items = GetIPEndPointsConfiguration();
            if (items == null || items.Count == 0)
                return null;

            return (from item in items
                    where string.Compare(id, item.Id, true) == 0
                    select item).SingleOrDefault();
        }

        private static IPEndPointsSection GetIPEndPointsConfig()
        {
            return ConfigurationManager.GetSection(IPEndPointsSection.IPEndPointsConfiguration) as IPEndPointsSection;
        }
    }
}
