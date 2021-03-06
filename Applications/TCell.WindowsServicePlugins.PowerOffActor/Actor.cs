﻿using System;
using System.Diagnostics;

using TCell.Text;
using TCell.Abstraction;

namespace TCell.WindowsServicePlugins.PowerOffActor
{
    public class Actor : IServiceActor, IStringCommandServiceActor
    {
        #region properties
        public string Id
        {
            get { return "PowerOffActor"; }
        }
        #endregion

        #region public functions
        public bool StartActor()
        {
            PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            return true;
        }

        public bool StopActor()
        {
            PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            return true;
        }

        public bool ExecuteStringCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return false;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd == null)
                return false;

            bool execResult = false;
            if (cmd.Name == TextCommand.CommandName.Shutdown)
            {
                try
                {
                    ProcessExecutor.StartProcess("shutdown", "/s /t 0");
                }
                catch (Exception ex)
                {
                    PlayerHelper.LogException($"Exception occurred when {Id} shutdown self, {ex.Message}", ex);
                    execResult = false;
                }
            }

            return execResult;
        }
        #endregion
    }
}
