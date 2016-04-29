using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using TCell.Text;
using TCell.Abstraction;
using TCell.MediaPlayerPlugins.ImagePlayer.Configuration;

namespace TCell.MediaPlayerPlugins.ImagePlayer
{
    public class Player : Image, IPlayable
    {
        #region properties
        private Action<string, Exception> exceptionLogHandler = null;
        private Action<TraceEventType, string> eventLogHandler = null;

        public string Id
        {
            get { return "ImagePlayer"; }
        }

        public string SourcePath { get; set; }
        #endregion

        #region public functions
        public void SetLogHandler(Action<string, Exception> handler)
        {
            if (handler == null)
                exceptionLogHandler = null;
            else
                exceptionLogHandler += handler;
        }

        public void SetLogHandler(Action<TraceEventType, string> handler)
        {
            if (handler == null)
                eventLogHandler = null;
            else
                eventLogHandler += handler;
        }

        public bool Start()
        {
            return LoadConfiguration();
        }

        public bool Stop()
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
            }
            else
            {
                this.Source = BitmapFrame.Create(new Uri(sourcePath));
                this.Visibility = Visibility.Visible;
            }
            return true;
        }

        private void LogMessage(TraceEventType evt, string msg)
        {
            if (eventLogHandler != null)
            {
                eventLogHandler(evt, msg);
            }
        }

        private void LogException(string msg, Exception ex)
        {
            if (exceptionLogHandler != null)
            {
                exceptionLogHandler(msg, ex);
            }
        }
        #endregion
    }
}
