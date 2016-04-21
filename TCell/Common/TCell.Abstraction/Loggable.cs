using System;

namespace TCell.Abstraction
{
    abstract public class Loggable
    {
        static private Action<string, Exception> logExceptionHandler = null;

        static public void SetLogHandler(Action<string, Exception> handler)
        {
            if (handler == null)
                logExceptionHandler = null;
            else
                logExceptionHandler += handler;
        }

        static protected void LogException(string msg, Exception ex)
        {
            if (logExceptionHandler != null)
            {
                logExceptionHandler(msg, ex);
            }
        }
    }
}
