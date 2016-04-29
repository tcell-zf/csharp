using System;
using System.Diagnostics;

namespace TCell.Abstraction
{
    public interface IPlayable
    {
        string Id { get; }
        string SourcePath { get; set; }

        void SetLogHandler(Action<string, Exception> handler);
        void SetLogHandler(Action<TraceEventType, string> handler);
        bool StartPlayer();
        bool StopPlayer();
        bool ExecuteCommand(string commandText);
    }

    public interface IPlayerHostable
    {
    }
}
