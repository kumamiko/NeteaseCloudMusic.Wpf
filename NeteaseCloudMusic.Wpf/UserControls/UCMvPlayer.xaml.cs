using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NeteaseCloudMusic.Wpf.UserControls
{
    /// <summary>
    /// UCMvPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class UCMvPlayer : UserControl
    {
        private bool playing = false;
        DispatcherTimer timer = null;
        private bool changeSliderFlag = false;

        public UCMvPlayer()
        {
            InitializeComponent();

            Loaded += (_, __) => PauseOrResume(null, null);
        }

        private void PauseOrResume(object sender, RoutedEventArgs e)
        {
            if (playing)
            {
                media.Pause();
                timer?.Stop();
            }
            else media.Play();

            timer?.Start();

            playing = !playing;

            txtPlayOrPause.Text = playing ? "\uE103" : "\uE102";
        }

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            slider.Maximum = media.NaturalDuration.TimeSpan.TotalMilliseconds;
            //媒体文件打开成功
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            if (!changeSliderFlag) slider.Dispatcher.Invoke(() => slider.Value = media.Position.TotalMilliseconds);
            tbCurrentTime.Dispatcher.Invoke(() => tbCurrentTime.Text = $"{(int)media.Position.TotalMinutes:00}:{media.Position.Seconds:00}");
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            changeSliderFlag = true;
        }
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            media.Position = TimeSpan.FromMilliseconds(slider.Value);
            changeSliderFlag = false;
        }

        private void Slider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            media.Position = TimeSpan.FromMilliseconds(slider.Value);
            changeSliderFlag = false;
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed) changeSliderFlag = true;
        }
    }
}
