using System;
using System.Diagnostics;
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using EntLib = Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace TCell.Logging
{
    public sealed class Logger
    {
        #region Constructors
        private Logger()
        {
            IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
            EntLib.LogWriterFactory logWriterFactory = new EntLib.LogWriterFactory(configurationSource);
            EntLib.Logger.SetLogWriter(logWriterFactory.Create());
        }
        #endregion

        #region Properties
        public static readonly Logger LoggerInstance = new Logger();

        public static string Filename
        {
            get
            {
                string filename = string.Empty;
                {
                    LoggingSettings section = (LoggingSettings)ConfigurationManager.GetSection(LoggingSettings.SectionName);
                    if (section != null && section.TraceListeners.Count > 0)
                    {
                        RollingFlatFileTraceListenerData listener = section.TraceListeners.Get(0) as RollingFlatFileTraceListenerData;
                        if (listener != null)
                            filename = listener.FileName;
                    }
                }
                return filename;
            }
        }
        #endregion

        #region Public methods
        public void Log(TraceEventType severity, string msg)
        {
            EntLib.LogEntry entry = new EntLib.LogEntry()
            {
                Severity = severity,
                Message = msg,
                TimeStamp = DateTime.Now
            };
            EntLib.Logger.Write(entry);
        }

        public void Log(string msg, Exception ex)
        {
            string errMsg = msg;
            if (ex != null)
                errMsg = $"{msg}{Environment.NewLine}{ex.Source}{Environment.NewLine}{ex.StackTrace}";
            EntLib.LogEntry entry = new EntLib.LogEntry()
            {
                Severity = TraceEventType.Error,
                Message = errMsg,
                TimeStamp = DateTime.Now
            };
            EntLib.Logger.Write(entry);
        }
        #endregion
    }
}
