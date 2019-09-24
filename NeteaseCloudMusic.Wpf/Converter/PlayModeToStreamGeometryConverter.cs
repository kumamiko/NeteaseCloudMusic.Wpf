using NeteaseCloudMusic.Wpf.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NeteaseCloudMusic.Wpf.Converter
{
    public class PlayModeToStreamGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iconKey = string.Empty;
            switch ((PlayMode)value)
            {
                case PlayMode.Order:
                    iconKey = "Icon.Order";
                    break;
                case PlayMode.RepeatOne:
                    iconKey = "Icon.RepeatOne";
                    break;
                case PlayMode.RepeatAll:
                    iconKey = "Icon.RepeatAll";
                    break;
                case PlayMode.Random:
                    iconKey = "Icon.Shuffle";
                    break;
            }

            if (iconKey != string.Empty && App.Current.FindResource(iconKey) is StreamGeometry sg)
                return sg;
            else
                return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
