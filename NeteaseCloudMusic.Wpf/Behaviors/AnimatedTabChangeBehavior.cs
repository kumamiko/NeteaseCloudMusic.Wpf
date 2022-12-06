using Microsoft.Xaml.Behaviors;
using NeteaseCloudMusic.Wpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace NeteaseCloudMusic.Wpf.Behaviors
{
    public class AnimatedTabChangeBehavior : Behavior<AnimatedTab>
    {
        private int _lastSelectedIndex = -1;

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += SelectionChanged;
            AssociatedObject.SelectionChanging += SelectionChanging;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= SelectionChanged;
            AssociatedObject.SelectionChanging -= SelectionChanging;
        }

        private void SelectionChanging(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedIndex == -1) return;

            if ((AssociatedObject is AnimatedTab tab) && AssociatedObject.Items[_lastSelectedIndex] is TabItem tabitem && tabitem.Content is FrameworkElement content)
            {
                if (AssociatedObject.SelectedIndex > _lastSelectedIndex)
                {
                    ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0), new Thickness(-100, 0, 0, 0), tab.SelectionChangingDuration);
                    ta.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn };
                    content.BeginAnimation(FrameworkElement.MarginProperty, ta);
                    DoubleAnimation da = new DoubleAnimation(1, 0, tab.SelectionChangingDuration);
                    content.BeginAnimation(FrameworkElement.OpacityProperty, da);
                }
                if (AssociatedObject.SelectedIndex < _lastSelectedIndex)
                {
                    ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0), new Thickness(100, 0, 0, 0), tab.SelectionChangingDuration);
                    ta.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn };
                    content.BeginAnimation(FrameworkElement.MarginProperty, ta);
                    DoubleAnimation da = new DoubleAnimation(1, 0, tab.SelectionChangingDuration);
                    content.BeginAnimation(FrameworkElement.OpacityProperty, da);
                }
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((AssociatedObject is AnimatedTab tab) && AssociatedObject.SelectedItem is TabItem tabitem && tabitem.Content is FrameworkElement content)
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
                    ThicknessAnimation ta = new ThicknessAnimation(new Thickness(-200, 0, 0, 0), new Thickness(0), TimeSpan.FromSeconds(0.25));
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
