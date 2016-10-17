using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
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

        private string basePath = string.Empty;
        public string BasePath
        {
            set { this.basePath = value; }
        }
        public string SourcePath { get; set; }

        private int? playInterval = null;
        private DateTime latestPlayDateTime = DateTime.Now;
        private DispatcherTimer checkIntervalTimer = null;

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

            bool execResult = LoadConfiguration();
            if (execResult)
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            else
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} failed!");

            return execResult;
        }

        public bool StopPlayer()
        {
            currStatus = PlayerStatusType.Idle;
            if (checkIntervalTimer != null)
            {
                checkIntervalTimer.Stop();
                checkIntervalTimer = null;
            }
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
                    bool isCountDown = false;
                    if (!bool.TryParse(cmd.GetParameterValue(TextCommand.ParameterName.IsCountDown), out isCountDown))
                        isCountDown = false;
                    execResult = PlayMedia(path, isCountDown);
                    break;
                case TextCommand.CommandName.MediaStop:
                    execResult = PlayMedia(string.Empty, false);
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

            if (item != null && !string.IsNullOrEmpty(item.PlayInterval))
            {
                int interval;
                if (int.TryParse(item.PlayInterval, out interval))
                    playInterval = interval;
            }
            return true;
        }

        private bool PlayMedia(string sourcePath, bool isCountDown)
        {
            string path = FindFileByCategory(sourcePath, FileCategory.Image);
            if (currStatus == PlayerStatusType.Playing && string.Compare(path, SourcePath, true) == 0)
                return true;

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
                if (category != FileCategory.Image)
                {
                    this.Source = null;
                    this.Visibility = Visibility.Hidden;
                    currStatus = PlayerStatusType.Idle;
                    return false;
                }

                this.Source = BitmapFrame.Create(new Uri(SourcePath));
                this.Visibility = Visibility.Visible;
                currStatus = PlayerStatusType.Playing;

                if (isCountDown && playInterval != null)
                {
                    latestPlayDateTime = DateTime.Now;
                    checkIntervalTimer = ActionInATimeInterval(1000, new EventHandler(CheckCountDown));
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
                        files = Directory.GetFiles(basePath, filename, SearchOption.TopDirectoryOnly);
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

        private void CheckCountDown(Object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - latestPlayDateTime;
            int seconds = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
            if (seconds >= playInterval)
            {
                checkIntervalTimer.Stop();
                checkIntervalTimer = null;

                if (MediaActedHandler != null)
                    MediaActedHandler(new MediaActedNotifier()
                    {
                        Action = PlayerActionType.Ended,
                        Id = this.Id,
                        SourcePath = this.SourcePath,
                        Param = null
                    });
                PlayMedia(string.Empty, false);
            }
        }

        private DispatcherTimer ActionInATimeInterval(double delayTime, EventHandler handler)
        {
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(delayTime),
                IsEnabled = false
            };
            timer.Tick += new EventHandler(handler);
            timer.Start();

            return timer;
        }
        #endregion
    }
}
