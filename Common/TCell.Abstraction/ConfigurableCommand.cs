using System;
using System.IO;
using System.Collections.Generic;

namespace TCell.Abstraction
{
    public class ConfigurableCommand
    {
        private string baseCommandPath = string.Empty;
        protected Action<byte[]> DataReceivedHandler = null;
        private Stack<string> recentCmdNames = null;

        public ConfigurableCommand(string basePath)
        {
            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException($"{basePath} not exists.");

            baseCommandPath = basePath;
        }

        protected bool Send(string cmdName)
        {
            if (string.IsNullOrEmpty(cmdName))
                throw new ArgumentNullException(nameof(cmdName));

            string path = Path.Combine(baseCommandPath, cmdName);
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} not exists.");

            path = Path.Combine(path, "request.txt");
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} not exists.");

            byte[] cmd = ReadRequestBytes(path);
            if (cmd == null || cmd.Length == 0)
                return false;

            bool result = SendRequestBytes(cmd);
            if (result)
            {
                if (recentCmdNames == null)
                    recentCmdNames = new Stack<string>();

                recentCmdNames.Push(cmdName);
            }
            return result;
        }

        protected bool Process(byte[] response)
        {
            if (recentCmdNames == null || recentCmdNames.Count == 0)
                return false;

            string cmdName = recentCmdNames.Pop();
            if (string.IsNullOrEmpty(cmdName))
                return false;

            string path = Path.Combine(baseCommandPath, cmdName);
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} not exists.");

            path = Path.Combine(path, "response.txt");
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} not exists.");

            byte[] resp = ReadResponseBytes(path, response);
            if (resp == null || resp.Length == 0)
                return false;

            return ProcessResponseBytes(resp);
        }

        virtual protected byte[] ReadRequestBytes(string path)
        {
            throw new NotImplementedException("Read request text method not implemented!");
        }

        virtual protected bool SendRequestBytes(byte[] cmd)
        {
            throw new NotImplementedException("Send request bytes method not implemented!");
        }

        virtual protected byte[] ReadResponseBytes(string path, byte[] rawResp)
        {
            throw new NotImplementedException("Read response text method not implemented!");
        }

        virtual protected bool ProcessResponseBytes(byte[] resp)
        {
            throw new NotImplementedException("Process response bytes method not implemented!");
        }
    }
}
