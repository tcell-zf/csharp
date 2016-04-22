using System.IO;
using System.Reflection;

namespace TCell.Text
{
    sealed public class Environments
    {
        static public string WebApplcationPath { get; set; }

        static public string ApplicationPath
        {
            get
            {
                if (string.IsNullOrEmpty(WebApplcationPath))
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                else
                    return WebApplcationPath;
            }
        }
    }
}
