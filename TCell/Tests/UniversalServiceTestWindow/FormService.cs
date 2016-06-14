using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.Generic;

using TCell.Text;
using TCell.Logging;
using TCell.Abstraction;

namespace UniversalServiceTestWindow
{
    public partial class FormService : Form
    {
        #region constructors
        public FormService()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private List<IReceivable> receivers = null;
        private List<IServiceActor> actors = null;

        private string DeviceId
        {
            get { return ConfigurationManager.AppSettings["deviceId"]; }
        }
        #endregion

        #region events
        private void Form1_Load(object sender, EventArgs e)
        {
            LogMessage(TraceEventType.Start, "Universal windows service starting...");
            PlayerHelper.SetLogHandler(LogMessage);
            PlayerHelper.SetLogHandler(LogException);

            LoadConfigurations();
            LoadCommandReceivers();
            LoadActors();

            LogMessage(TraceEventType.Start, "Universal windows service started.");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
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
            // stop all actors
            if (actors != null && actors.Count > 0)
            {
                foreach (IServiceActor actor in actors)
                {
                    if (actor == null)
                        continue;

                    actor.StopActor();
                }
            }

            LogMessage(TraceEventType.Stop, "Universal windows service stopped.");
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
            }

            foreach (IServiceActor actor in actors)
            {
                actor.ExecuteCommand(commandText);
            }
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

        private void LoadActors()
        {
            string[] dllPaths = Directory.GetFiles(Environments.ApplicationPath, "*.dll");
            if (dllPaths == null || dllPaths.Length == 0)
                return;

            Type actorType = typeof(IServiceActor);
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

                        if (type.GetInterface(actorType.FullName) != null)
                        {
                            IServiceActor actor = (IServiceActor)Activator.CreateInstance(type);

                            if (actors == null)
                                actors = new List<IServiceActor>();

                            if (actor.StartActor())
                                actors.Add(actor);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException($"Load {path} service command actor failed, {ex.Message}", ex);
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
        #endregion
    }
}
