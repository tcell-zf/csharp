using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.ServiceProcess;
using System.Collections.Generic;

using TCell.Text;
using TCell.Logging;
using TCell.Abstraction;

namespace TCell.UniversalWindowsService
{
    partial class UniversalService : ServiceBase
    {
        #region constructors
        public UniversalService()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private List<IReceivable> receivers = null;
        #endregion

        #region events
        protected override void OnStart(string[] args)
        {
            LogMessage(TraceEventType.Start, "Universal windows service starting...");

            LoadConfigurations();
            LoadCommandReceivers();
            //LoadPlayers();

            //if (autoStartLoopInterval != null && players != null && players.Count > 0)
            //{
            //    if (!string.IsNullOrEmpty(MediaPath) && Directory.Exists(MediaPath))
            //    {
            //        checkLoopTimer = ActionInATimeInterval(60 * 1000, new EventHandler(CheckAutoLoop));
            //    }
            //}

            LogMessage(TraceEventType.Start, "Universal windows service started.");
        }

        protected override void OnStop()
        {
            LogMessage(TraceEventType.Stop, "Universal windows service stopping...");

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
            //// stop all players
            //if (players != null && players.Count > 0)
            //{
            //    foreach (IPlayable player in players)
            //    {
            //        if (player == null)
            //            continue;

            //        player.StopPlayer();
            //    }
            //}
            //// stop auto loop timer
            //if (checkLoopTimer != null)
            //{
            //    checkLoopTimer.Stop();
            //    checkLoopTimer = null;
            //}

            LogMessage(TraceEventType.Stop, "Universal windows service stopped.");
        }

        private void OnCommandReceived(string id, string commandText)
        {
            //if (string.IsNullOrEmpty(commandText))
            //    return;

            //TextCommand cmd = TextCommand.Parse(commandText);
            //if (cmd != null)
            //{
            //    string targetDevIds = cmd.GetParameterValue(TextCommand.ParameterName.DeviceIds);
            //    if (!IsItMe(targetDevIds))
            //        return;

            //    switch (cmd.Name)
            //    {
            //        case TextCommand.CommandName.MediaQuery:
            //            {
            //                string sourcePath;
            //                PlayerStatusType status = CheckPlayerStatus(out sourcePath);
            //                TextCommand resp = new TextCommand()
            //                {
            //                    Name = TextCommand.CommandName.MediaReplyQuery
            //                };
            //                if (!string.IsNullOrEmpty(DeviceId))
            //                    resp.SetParameterValue(TextCommand.ParameterName.DeviceId, DeviceId);
            //                resp.SetParameterValue(TextCommand.ParameterName.Status, status.ToString());
            //                if (!string.IsNullOrEmpty(sourcePath))
            //                    resp.SetParameterValue(TextCommand.ParameterName.Path, sourcePath);

            //                SendResponse(resp);
            //            }
            //            return;
            //        case TextCommand.CommandName.MediaLoop:
            //            {
            //                string mediaPath = cmd.GetParameterValue(TextCommand.ParameterName.Path);
            //                if (string.IsNullOrEmpty(mediaPath))
            //                    mediaPath = ConfigurationManager.AppSettings["mediaPath"];
            //                if (!string.IsNullOrEmpty(mediaPath))
            //                {
            //                    if (Directory.Exists(mediaPath))
            //                    {
            //                        bool isMuted = true;
            //                        if (!bool.TryParse(cmd.GetParameterValue(TextCommand.ParameterName.IsMute), out isMuted))
            //                            isMuted = true;

            //                        Loop(mediaPath, isMuted);
            //                    }
            //                }
            //            }
            //            return;
            //        default:
            //            break;
            //    }
            //}

            //foreach (IPlayable player in players)
            //{
            //    if (player is UIElement)
            //    {
            //        UIElement uiElement = player as UIElement;
            //        uiElement.Dispatcher.BeginInvoke(new ExecuteCommandDelegate(player.ExecuteCommand), new object[] { commandText });
            //    }
            //    else
            //    {
            //        player.ExecuteCommand(commandText);
            //    }
            //}
            //if (loopMode != null)
            //    loopMode = null;
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
            //string str = ConfigurationManager.AppSettings["title"];
            //if (!string.IsNullOrEmpty(str))
            //    Title = str;

            //str = ConfigurationManager.AppSettings["backgroundImageUri"];
            //if (!string.IsNullOrEmpty(str) && System.IO.File.Exists(str))
            //    backgroundImageBrush.ImageSource = BitmapFrame.Create(new Uri(str));

            //str = ConfigurationManager.AppSettings["displayDeviceName"];
            //if (!string.IsNullOrEmpty(str))
            //{
            //    foreach (Screen screen in Screen.AllScreens)
            //    {
            //        if (screen.DeviceName.ToLower().Contains(str.ToLower()))
            //        {
            //            WindowState = WindowState.Normal;
            //            Top = screen.WorkingArea.Top;
            //            Left = screen.WorkingArea.Left;
            //            WindowState = WindowState.Maximized;

            //            break;
            //        }
            //    }
            //}

            //int interval;
            //if (int.TryParse(ConfigurationManager.AppSettings["autoStartLoopInterval"], out interval))
            //    autoStartLoopInterval = interval;
            //else
            //    autoStartLoopInterval = null;
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
        #endregion
    }
}
