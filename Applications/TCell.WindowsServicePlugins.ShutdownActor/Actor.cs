using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TCell.Abstraction;

namespace TCell.WindowsServicePlugins.ShutdownActor
{
    public class Actor : IServiceActor
    {
        #region properties
        public string Id
        {
            get { return "ShutdownActor"; }
        }
        #endregion

        #region public functions
        public bool StartActor()
        {
            //currStatus = PlayerStatusType.Idle;

            //bool execResult = LoadConfiguration();
            //if (execResult)
            //    PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            //else
            //    PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} failed!");

            //return execResult;
        }

        public bool StopActor()
        {
            //currStatus = PlayerStatusType.Idle;
            //if (checkIntervalTimer != null)
            //{
            //    checkIntervalTimer.Stop();
            //    checkIntervalTimer = null;
            //}
            //PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            //return true;
        }

        public bool ExecuteCommand(string commandText)
        {
            //if (string.IsNullOrEmpty(commandText))
            //    return false;

            //TextCommand cmd = TextCommand.Parse(commandText);
            //if (cmd == null)
            //    return false;

            //bool execResult = false;
            //switch (cmd.Name)
            //{
            //    case TextCommand.CommandName.MediaPlay:
            //        string path = cmd.GetParameterValue(TextCommand.ParameterName.Path);
            //        bool isCountDown = false;
            //        if (!bool.TryParse(cmd.GetParameterValue(TextCommand.ParameterName.IsCountDown), out isCountDown))
            //            isCountDown = false;
            //        execResult = PlayMedia(path, isCountDown);
            //        break;
            //    case TextCommand.CommandName.MediaStop:
            //        execResult = PlayMedia(string.Empty, false);
            //        break;
            //    default:
            //        this.Source = null;
            //        this.Visibility = Visibility.Hidden;
            //        break;
            //}

            //return execResult;
        }
        #endregion
    }
}
