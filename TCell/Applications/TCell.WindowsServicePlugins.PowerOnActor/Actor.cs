using System;
using System.Diagnostics;
using System.Collections.Generic;

using TCell.Net;
using TCell.Text;
using TCell.Abstraction;
using TCell.WindowsServicePlugins.PowerOnActor.Configuration;

namespace TCell.WindowsServicePlugins.PowerOnActor
{
    public class Actor : IServiceActor
    {
        #region properties
        private NaryType nary = NaryType.Decimal;
        private Dictionary<string, byte[]> macAddresses = null;

        public string Id
        {
            get { return "PowerOnActor"; }
        }
        #endregion

        #region public functions
        public bool StartActor()
        {
            bool execResult = LoadConfiguration();
            if (execResult)
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} successfully.");
            else
                PlayerHelper.LogMessage(TraceEventType.Start, $"Start {Id} failed!");

            return execResult;
        }

        public bool StopActor()
        {
            PlayerHelper.LogMessage(TraceEventType.Stop, $"Stop {Id} successfully.");
            return true;
        }

        public bool ExecuteCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return false;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd == null)
                return false;

            bool execResult = true;
            if (cmd.Name == TextCommand.CommandName.PowerOn)
            {
                if (macAddresses == null || macAddresses.Count == 0)
                    return false;

                List<string> machineList = null;
                string machines = cmd.GetParameterValue(TextCommand.ParameterName.Machines);
                if (!string.IsNullOrEmpty(machines))
                {
                    string[] strArr = machines.Split(new char[] { ',' });
                    if (strArr != null && strArr.Length > 0)
                        machineList = new List<string>(strArr);
                }

                foreach (KeyValuePair<string, byte[]> kv in macAddresses)
                {
                    if (machineList != null && !machineList.Contains(kv.Key))
                        continue;

                    try
                    {
                        bool ret = WOLCommander.WakeUp(kv.Value);
                        if (!ret)
                            execResult = false;
                    }
                    catch (Exception ex)
                    {
                        PlayerHelper.LogException($"Exception occurred when {Id} wake up {kv.Key}, {ex.Message}", ex);
                    }
                }
            }

            return execResult;
        }
        #endregion

        #region private functions
        private bool LoadConfiguration()
        {
            PowerOnConfigItem item = ConfigurationHelper.GetPowerOnConfiguration();
            if (item != null && !string.IsNullOrEmpty(item.Nary))
            {
                if (!Enum.TryParse<NaryType>(item.Nary, out nary))
                    nary = NaryType.Decimal;
            }

            List<PowerOnMachineConfigItem> machines = ConfigurationHelper.GetPowerOnMachineConfiguration();
            if (machines != null && machines.Count > 0)
            {
                foreach (PowerOnMachineConfigItem machine in machines)
                {
                    if (string.IsNullOrEmpty(machine.Name) || string.IsNullOrEmpty(machine.Mac))
                        continue;

                    byte[] mac = NumericTextParser.ParseNumericStringToArray(nary,
                        machine.Mac, new char[] { '-', ':' });

                    if (mac != null && mac.Length == 6)
                    {
                        if (macAddresses == null)
                            macAddresses = new Dictionary<string, byte[]>();

                        macAddresses.Add(machine.Name, mac);
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
