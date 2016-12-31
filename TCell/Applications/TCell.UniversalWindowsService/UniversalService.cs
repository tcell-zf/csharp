using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
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
        private List<IServiceActor> actors = null;
        private List<IStringCommandServiceActor> stringCmdActors = null;
        private List<IBytesCommandServiceActor> bytesCmdActors = null;

        private string DeviceId
        {
            get { return ConfigurationManager.AppSettings["deviceId"]; }
        }
        #endregion

        #region events
        protected override void OnStart(string[] args)
        {
            LogMessage(TraceEventType.Start, "Universal windows service starting...");
            PlayerHelper.SetLogHandler(LogMessage);
            PlayerHelper.SetLogHandler(LogException);

            LoadConfigurations();
            LoadActors();
            LoadCommandReceivers();

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

                    receiver.StopReceiver();
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

        private void OnStringCommandReceived(string id, string commandText)
        {
            if (string.IsNullOrEmpty(commandText) || stringCmdActors == null || stringCmdActors.Count == 0)
                return;

            // Fuck, useless
            //TextCommand cmd = TextCommand.Parse(commandText);
            //if (cmd != null)
            //{
            //    string targetDevIds = cmd.GetParameterValue(TextCommand.ParameterName.MultiDeviceId);
            //    if (!IsItMe(targetDevIds))
            //        return;
            //}

            foreach (IStringCommandServiceActor actor in stringCmdActors)
            {
                actor.ExecuteStringCommand(commandText);
            }
        }

        private void OnBytesCommandReceived(string id, Dictionary<int, KeyValuePair<byte, byte?>> commandBytes, string commandText)
        {
            if (commandBytes == null || commandBytes.Count == 0
                || bytesCmdActors == null || bytesCmdActors.Count == 0)
                return;

            foreach (IBytesCommandServiceActor actor in bytesCmdActors)
            {
                actor.ExecuteBytesCommand(commandBytes, commandText);
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
            Type stringCmdRecType = typeof(IStringCommandReceivable);
            Type bytesCmdRecType = typeof(IBytesCommandReceivable);
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

                            bool isSupportedReceiver = false;
                            if (type.GetInterface(stringCmdRecType.FullName) != null)
                            {
                                IStringCommandReceivable stringRec = (IStringCommandReceivable)receiver;
                                stringRec.StringCommandReceivedHandler += this.OnStringCommandReceived;

                                isSupportedReceiver = true;
                            }
                            if (type.GetInterface(bytesCmdRecType.FullName) != null)
                            {
                                IBytesCommandReceivable bytesRec = (IBytesCommandReceivable)receiver;
                                bytesRec.BytesCommandReceivedHandler += this.OnBytesCommandReceived;

                                isSupportedReceiver = true;
                            }

                            if (isSupportedReceiver)
                            {
                                if (receiver.StartReceiver())
                                {
                                    if (receivers == null)
                                        receivers = new List<IReceivable>();

                                    receivers.Add(receiver);
                                }
                            }
                            else
                            {
                                LogMessage(TraceEventType.Start, $"Unsupported receiver type, {type.FullName}.");
                                continue;
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

        private void LoadActors()
        {
            string[] dllPaths = Directory.GetFiles(Environments.ApplicationPath, "*.dll");
            if (dllPaths == null || dllPaths.Length == 0)
                return;

            Type actorType = typeof(IServiceActor);
            Type stringActorType = typeof(IStringCommandServiceActor);
            Type bytesActorType = typeof(IBytesCommandServiceActor);
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

                            if (type.GetInterface(stringActorType.FullName) != null)
                            {
                                IStringCommandServiceActor stringActor = (IStringCommandServiceActor)actor;

                                if (stringCmdActors == null)
                                    stringCmdActors = new List<IStringCommandServiceActor>();
                                stringCmdActors.Add(stringActor);
                            }
                            else if (type.GetInterface(bytesActorType.FullName) != null)
                            {
                                IBytesCommandServiceActor bytesActor = (IBytesCommandServiceActor)actor;

                                if (bytesCmdActors == null)
                                    bytesCmdActors = new List<IBytesCommandServiceActor>();
                                bytesCmdActors.Add(bytesActor);
                            }
                            else
                            {
                                LogMessage(TraceEventType.Start, $"Unsupported actor type, {type.FullName}.");
                                continue;
                            }

                            if (actor.StartActor())
                            {
                                if (actors == null)
                                    actors = new List<IServiceActor>();

                                actors.Add(actor);
                            }
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
