using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NeteaseCloudMusic.Wpf.Converter
{
    public class PlayingToBrushConverter : IValueConverter
    {
        public static SolidColorBrush _brushUnPlaying = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
        public static SolidColorBrush _brushPlaying = new SolidColorBrush(Color.FromRgb(0xc3, 0x2d, 0x2e));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? _brushPlaying : _brushUnPlaying;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
