using System;
using System.Globalization;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.Converter
{
    public class PlayCountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 50000 ? $"{(int)value / 10000}万" : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
