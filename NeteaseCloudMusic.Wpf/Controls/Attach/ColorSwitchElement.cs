using System.Windows;
using System.Windows.Media;

namespace NeteaseCloudMusic.Wpf.Controls.Attach
{
    public class ColorSwitchElement
    {
        public static readonly DependencyProperty MouseHoverBackgroundColorProperty = DependencyProperty.RegisterAttached(
            "MouseHoverBackgroundColor", typeof(Color), typeof(ColorSwitchElement), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetMouseHoverBackgroundColor(DependencyObject element, Color value) => element.SetValue(MouseHoverBackgroundColorProperty, value);

        public static Color GetMouseHoverBackgroundColor(DependencyObject element) => (Color)element.GetValue(MouseHoverBackgroundColorProperty);

        public static readonly DependencyProperty MouseHoverForegroundColorProperty = DependencyProperty.RegisterAttached(
            "MouseHoverForegroundColor", typeof(Color), typeof(ColorSwitchElement), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetMouseHoverForegroundColor(DependencyObject element, Color value) => element.SetValue(MouseHoverForegroundColorProperty, value);

        public static Color GetMouseHoverForegroundColor(DependencyObject element) => (Color)element.GetValue(MouseHoverForegroundColorProperty);
    }
}
