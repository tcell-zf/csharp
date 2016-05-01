using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
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
        #region constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private List<IPlayable> players = null;
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogMessage(TraceEventType.Start, "Universal media player starting...");

            LoadConfigurations();
            LoadPlayers();
            PlayerHelper.SetLogHandler(LogMessage);
            PlayerHelper.SetLogHandler(LogException);

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
            if (players != null && players.Count > 0)
            {
                foreach (IPlayable player in players)
                {
                    if (player == null)
                        continue;

                    if (!player.StopPlayer())
                        LogMessage(TraceEventType.Stop, $"Stop {player.Id} return false!");
                }
            }

            base.OnClosed(e);
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
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
                                players.Add(player);

                                if (player is UIElement)
                                {
                                    UIElement uiElement = player as UIElement;
                                    if (uiElement != null)
                                        container.Children.Add(uiElement);
                                }

                                LogMessage(TraceEventType.Start, $"Load {player.Id} successfully.");
                            }
                            else
                            {
                                LogMessage(TraceEventType.Start, $"Start {player.Id} return false!");
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
