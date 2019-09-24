using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace NeteaseCloudMusic.Wpf.Controls
{
    public class FadeInImage : Image
    {
        static FadeInImage()
        {
            Image.SourceProperty.OverrideMetadata(typeof(FadeInImage), new FrameworkPropertyMetadata(SourcePropertyChanged));
        }

        private static void SourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Image image = obj as Image;
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            image.BeginAnimation(OpacityProperty, da);
        }
    }
}
