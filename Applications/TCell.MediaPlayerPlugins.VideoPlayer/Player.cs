using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using TCell.Text;
using TCell.Abstraction;

namespace TCell.MediaPlayerPlugins.VideoPlayer
{
    public class Player : MediaElement, IPlayable
    {
        #region properties
        private Action<string, Exception> exceptionLogHandler = null;
        private Action<TraceEventType, string> eventLogHandler = null;

        public string Id
        {
            get { return "VideoPlayer"; }
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

        public bool StartPlayer()
        {
            return true;
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
                    break;
            }

            return execResult;
        }
        #endregion

        #region private functions
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
                this.Source = new Uri(sourcePath);
                this.Visibility = Visibility.Visible;
                this.Play();
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
