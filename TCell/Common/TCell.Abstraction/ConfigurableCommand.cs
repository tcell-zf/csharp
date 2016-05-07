using System;
using System.IO;

namespace TCell.Abstraction
{
    public class ConfigurableCommand
    {
        private string baseCommandPath = string.Empty;
        protected Action<byte[]> DataReceivedHandler = null;

        public ConfigurableCommand(string basePath)
        {
            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException($"{basePath} not exists.");

            baseCommandPath = basePath;
        }

        public bool Send(string cmdName)
        {
            if (string.IsNullOrEmpty(cmdName))
                throw new ArgumentNullException(nameof(cmdName));

            string path = Path.Combine(baseCommandPath, cmdName);
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} not exists.");

            path = Path.Combine(path, "request.txt");
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} not exists.");

            byte[] cmd = ReadCommandBytes(path);
            if (cmd == null || cmd.Length == 0)
                return false;

            return SendCommandBytes(cmd);
        }

        virtual protected byte[] ReadCommandBytes(string path)
        {
            throw new NotImplementedException("Read command text method not implemented!");
        }

        virtual protected bool SendCommandBytes(byte[] cmd)
        {
            throw new NotImplementedException("Send command bytes method not implemented!");
        }

        virtual protected bool ProcessCommandBytes(byte[] cmd)
        {
            throw new NotImplementedException("Process command bytes method not implemented!");
        }
    }
}
