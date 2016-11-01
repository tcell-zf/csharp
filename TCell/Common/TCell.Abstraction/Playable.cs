using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TCell.Abstraction
{
    public interface IReceivable
    {
        string Id { get; }

        bool StartReceiver();
        bool StopReceiver();
        bool Send(string response);
    }

    public interface IStringCommandReceivable
    {
        Action<string, string> StringCommandReceivedHandler { get; set; }
    }

    public interface IBytesCommandReceivable
    {
        Action<string, Dictionary<int, KeyValuePair<byte, byte?>>> BytesCommandReceivedHandler { get; set; }
    }


    public enum PlayerStatusType
    {
        Idle, Playing, Paused
    }

    public enum PlayerActionType
    {
        Opend, Ended, Failed
    }

    public class MediaActedNotifier
    {
        public PlayerActionType Action { get; set; }
        public string Id { get; set; }
        public string SourcePath { get; set; }
        public object Param { get; set; }
    }

    public interface IPlayable
    {
        string Id { get; }
        string BasePath { set; }
        string SourcePath { get; set; }
        PlayerStatusType Status { get; }

        Action<MediaActedNotifier> MediaActedHandler { get; set; }

        bool StartPlayer(object owner);
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




    public interface IServiceActor
    {
        string Id { get; }

        bool StartActor();
        bool StopActor();
        bool ExecuteCommand(string commandText);
    }
}
