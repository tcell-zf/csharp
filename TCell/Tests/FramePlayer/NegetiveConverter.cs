using System;
using System.Globalization;
using System.Windows.Data;

namespace FramePlayer
{
    public class NegetiveConverter : IValueConverter
    {
        public NegetiveConverter()
        {
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return -100;
        }
    }
}
