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

using TCell.Text;
using TCell.Logging;
using TCell.Abstraction;

namespace TCell.UniversalMediaPlayer
{
    public partial class MainWindow : Window, IPlayerHostable
    {
        private delegate bool ExecuteCommandDelegate(string commandText);

        #region constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private List<IReceivable> receivers = null;
        private List<IPlayable> players = null;

        private string DeviceId
        {
            get { return ConfigurationManager.AppSettings["deviceId"]; }
        }
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogMessage(TraceEventType.Start, "Universal media player starting...");
            PlayerHelper.SetLogHandler(LogMessage);
            PlayerHelper.SetLogHandler(LogException);

            LoadConfigurations();
            LoadCommandReceivers();
            LoadPlayers();

            LogMessage(TraceEventType.Start, "Universal media player started.");




            TextCommand cmd = new TextCommand()
            {
                Name = TextCommand.CommandName.MediaPlay
            };
            //cmd.SetParameterValue(TextCommand.ParameterName.Path, @"C:\Users\tcell\Pictures\Photo\Kid\20th month\IMG_20160326_111818.jpg");
            cmd.SetParameterValue(TextCommand.ParameterName.Path, @"C:\Users\tcell\Desktop\Screen1\123.mp4");
            TextCommand cmd1 = new TextCommand()
            {
                Name = TextCommand.CommandName.MediaMute
            };

            foreach (IPlayable player in players)
            {
                player.ExecuteCommand(cmd.ToString());
                player.ExecuteCommand(cmd1.ToString());
            }
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

                    receiver.StopRrceiver();
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
                string targetDevIds = cmd.GetParameterValue(TextCommand.ParameterName.DeviceIds);
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

                        }
                        break;
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
        }

        private void OnMediaActed(MediaActedNotifier notifier)
        {

        }
        #endregion

        #region private functions
        private void LoadConfigurations()
        {
            string str = ConfigurationManager.AppSettings["title"];
            if (!string.IsNullOrEmpty(str))
                Title = str;

            str = ConfigurationManager.AppSettings["backgroundImageUri"];
            if (!string.IsNullOrEmpty(str) && File.Exists(str))
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
        }

        private void LoadCommandReceivers()
        {
            string[] dllPaths = Directory.GetFiles(Environments.ApplicationPath, "*.dll");
            if (dllPaths == null || dllPaths.Length == 0)
                return;

            Type receivererType = typeof(IReceivable);
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

                            if (receivers == null)
                                receivers = new List<IReceivable>();

                            receiver.CommandReceivedHandler += this.OnCommandReceived;
                            if (receiver.StartReceiver())
                                receivers.Add(receiver);
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

                            if (players == null)
                                players = new List<IPlayable>();

                            if (player.StartPlayer())
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

        private void LogException(string msg, Exception ex)
        {
            Logger.LoggerInstance.Log(msg, ex);
        }

        private void LogMessage(TraceEventType evt, string msg)
        {
            Logger.LoggerInstance.Log(evt, msg);
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
