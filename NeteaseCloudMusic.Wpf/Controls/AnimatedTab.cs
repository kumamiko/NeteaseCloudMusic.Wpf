using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NeteaseCloudMusic.Wpf.Controls
{
    /// <summary>
    /// 参考这里👇
    /// https://neointelligence.wordpress.com/2011/01/11/neotab-animated-tab-control-in-wpf/
    /// </summary>
    public class AnimatedTab : TabControl
    {
        private DispatcherTimer timer;

        // this event will be fired when the user clicks to
        // change the index of the tab control
        public static readonly RoutedEvent SelectionChangingEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanging", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(AnimatedTab));

        public event RoutedEventHandler SelectionChanging
        {
            add { AddHandler(SelectionChangingEvent, value); }
            remove { RemoveHandler(SelectionChangingEvent, value); }
        }

        public static readonly DependencyProperty SelectionChangingDurationProperty = DependencyProperty.RegisterAttached(
                    "SelectionChangingDuration", typeof(TimeSpan), typeof(AnimatedTab), new PropertyMetadata(TimeSpan.FromSeconds(0.2)));

        public TimeSpan SelectionChangingDuration
        {
            get { return (TimeSpan)GetValue(SelectionChangingDurationProperty); }
            set { SetValue(SelectionChangingDurationProperty, value); }
        }

        public AnimatedTab()
        {
            DefaultStyleKey = typeof(AnimatedTab);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                (Action)delegate
                {
                    this.RaiseSelectionChangingEvent();

                    this.StopTimer();

                    this.timer = new DispatcherTimer { Interval = SelectionChangingDuration };

                    EventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        this.StopTimer();
                        base.OnSelectionChanged(e);
                    };
                    this.timer.Tick += handler;
                    this.timer.Start();
                });
        }

        // This method raises the event to change the tab items
        private void RaiseSelectionChangingEvent()
        {
            var args = new RoutedEventArgs(SelectionChangingEvent);
            RaiseEvent(args);
        }

        private void StopTimer()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer = null;
            }
        }
    }
}
