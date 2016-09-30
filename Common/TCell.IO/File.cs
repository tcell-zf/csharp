using System.IO;
using System.Text;

namespace TCell.IO
{
    public enum FileType
    {
        Unknown,

        EXE,

        JPG,
        GIF,
        PNG,
        BMP,

        WMA,
        WMV,
        AVI,
        MP3,
        MP4,
        RMVB,

        ZIP,
        RAR,

        SWF,

        PowerPoint
    }

    public enum FileCategory
    {
        Unknown,

        Image,
        Video,
        Audio,
        Executable,
        Compress,

        Flash,
        Gif,

        MicrosoftOffice
    }

    public class File
    {
        static public FileType GetFileType(string path)
        {
            string typeString = string.Empty;
            return GetFileType(path, out typeString);
        }

        static public FileType GetFileType(string path, out string typeString)
        {
            typeString = string.Empty;

            FileType type = FileType.Unknown;
            if (!System.IO.File.Exists(path))
                return type;

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            StringBuilder sb = new StringBuilder();
            byte by;
            try
            {
                by = br.ReadByte();
                sb.Append(by);
                by = br.ReadByte();
                sb.Append(by);
            }
            catch { }
            finally
            {
                br.Close();
                fs.Close();
            }

            switch (sb.ToString())
            {
                case "7790":
                    type = FileType.EXE;
                    break;

                case "4838":
                case "255216":
                    string extension = System.IO.Path.GetExtension(path).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg")
                        type = FileType.JPG;
                    else if (extension == ".wmv")
                        type = FileType.WMV;
                    else if (extension == ".wma")
                        type = FileType.WMA;
                    else { }
                    break;
                case "7173":
                    type = FileType.GIF;
                    break;
                case "6677":
                    type = FileType.BMP;
                    break;
                case "13780":
                    type = FileType.PNG;
                    break;
                case "8273":
                    type = FileType.AVI;
                    break;
                case "7368":
                case "255251":
                    type = FileType.MP3;
                    break;
                case "00":
                    type = FileType.MP4;
                    break;
                case "4682":
                    type = FileType.RMVB;
                    break;

                case "8075":
                    type = FileType.ZIP;
                    break;
                case "8297":
                    type = FileType.RAR;
                    break;
                case "6787":
                    type = FileType.SWF;
                    break;
                case "": // TBD
                    type = FileType.PowerPoint;
                    break;
                default:
                    break;
            }

            typeString = sb.ToString();
            return type;
        }

        static public FileCategory GetFileCategory(string path)
        {
            FileCategory category = FileCategory.Unknown;
            FileType type = GetFileType(path);
            switch (type)
            {
                case FileType.EXE:
                    category = FileCategory.Executable;
                    break;
                case FileType.JPG:
                case FileType.PNG:
                case FileType.BMP:
                    category = FileCategory.Image;
                    break;
                case FileType.WMA:
                case FileType.MP3:
                    category = FileCategory.Audio;
                    break;
                case FileType.WMV:
                case FileType.AVI:
                case FileType.MP4:
                case FileType.RMVB:
                    category = FileCategory.Video;
                    break;
                case FileType.ZIP:
                case FileType.RAR:
                    category = FileCategory.Compress;
                    break;
                case FileType.SWF:
                    category = FileCategory.Flash;
                    break;
                case FileType.GIF:
                    category = FileCategory.Gif;
                    break;
                default:
                    break;
            }
            return category;
        }
    }
}
