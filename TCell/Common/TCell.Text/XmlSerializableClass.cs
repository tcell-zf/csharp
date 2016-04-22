using System;
using System.IO;
using System.Xml.Serialization;

namespace TCell.Text
{
    [Serializable]
    public class XmlSerializableClass
    {
        public static T Load<T>(string filepath)
        {
            T obj = default(T);

            XmlSerializer formatter = new XmlSerializer(typeof(T));
            try
            {
                Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                obj = (T)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                obj = default(T);
            }

            return obj;
        }

        public bool Save<T>(string filepath)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            try
            {
                Stream stream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
