using System.Configuration;

namespace TCell.MediaPlayerPlugins.ImagePlayer.Configuration
{
    public class ConfigurationHelper
    {
        public static ImagePlayerConfigItem GetImagePlayerConfiguration()
        {
            return GetImagePlayerConfig().ImagePlayer;
        }

        private static ImagePlayerSection GetImagePlayerConfig()
        {
            return ConfigurationManager.GetSection(ImagePlayerSection.ImagePlayerConfiguration) as ImagePlayerSection;
        }
    }
}
