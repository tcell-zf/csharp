using System.Diagnostics;

namespace TCell.Text
{
    public class ProcessExecutor
    {
        static public bool StartProcess(string filename, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = filename,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return (Process.Start(psi) != null);
        }
    }
}
