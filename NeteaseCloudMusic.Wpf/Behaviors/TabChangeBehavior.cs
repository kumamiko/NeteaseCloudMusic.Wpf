using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace NeteaseCloudMusic.Wpf.Behaviors
{
    public class TabChangeBehavior : Behavior<TabControl>
    {
        private int _lastSelectedIndex = -1;

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject == null) return;

            if (AssociatedObject.SelectedItem is TabItem tabitem && tabitem.Content is FrameworkElement content)
            {
                if (AssociatedObject.SelectedIndex > _lastSelectedIndex)
                {
                    ThicknessAnimation ta = new ThicknessAnimation(new Thickness(200, 0, 0, 0), new Thickness(0), TimeSpan.FromSeconds(0.25));
                    ta.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
                    DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.15));
                    content.BeginAnimation(FrameworkElement.MarginProperty, ta);
                    content.BeginAnimation(FrameworkElement.OpacityProperty, da);
                }
                if (AssociatedObject.SelectedIndex < _lastSelectedIndex)
                {
                    ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0, 0, 200, 0), new Thickness(0), TimeSpan.FromSeconds(0.25));
                    ta.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
                    DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.15));
                    content.BeginAnimation(FrameworkElement.MarginProperty, ta);
                    content.BeginAnimation(FrameworkElement.OpacityProperty, da);
                }

            }

            _lastSelectedIndex = AssociatedObject.SelectedIndex;
        }
    }
}
