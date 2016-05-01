using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using TCell.IO;
using TCell.Text;
using TCell.Abstraction;
using TCell.MediaPlayerPlugins.ImagePlayer.Configuration;

namespace TCell.MediaPlayerPlugins.ImagePlayer
{
    public class Player : Image, IPlayable
    {
        #region properties
        public string Id
        {
            get { return "ImagePlayer"; }
        }

        public string SourcePath { get; set; }

        private PlayerStatusType currStatus = PlayerStatusType.Idle;
        public PlayerStatusType Status
        {
            get { return currStatus; }
        }

        public Action<string, object> MediaActedHandler { get; set; }
        #endregion

        #region public functions
        public bool StartPlayer()
        {
            return LoadConfiguration();
        }

        public bool StopPlayer()
        {
            return true;
        }

        public bool ExecuteCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return true;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd == null)
                return true;

            bool execResult = true;
            switch (cmd.Name)
            {
                case TextCommand.CommandName.MediaPlay:
                    string path = cmd.GetParameterValue(TextCommand.ParameterName.Path);
                    execResult = PlayMedia(path);
                    break;
                case TextCommand.CommandName.MediaStop:
                    execResult = PlayMedia(string.Empty);
                    break;
                default:
                    this.Source = null;
                    this.Visibility = Visibility.Hidden;
                    break;
            }

            return execResult;
        }
        #endregion

        #region private functions
        private bool LoadConfiguration()
        {
            ImagePlayerConfigItem item = ConfigurationHelper.GetImagePlayerConfiguration();
            if (item != null && !string.IsNullOrEmpty(item.Stretch))
            {
                Stretch stretch = Stretch.None;
                if (!Enum.TryParse<Stretch>(item.Stretch, out stretch))
                    stretch = Stretch.None;

                this.Stretch = stretch;
            }
            return true;
        }

        private bool PlayMedia(string sourcePath)
        {
            SourcePath = sourcePath;
            if (string.IsNullOrEmpty(sourcePath))
            {
                this.Source = null;
                this.Visibility = Visibility.Hidden;
                currStatus = PlayerStatusType.Idle;
            }
            else
            {
                if (!System.IO.File.Exists(sourcePath))
                    return false;
                FileCategory category = File.GetFileCategory(sourcePath);
                if (category != FileCategory.Image)
                    return false;

                this.Source = BitmapFrame.Create(new Uri(sourcePath));
                this.Visibility = Visibility.Visible;
                currStatus = PlayerStatusType.Playing;
            }
            return true;
        }
        #endregion
    }
}
