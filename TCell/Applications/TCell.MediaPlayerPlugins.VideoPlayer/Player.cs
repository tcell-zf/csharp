using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using TCell.IO;
using TCell.Text;
using TCell.Abstraction;

namespace TCell.MediaPlayerPlugins.VideoPlayer
{
    public class Player : MediaElement, IPlayable
    {
        #region properties
        public string Id
        {
            get { return "VideoPlayer"; }
        }

        public string SourcePath { get; set; }

        private PlayerStatusType currStatus = PlayerStatusType.Idle;
        public PlayerStatusType Status
        {
            get { return currStatus; }
        }

        public Action<MediaActedNotifier> MediaActedHandler { get; set; }
        #endregion

        #region public functions
        public bool StartPlayer(object owner)
        {
            currStatus = PlayerStatusType.Idle;

            LoadedBehavior = MediaState.Manual;
            this.MediaOpened += Player_MediaOpened;
            this.MediaEnded += Player_MediaEnded;
            this.MediaFailed += Player_MediaFailed;

            PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            return true;
        }

        public bool StopPlayer()
        {
            currStatus = PlayerStatusType.Idle;
            PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            return true;
        }

        public bool ExecuteCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return false;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd == null)
                return false;

            bool execResult = false;
            switch (cmd.Name)
            {
                case TextCommand.CommandName.MediaPlay:
                    string path = cmd.GetParameterValue(TextCommand.ParameterName.Path);
                    execResult = PlayMedia(path);
                    break;
                case TextCommand.CommandName.MediaStop:
                    execResult = PlayMedia(string.Empty);
                    break;
                case TextCommand.CommandName.MediaMute:
                    execResult = MuteMedia(true);
                    break;
                case TextCommand.CommandName.MediaUnmute:
                    execResult = MuteMedia(false);
                    break;
                case TextCommand.CommandName.MediaPause:
                    execResult = PauseMedia();
                    break;
                default:
                    this.Source = null;
                    this.Visibility = Visibility.Hidden;
                    currStatus = PlayerStatusType.Idle;
                    break;
            }

            return execResult;
        }
        #endregion

        #region private functions
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            currStatus = PlayerStatusType.Playing;

            if (MediaActedHandler == null)
                return;

            MediaActedHandler(new MediaActedNotifier()
            {
                Action = PlayerActionType.Opend,
                Id = this.Id,
                SourcePath = this.SourcePath,
                Param = null
            });
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.Source = null;
            this.Visibility = Visibility.Hidden;
            currStatus = PlayerStatusType.Idle;

            if (MediaActedHandler == null)
                return;

            MediaActedHandler(new MediaActedNotifier()
            {
                Action = PlayerActionType.Ended,
                Id = this.Id,
                SourcePath = this.SourcePath,
                Param = null
            });
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.Source = null;
            this.Visibility = Visibility.Hidden;
            currStatus = PlayerStatusType.Idle;
            PlayerHelper.LogException($"Exception occured when play {SourcePath}, {e.ErrorException.Message}", e.ErrorException);

            if (MediaActedHandler == null)
                return;

            MediaActedHandler(new MediaActedNotifier()
            {
                Action = PlayerActionType.Failed,
                Id = this.Id,
                SourcePath = this.SourcePath,
                Param = e.ErrorException
            });
        }

        private bool PlayMedia(string sourcePath)
        {
            if (currStatus == PlayerStatusType.Paused)
            {
                this.Play();
                currStatus = PlayerStatusType.Playing;
            }

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
                {
                    this.Source = null;
                    this.Visibility = Visibility.Hidden;
                    currStatus = PlayerStatusType.Idle;
                    return false;
                }
                FileCategory category = File.GetFileCategory(sourcePath);
                if (category != FileCategory.Audio && category != FileCategory.Video)
                {
                    this.Source = null;
                    this.Visibility = Visibility.Hidden;
                    currStatus = PlayerStatusType.Idle;
                    return false;
                }

                this.Source = new Uri(sourcePath);
                this.Visibility = Visibility.Visible;
                this.Play();
                MuteMedia(false);
            }
            return true;
        }

        private bool MuteMedia(bool isMute)
        {
            this.IsMuted = isMute;
            return (this.IsMuted == isMute);
        }

        private bool PauseMedia()
        {
            this.Pause();
            currStatus = PlayerStatusType.Paused;
            return true;
        }
        #endregion
    }
}
