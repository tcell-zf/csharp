using System.IO;
using System.Reflection;

namespace TCell.Text
{
    sealed public class Environments
    {
        static private string applcationPath = string.Empty;
        static public string ApplicationPath
        {
            get
            {
                if (string.IsNullOrEmpty(applcationPath))
                    applcationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                return applcationPath;
            }
        }
    }
}
