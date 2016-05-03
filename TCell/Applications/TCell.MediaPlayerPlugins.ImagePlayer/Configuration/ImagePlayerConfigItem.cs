using System.Configuration;

namespace TCell.MediaPlayerPlugins.ImagePlayer.Configuration
{
    public class ImagePlayerConfigItem : ConfigurationElement
    {
        private const string StretchAttribute = "Stretch";
        [ConfigurationProperty(StretchAttribute, IsRequired = false)]
        public string Stretch
        {
            get { return (string)this[StretchAttribute]; }
            set { base[StretchAttribute] = value; }
        }

        private const string PlayIntervalAttribute = "PlayInterval";
        [ConfigurationProperty(PlayIntervalAttribute, IsRequired = false)]
        public string PlayInterval
        {
            get { return (string)this[PlayIntervalAttribute]; }
            set { base[PlayIntervalAttribute] = value; }
        }
    }
}
