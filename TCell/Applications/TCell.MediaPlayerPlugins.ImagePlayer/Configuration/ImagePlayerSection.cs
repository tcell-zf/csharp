using System.Configuration;

namespace TCell.MediaPlayerPlugins.ImagePlayer.Configuration
{
    public class ImagePlayerSection : ConfigurationSection
    {
        public const string ImagePlayerConfiguration = "imagePlayerConfiguration";
        private const string ImagePlayerString = "imagePlayer";

        [ConfigurationProperty(ImagePlayerString, IsDefaultCollection = true, IsRequired = false)]
        public ImagePlayerConfigItem ImagePlayer
        {
            get { return (ImagePlayerConfigItem)this[ImagePlayerString]; }
        }
    }
}
