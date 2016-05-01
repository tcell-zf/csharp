using System;
using System.Diagnostics;

namespace TCell.Abstraction
{
    public enum PlayerStatusType
    {
        Idle, Playing, Paused
    }

    public interface IPlayable
    {
        string Id { get; }
        string SourcePath { get; set; }
        PlayerStatusType Status { get; }

        Action<string, object> MediaActedHandler { get; set; }

        bool StartPlayer();
        bool StopPlayer();
        bool ExecuteCommand(string commandText);
    }

    public interface IPlayerHostable
    {
    }

    static public class PlayerHelper
    {
        static private Action<string, Exception> exceptionLogHandler = null;
        static private Action<TraceEventType, string> eventLogHandler = null;

        static public void SetLogHandler(Action<string, Exception> handler)
        {
            if (handler == null)
                exceptionLogHandler = null;
            else
                exceptionLogHandler += handler;
        }

        static public void SetLogHandler(Action<TraceEventType, string> handler)
        {
            if (handler == null)
                eventLogHandler = null;
            else
                eventLogHandler += handler;
        }

        static public void LogMessage(TraceEventType evt, string msg)
        {
            if (eventLogHandler != null)
            {
                eventLogHandler(evt, msg);
            }
        }

        static public void LogException(string msg, Exception ex)
        {
            if (exceptionLogHandler != null)
            {
                exceptionLogHandler(msg, ex);
            }
        }
    }
}
