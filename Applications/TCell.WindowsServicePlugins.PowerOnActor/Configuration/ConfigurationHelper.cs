using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace TCell.WindowsServicePlugins.PowerOnActor.Configuration
{
    public class ConfigurationHelper
    {
        public static PowerOnConfigItem GetPowerOnConfiguration()
        {
            return GetPowerOnMachineConfig().PowerOn;
        }

        public static List<PowerOnMachineConfigItem> GetPowerOnMachineConfiguration()
        {
            List<PowerOnMachineConfigItem> machines = null;
            PowerOnCollection collection = GetPowerOnMachineConfig().Machines;
            if (collection != null && collection.Count > 0)
            {
                machines = collection.Cast<PowerOnMachineConfigItem>().ToList();
            }
            return machines;
        }

        private static PowerOnSection GetPowerOnMachineConfig()
        {
            return ConfigurationManager.GetSection(PowerOnSection.PowerOnConfiguration) as PowerOnSection;
        }
    }
}
