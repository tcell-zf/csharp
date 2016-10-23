using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        private Window owner = null;

        public string Id
        {
            get { return "VideoPlayer"; }
        }

        private string basePath = string.Empty;
        public string BasePath
        {
            set { this.basePath = value; }
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

            if (owner != null)
            {
                if (owner is Window)
                    this.owner = (Window)owner;
            }

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
            string path = FindFileByCategory(sourcePath, FileCategory.Video);
            if (currStatus == PlayerStatusType.Paused)
            {
                this.Play();
                currStatus = PlayerStatusType.Playing;
            }
            else if (currStatus == PlayerStatusType.Playing && string.Compare(path, SourcePath, true) == 0)
            {
                return true;
            }
            else
            {
                SourcePath = path;
                if (string.IsNullOrEmpty(SourcePath))
                {
                    this.Source = null;
                    this.Visibility = Visibility.Hidden;
                    currStatus = PlayerStatusType.Idle;
                }
                else
                {
                    if (!System.IO.File.Exists(SourcePath))
                    {
                        this.Source = null;
                        this.Visibility = Visibility.Hidden;
                        currStatus = PlayerStatusType.Idle;
                        return false;
                    }
                    FileCategory category = TCell.IO.File.GetFileCategory(SourcePath);
                    if (category != FileCategory.Audio && category != FileCategory.Video)
                    {
                        this.Source = null;
                        this.Visibility = Visibility.Hidden;
                        currStatus = PlayerStatusType.Idle;
                        return false;
                    }

                    this.Source = new Uri(SourcePath);
                    this.Visibility = Visibility.Visible;
                    this.Play();
                    MuteMedia(false);
                    currStatus = PlayerStatusType.Playing;

                    if (owner != null)
                    {
                        MethodInfo mi = owner.GetType().GetMethod("ShowMe");
                        if (mi != null)
                        {
                            mi.Invoke(owner, new object[] { true });
                        }
                    }
                }
            }
            return true;
        }

        private string FindFileByCategory(string filename, FileCategory category, string subfolder = "")
        {
            string path = string.Empty;
            if (System.IO.File.Exists(filename))
            {
                FileCategory cat = TCell.IO.File.GetFileCategory(filename);
                if (category == cat)
                    path = filename;
            }
            else
            {
                string basePath = string.Empty;
                if (string.IsNullOrEmpty(basePath) || !Directory.Exists(basePath))
                    basePath = string.IsNullOrEmpty(subfolder) ? Path.Combine(Environments.ApplicationPath, "MediaFiles")
                        : Path.Combine(Environments.ApplicationPath, "MediaFiles", subfolder);
                else
                    basePath = string.IsNullOrEmpty(subfolder) ? basePath
                        : Path.Combine(basePath, subfolder);

                if (Directory.Exists(basePath))
                {
                    string[] files = null;
                    try
                    {
                        files = Directory.GetFiles(basePath, Path.GetFileName(filename), SearchOption.TopDirectoryOnly);
                    }
                    catch (Exception ex)
                    {
                        PlayerHelper.LogException($"Exception occurred when searching {filename} of {category} with subfolder={subfolder}, {ex.Message}", ex);
                        return string.Empty;
                    }

                    if (files != null && files.Length > 0)
                    {
                        foreach (string f in files)
                        {
                            FileCategory cat = TCell.IO.File.GetFileCategory(f);
                            if (category != cat)
                                continue;

                            path = f;
                            break;
                        }
                    }
                }
            }

            return path.Replace("\\", "/");
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
