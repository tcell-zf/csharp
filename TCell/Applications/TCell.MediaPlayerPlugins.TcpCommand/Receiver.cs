using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCell.Abstraction;

namespace TCell.MediaPlayerPlugins.TcpCommand
{
    public class Receiver : IReceivable
    {
        #region properties
        public string Id
        {
            get { return "TcpCommandReceiver"; }
        }

        public Action<string, string> CommandReceivedHandler { get; set; }
        #endregion

        #region public functions
        public bool StartReceiver()
        {
            return true;
        }

        public bool StopRrceiver()
        {
            return true;
        }

        public bool Send(string response)
        {
            return true;
        }
        #endregion
    }
}
