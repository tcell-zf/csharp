using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

using TCell.IO;
using TCell.Text;
using TCell.Logging;
using TCell.Abstraction;

namespace TCell.UniversalMediaPlayer
{
    public partial class MainWindow : Window, IPlayerHostable
    {
        private delegate bool ExecuteCommandDelegate(string commandText);
        private delegate void ShowWindowDelegate(bool isShow);

        #region constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private List<IReceivable> receivers = null;
        private List<IPlayable> players = null;

        private LoopMode loopMode = null;
        private int? autoStartLoopInterval = null;
        private DateTime lastActiveDateTime = DateTime.Now;
        private DispatcherTimer checkLoopTimer = null;

        private string DeviceId
        {
            get { return ConfigurationManager.AppSettings["deviceId"]; }
        }
        private string MediaPath
        {
            get { return ConfigurationManager.AppSettings["mediaPath"]; }
        }
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogMessage(TraceEventType.Start, "Universal media player starting...");

            Mouse.OverrideCursor = System.Windows.Input.Cursors.None;
            PlayerHelper.SetLogHandler(LogMessage);
            PlayerHelper.SetLogHandler(LogException);

            LoadConfigurations();
            LoadCommandReceivers();
            LoadPlayers();

            if (autoStartLoopInterval != null && players != null && players.Count > 0)
            {
                if (!string.IsNullOrEmpty(MediaPath) && Directory.Exists(MediaPath))
                {
                    checkLoopTimer = ActionInATimeInterval(60 * 1000, new EventHandler(CheckAutoLoop));
                }
            }

            LogMessage(TraceEventType.Start, "Universal media player started.");
        }

        protected override void OnClosed(EventArgs e)
        {
            LogMessage(TraceEventType.Stop, "Universal media player stopping...");

            // stop all command receivers
            if (receivers != null && receivers.Count > 0)
            {
                foreach (IReceivable receiver in receivers)
                {
                    if (receiver == null)
                        continue;

                    receiver.StopReceiver();
                }
            }
            // stop all players
            if (players != null && players.Count > 0)
            {
                foreach (IPlayable player in players)
                {
                    if (player == null)
                        continue;

                    player.StopPlayer();
                }
            }
            // stop auto loop timer
            if (checkLoopTimer != null)
            {
                checkLoopTimer.Stop();
                checkLoopTimer = null;
            }

            base.OnClosed(e);

            LogMessage(TraceEventType.Stop, "Universal media player stopped.");
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void OnCommandReceived(string id, string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd != null)
            {
                string targetDevIds = cmd.GetParameterValue(TextCommand.ParameterName.MultiDeviceId);
                if (!IsItMe(targetDevIds))
                    return;

                switch (cmd.Name)
                {
                    case TextCommand.CommandName.MediaQuery:
                        {
                            string sourcePath;
                            PlayerStatusType status = CheckPlayerStatus(out sourcePath);
                            TextCommand resp = new TextCommand()
                            {
                                Name = TextCommand.CommandName.MediaReplyQuery
                            };
                            if (!string.IsNullOrEmpty(DeviceId))
                                resp.SetParameterValue(TextCommand.ParameterName.DeviceId, DeviceId);
                            resp.SetParameterValue(TextCommand.ParameterName.Status, status.ToString());
                            if (!string.IsNullOrEmpty(sourcePath))
                                resp.SetParameterValue(TextCommand.ParameterName.Path, sourcePath);

                            SendResponse(resp);
                        }
                        return;
                    case TextCommand.CommandName.MediaLoop:
                        {
                            string mediaPath = cmd.GetParameterValue(TextCommand.ParameterName.Path);
                            if (string.IsNullOrEmpty(mediaPath))
                                mediaPath = ConfigurationManager.AppSettings["mediaPath"];
                            if (!string.IsNullOrEmpty(mediaPath))
                            {
                                if (Directory.Exists(mediaPath))
                                {
                                    bool isMuted = true;
                                    if (!bool.TryParse(cmd.GetParameterValue(TextCommand.ParameterName.IsMute), out isMuted))
                                        isMuted = true;

                                    Loop(mediaPath, isMuted);
                                }
                            }
                        }
                        return;
                    default:
                        break;
                }
            }

            foreach (IPlayable player in players)
            {
                if (player is UIElement)
                {
                    UIElement uiElement = player as UIElement;
                    uiElement.Dispatcher.BeginInvoke(new ExecuteCommandDelegate(player.ExecuteCommand), new object[] { commandText });
                }
                else
                {
                    player.ExecuteCommand(commandText);
                }
            }
            if (loopMode != null)
                loopMode = null;
        }

        private void OnMediaActed(MediaActedNotifier notifier)
        {
            if (notifier == null)
                return;

            if (loopMode != null)
            {
                switch (notifier.Action)
                {
                    case PlayerActionType.Opend:
                        {
                            if (loopMode.IsMuted)
                            {
                                foreach (IPlayable player in players)
                                {
                                    player.ExecuteCommand(new TextCommand()
                                    {
                                        Name = TextCommand.CommandName.MediaMute
                                    }.ToString());
                                }
                            }
                        }
                        break;
                    case PlayerActionType.Ended:
                        {
                            PlayMedia(loopMode.GetNextMedia());
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region public functions
        public void ShowMe(bool isShow)
        {
            this.Dispatcher.BeginInvoke(new ShowWindowDelegate(ShowWindow), new object[] { isShow });
        }
        #endregion

        #region private functions
        private void LogException(string msg, Exception ex)
        {
            Logger.LoggerInstance.Log(msg, ex);
        }

        private void LogMessage(TraceEventType evt, string msg)
        {
            Logger.LoggerInstance.Log(evt, msg);
        }

        private void LoadConfigurations()
        {
            string str = ConfigurationManager.AppSettings["title"];
            if (!string.IsNullOrEmpty(str))
                Title = str;

            str = FindFileByCategory("background", FileCategory.Image, "App");
            if (!string.IsNullOrEmpty(str) && System.IO.File.Exists(str))
                backgroundImageBrush.ImageSource = BitmapFrame.Create(new Uri(str));

            str = ConfigurationManager.AppSettings["displayDeviceName"];
            if (!string.IsNullOrEmpty(str))
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.DeviceName.ToLower().Contains(str.ToLower()))
                    {
                        WindowState = WindowState.Normal;
                        Top = screen.WorkingArea.Top;
                        Left = screen.WorkingArea.Left;
                        WindowState = WindowState.Maximized;

                        break;
                    }
                }
            }

            int interval;
            if (int.TryParse(ConfigurationManager.AppSettings["autoStartLoopInterval"], out interval))
                autoStartLoopInterval = interval;
            else
                autoStartLoopInterval = null;
        }

        private string FindFileByCategory(string filename, FileCategory category, string subfolder = "")
        {
            string basePath = string.Empty;
            if (string.IsNullOrEmpty(MediaPath) || !Directory.Exists(MediaPath))
                basePath = string.IsNullOrEmpty(subfolder) ? Path.Combine(Environments.ApplicationPath, "MediaFiles")
                    : Path.Combine(Environments.ApplicationPath, "MediaFiles", subfolder);
            else
                basePath = string.IsNullOrEmpty(subfolder) ? MediaPath
                    : Path.Combine(MediaPath, subfolder);

            if (!Directory.Exists(basePath))
                return string.Empty;

            string[] files = null;
            try
            {
                files = Directory.GetFiles(basePath, $"{filename}.*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                Logger.LoggerInstance.Log($"Exception occurred when searching {filename}.* of {category} with subfolder={subfolder}, {ex.Message}", ex);
                return string.Empty;
            }

            if (files == null || files.Length == 0)
                return string.Empty;

            string path = string.Empty;
            foreach (string f in files)
            {
                FileCategory cat = TCell.IO.File.GetFileCategory(f);
                if (category != cat)
                    continue;

                path = f;
                break;
            }

            return path;
        }

        private void LoadCommandReceivers()
        {
            string[] dllPaths = Directory.GetFiles(Environments.ApplicationPath, "*.dll");
            if (dllPaths == null || dllPaths.Length == 0)
                return;

            Type receivererType = typeof(IReceivable);
            Type stringCmdRecType = typeof(IStringCommandReceivable);
            foreach (string path in dllPaths)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path);
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                            continue;

                        if (type.GetInterface(receivererType.FullName) != null)
                        {
                            IReceivable receiver = (IReceivable)Activator.CreateInstance(type);

                            if (type.GetInterface(stringCmdRecType.FullName) != null)
                            {
                                IStringCommandReceivable stringRec = (IStringCommandReceivable)receiver;
                                stringRec.StringCommandReceivedHandler += this.OnCommandReceived;

                                if (receiver.StartReceiver())
                                {
                                    if (receivers == null)
                                        receivers = new List<IReceivable>();

                                    receivers.Add(receiver);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException($"Load {path} command receiver failed, {ex.Message}", ex);
                }
            }
        }

        private void LoadPlayers()
        {
            string[] dllPaths = Directory.GetFiles(Environments.ApplicationPath, "*.dll");
            if (dllPaths == null || dllPaths.Length == 0)
                return;

            Type playerType = typeof(IPlayable);
            foreach (string path in dllPaths)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path);
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                            continue;

                        if (type.GetInterface(playerType.FullName) != null)
                        {
                            IPlayable player = (IPlayable)Activator.CreateInstance(type);
                            if (!string.IsNullOrEmpty(MediaPath) && Directory.Exists(MediaPath))
                                player.BasePath = MediaPath;

                            if (players == null)
                                players = new List<IPlayable>();

                            if (player.StartPlayer(this))
                            {
                                player.MediaActedHandler += OnMediaActed;
                                players.Add(player);

                                if (player is UIElement)
                                {
                                    UIElement uiElement = player as UIElement;
                                    if (uiElement != null)
                                        container.Children.Add(uiElement);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException($"Load {path} player failed, {ex.Message}", ex);
                }
            }
        }

        private bool IsItMe(string deviceIds)
        {
            if (string.IsNullOrEmpty(deviceIds) || string.IsNullOrEmpty(DeviceId))
                return true;

            string[] devIdArr = deviceIds.Split(new char[] { ',' });
            if (devIdArr == null || devIdArr.Length == 0)
                return true;

            bool isItMe = false;
            foreach (string id in devIdArr)
            {
                if (id == DeviceId)
                {
                    isItMe = true;
                    break;
                }
            }
            return isItMe;
        }

        private PlayerStatusType CheckPlayerStatus(out string sourcePath)
        {
            sourcePath = string.Empty;

            if (players == null || players.Count == 0)
                return PlayerStatusType.Idle;

            PlayerStatusType status = PlayerStatusType.Idle;
            foreach (IPlayable player in players)
            {
                if (player == null)
                    continue;

                if (player.Status != PlayerStatusType.Idle)
                {
                    status = player.Status;
                    sourcePath = player.SourcePath;
                    break;
                }
            }

            return status;
        }

        private void SendResponse(TextCommand cmd)
        {
            if (receivers == null || receivers.Count == 0)
                return;

            foreach (IReceivable receiver in receivers)
            {
                receiver.Send(cmd.ToString());
            }
        }

        private void Loop(string mediaPath, bool isMute)
        {
            if (string.IsNullOrEmpty(mediaPath))
                return;
            if (!Directory.Exists(mediaPath))
                return;

            loopMode = new LoopMode()
            {
                IsMuted = isMute
            };
            string[] filePaths = Directory.GetFiles(mediaPath);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (string path in filePaths)
            {
                FileCategory category = TCell.IO.File.GetFileCategory(path);
                if (category == FileCategory.Video
                    || category == FileCategory.Audio
                    || category == FileCategory.Image)
                    loopMode.AddPlayableMedia(path);
            }

            PlayMedia(loopMode.GetNextMedia());
        }

        private void PlayMedia(string path)
        {
            TextCommand cmdPlay = new TextCommand()
            {
                Name = TextCommand.CommandName.MediaPlay
            };
            cmdPlay.SetParameterValue(TextCommand.ParameterName.Path, path);
            if (loopMode != null)
                cmdPlay.SetParameterValue(TextCommand.ParameterName.IsCountDown, true.ToString());

            foreach (IPlayable player in players)
            {
                if (player is UIElement)
                {
                    UIElement uiElement = player as UIElement;
                    uiElement.Dispatcher.BeginInvoke(new ExecuteCommandDelegate(player.ExecuteCommand), new object[] { cmdPlay.ToString() });
                }
                else
                {
                    player.ExecuteCommand(cmdPlay.ToString());
                }
            }
        }

        private void CheckAutoLoop(Object sender, EventArgs e)
        {
            if (loopMode != null)
                return;

            TimeSpan ts = DateTime.Now - lastActiveDateTime;
            int minutes = ts.Hours * 60 + ts.Minutes;
            if (minutes >= autoStartLoopInterval)
            {
                bool isMute = true;
                if (!bool.TryParse(ConfigurationManager.AppSettings["autoStartLoopOnMute"], out isMute))
                    isMute = true;
                Loop(MediaPath, isMute);
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

        private void ShowWindow(bool isShow)
        {
            if (isShow)
            {
                //WindowState = WindowState.Maximized;
                Show();
                Activate();
            }
            else
            {
                //WindowState = WindowState.Minimized;
                Hide();
            }
        }
        #endregion
    }

    internal class LoopMode
    {
        private Dictionary<int, string> playableMedia = null;
        private int currMediaIndex = 0;

        public bool IsMuted { get; set; }

        public void AddPlayableMedia(string path)
        {
            if (!System.IO.File.Exists(path))
                return;

            if (playableMedia == null)
                playableMedia = new Dictionary<int, string>();

            playableMedia.Add(playableMedia.Count, path);
        }

        public string GetNextMedia()
        {
            if (playableMedia == null || playableMedia.Count == 0)
                return string.Empty;

            string path = playableMedia[currMediaIndex];
            currMediaIndex++;
            if (currMediaIndex >= playableMedia.Count)
                currMediaIndex = 0;

            return path;
        }
    }
}
