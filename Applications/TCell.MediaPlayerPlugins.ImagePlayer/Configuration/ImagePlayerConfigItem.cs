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
    }
}
