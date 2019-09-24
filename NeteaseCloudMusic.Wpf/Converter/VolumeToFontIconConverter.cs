using System;
using System.Globalization;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.Converter
{
    public class VolumeToFontIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value > 0 ? "\uE994" : "\uE992";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
