using System.Linq;
using System.Collections.Generic;

namespace TCell.Entities
{
    public abstract class EntityBase
    {
        public string Id { get; set; }
    }

    public static partial class ConfigurationHelper
    {
        public static T SearchConfigSetting<T>(List<T> entities, string id)
            where T : EntityBase
        {
            T item = (from s in entities
                      where string.Compare(s.Id, id, false) == 0
                      select s).SingleOrDefault();

            if (item == null)
                return default(T);
            if (item.GetType() != typeof(T))
                return default(T);

            return item;
        }
    }
}
