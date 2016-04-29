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
            Logger.LoggerInstance.Log(TraceEventType.Start, "Universal media player starting...");

            LoadConfigurations();
            LoadPlayers();

            Logger.LoggerInstance.Log(TraceEventType.Start, "Universal media player started.");




            TextCommand cmd = new TextCommand()
            {
                Name = TextCommand.CommandName.MediaPlay
            };
            cmd.SetParameterValue(TextCommand.ParameterName.Path, @"C:\Users\tcell\Pictures\Photo\Kid\20th month\IMG_20160326_111818.jpg");
            foreach (IPlayable player in players)
                player.ExecuteCommand(cmd.ToString());
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
                        Logger.LoggerInstance.Log(TraceEventType.Stop, $"Stop {player.Id} return false!");
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

                                Logger.LoggerInstance.Log(TraceEventType.Start, $"Load {player.Id} successfully.");
                            }
                            else
                            {
                                Logger.LoggerInstance.Log(TraceEventType.Start, $"Start {player.Id} return false!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LoggerInstance.Log($"Load {path} player failed, {ex.Message}", ex);
                }
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
