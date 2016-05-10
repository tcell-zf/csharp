using System.Configuration;

namespace TCell.WindowsServicePlugins.PowerOnActor.Configuration
{
    public class PowerOnSection : ConfigurationSection
    {
        public const string PowerOnConfiguration = "powerOnMachineConfiguration";
        private const string PowerOnString = "powerOn";
        private const string PowerOnMachinesString = "machines";

        [ConfigurationProperty(PowerOnString, IsDefaultCollection = false, IsRequired = false)]
        public PowerOnConfigItem PowerOn
        {
            get { return (PowerOnConfigItem)this[PowerOnString]; }
        }

        [ConfigurationProperty(PowerOnMachinesString, IsDefaultCollection = false, IsRequired = false)]
        public PowerOnCollection Machines
        {
            get { return (PowerOnCollection)this[PowerOnMachinesString]; }
        }
    }
}
