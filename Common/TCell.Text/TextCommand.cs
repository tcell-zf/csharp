using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TCell.Text
{
    // Command string example: cmd?param1=value1&param2=value2
    public class TextCommand
    {
        public string Name { get; set; }
        private Dictionary<string, string> Parameters { get; set; }

        public override string ToString()
        {
            if (Parameters == null || Parameters.Count == 0)
                return Name;

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in Parameters)
            {
                if (sb.Length > 0)
                    sb.Append("&");

                sb.Append(string.Format("{0}={1}", kv.Key, kv.Value));
            }

            return string.Format("{0}?{1}", Name, sb.ToString());
        }

        public static TextCommand Parse(string cmdString)
        {
            if (string.IsNullOrEmpty(cmdString))
                return null;

            string pattern = @"[\s]*[?&]";
            Regex regex = new Regex(pattern);
            string[] cmdArr = regex.Split(cmdString);
            if (cmdArr == null || cmdArr.Length == 0)
            {
                return new TextCommand()
                {
                    Name = cmdString
                };
            }

            string cmdName = string.Empty;
            Dictionary<string, string> param = null;
            for (int i = 0; i < cmdArr.Length; i++)
            {
                if (string.IsNullOrEmpty(cmdArr[i]))
                    continue;

                string[] paramArr = cmdArr[i].Split(new char[] { '=' });
                if (paramArr.Length == 1)
                {
                    if (string.IsNullOrEmpty(cmdName))
                        cmdName = paramArr[i];
                }
                else if (paramArr.Length == 2)
                {
                    if (param == null)
                        param = new Dictionary<string, string>();

                    if (!param.ContainsKey(paramArr[0]))
                        param.Add(paramArr[0], paramArr[1]);
                }
                else { }
            }

            return new TextCommand()
            {
                Name = cmdName,
                Parameters = param
            };
        }

        public static TextCommand Parse(byte[] cmdBytes)
        {
            return TextCommand.Parse(TextCommand.GetString(cmdBytes));
        }

        public static byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public string GetParameterValue(string paramName)
        {
            if (Parameters == null || Parameters.Count == 0)
                return string.Empty;
            if (!Parameters.ContainsKey(paramName))
                return string.Empty;

            return Parameters[paramName];
        }

        public Dictionary<string, string> GetParameterValues()
        {
            return Parameters;
        }

        public bool SetParameterValue(string paramName, string paramValue)
        {
            if (Parameters == null)
                Parameters = new Dictionary<string, string>();

            if (Parameters.ContainsKey(paramName))
                Parameters[paramName] = paramValue;
            else
                Parameters.Add(paramName, paramValue);

            return true;
        }

        public class CommandName
        {
            public const string Shutdown = "shutdown";

            public const string MediaPlay = "play";
            public const string MediaStop = "stop";
            public const string MediaPause = "pause";
            public const string MediaMute = "mute";
            public const string MediaUnmute = "unmute";

            public const string MediaPPTControl = "ppt-control";
            public const string MediaQuery = "query";
            public const string MediaStandby = "standby";
            public const string MediaLoop = "loop";

            public const string MediaReplyQuery = "reply-query";
        }

        public class ParameterName
        {
            public const string DeviceIds = "deviceids";
            public const string DeviceId = "deviceid";

            public const string Status = "status";
            public const string Path = "path";
            public const string IsMute = "ismute";
            public const string IsCountDown = "iscountdown";

            public const string Action = "action";
            public const string MultiDeviceId = "deviceids";
            public const string MediaType = "mediatype";

            public const string PPTFisrt = "first";
            public const string PPTPrevious = "previous";
            public const string PPTNext = "next";
            public const string PPTLast = "last";
            public const string PPTClose = "close";
        }
    }
}
