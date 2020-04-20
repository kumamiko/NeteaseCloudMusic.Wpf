using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NeteaseCloudMusic.Wpf.Config;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Pages;
using NeteaseCloudMusic.Wpf.Services;
using NeteaseCloudMusic.Wpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        /// <summary>
        /// 退出保存时，检查frame的content类型，如果不是下面几个就保存为默认值
        /// 因为像 ArtistPage 初始化时，需要带上 参数，否则对应的VM无法初始化，懒得再保存一个变量了
        /// _(:з)∠)_
        /// </summary>
        private List<Type> _typesForChecking = new List<Type>
        {
            typeof(SearchPage),
            typeof(LocalMusicPage),
            typeof(DownloadPage),
            typeof(AboutPage),
        };

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, __) => Init();

            Messenger.Default.Register<bool>(this, "Play", PlayMusic);
            Messenger.Default.Register<string>(this, "ShowInfo", ShowInfo);
            Messenger.Default.Register<(Type, long)>(this, "Navigate", NavigateTo);
            Messenger.Default.Register<bool>(this, "RepeatSingle", RepeatSingle);

            Unloaded += (_, __) =>
            {
                Messenger.Default.Unregister<bool>(this, "Play", PlayMusic);
                Messenger.Default.Unregister<string>(this, "ShowInfo", ShowInfo);
                Messenger.Default.Unregister<(Type, long)>(this, "Navigate", NavigateTo);
                Messenger.Default.Unregister<bool>(this, "RepeatSingle", RepeatSingle);
            };

            Closing += (_, __) => SaveSetting();
        }

        private void ChangeRepeatModeIcon(int obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 单曲循环
        /// </summary>
        /// <param name="obj"></param>
        private void RepeatSingle(bool obj)
        {
            media.Position = TimeSpan.Zero;
        }

        /// <summary>
        /// 跳转到其他页面
        /// </summary>
        /// <param name="obj"></param>
        private void NavigateTo((Type pageType, long id) obj)
        {
            if (ucNav.listviewNavigation.SelectedIndex != -1)
                ucNav.listviewNavigation.SelectedIndex = -1;
            else
                ListView_SelectionChanged(null, null);


            Page page = null;
            //跳入歌手页面
            if (obj.pageType == typeof(ArtistPage))
            {
                var artistVM = new ArtistViewModel((int)obj.id);
                page = Activator.CreateInstance(obj.pageType, artistVM) as ArtistPage;
            }
            //跳入专辑页面
            if (obj.pageType == typeof(AlbumPage))
            {
                var albumVM = new AlbumViewModel((int)obj.id);
                page = Activator.CreateInstance(obj.pageType, albumVM) as AlbumPage;
            }
            //跳入MV页面
            if (obj.pageType == typeof(MvPage))
            {
                var mvVM = new MvViewModel((int)obj.id);
                page = Activator.CreateInstance(obj.pageType, mvVM) as MvPage;
            }
            //跳入PlayList页面
            if (obj.pageType == typeof(PlayListPage))
            {
                var playlistVM = new PlayListViewModel(obj.id);
                page = Activator.CreateInstance(obj.pageType, playlistVM) as PlayListPage;
            }

            if (page != null)
            {
                frame.Content = page;
            }
        }

        private void Init()
        {
            LoadSetting();

            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();

            titleBar.btnMinimum.Click += (_, __) =>
            {
                this.WindowState = WindowState.Minimized;
            };

            titleBar.btnMaximumAndRestore.Click += (_, __) =>
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            };

            titleBar.btnClose.Click += (_, __) => Close();

            titleBar.btnGoBack.Click += NavigateBack;

            this.StateChanged += (_, e) =>
            {
                titleBar.btnMaximumAndRestore.Content = this.WindowState == WindowState.Maximized ? "\uE923" : "\uE739";
                (titleBar.btnMaximumAndRestore.ToolTip as ToolTip).Content = this.WindowState == WindowState.Maximized ? "还原" : "最大化";
            };

            ucNav.btnOpenCloseSetting.Click += OpenCloseSetting;
            ucNav.btnOpenCloseNavigation.Click += OpenCloseNavigation;

            ucNav.listviewNavigation.SelectionChanged += ListView_SelectionChanged;

            CoverRotateStoryBoard = this.FindResource("CoverRotate") as Storyboard;
        }
        private void SaveSetting()
        {
            GlobalData.Config.MainRestoreBounds = this.RestoreBounds;
            GlobalData.Config.MainWindowState = this.WindowState;
            if (frame.Content.GetType() is Type type)
            {
                if (_typesForChecking.Find(t => t == type) != null)
                    GlobalData.Config.LastPage = frame.Content.GetType();
                else
                    GlobalData.Config.LastPage = typeof(LocalMusicPage);
            }
            GlobalData.Config.LastSelectedIndex = ucNav.listviewNavigation.SelectedIndex == -1 ? 1 : ucNav.listviewNavigation.SelectedIndex;
        }

        private void LoadSetting()
        {
            //读取配置文件
            this.WindowState = WindowState.Normal;
            if (GlobalData.Config.MainRestoreBounds is var rec)
            {
                this.Left = rec.Left;
                this.Top = rec.Top;
                this.Height = rec.Height;
                this.Width = rec.Width;
            }
            if (GlobalData.Config.MainWindowState is var state)
            {
                this.WindowState = state;
            }

            if (GlobalData.Config.LastPage is Type pageType)
            {
                frame.Content = Activator.CreateInstance(pageType);
            }

            if (GlobalData.Config.LastSelectedIndex != -1)
            {
                ucNav.listviewNavigation.SelectedIndex = GlobalData.Config.LastSelectedIndex;
                BackIndexStack.Push(GlobalData.Config.LastSelectedIndex);
            }
        }

        private void PlayMusic(bool flag)
        {
            playing = false;
            PauseOrResume(null, null);
        }

        #region Frame 前进后退
        private Stack<int> BackIndexStack = new Stack<int>();
        private bool IsGoBack = false;
        private static readonly object BackLock = new object();
        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            lock (BackLock)
            {
                IsGoBack = true;
                if (BackIndexStack.Count > 1)
                {
                    int currentIndex = BackIndexStack.Pop();//最顶层去掉
                    int lastIndex = BackIndexStack.Pop();//倒数第二个才是上次的 Index
                    this.ucNav.listviewNavigation.SelectedIndex = lastIndex;
                    if (currentIndex == -1 && lastIndex == -1) ListView_SelectionChanged(null, null);//两次都是 -1，无法自动触发，手动触发
                    BackIndexStack.Push(lastIndex);//重新塞回倒数第二个
                }
                IsGoBack = false;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ucNav.listviewNavigation.SelectedIndex > ucNav.listviewNavigation.Items.Count - 1)
                return;

            lock (BackLock)
            {
                if (!IsGoBack)
                {
                    BackIndexStack.Push(ucNav.listviewNavigation.SelectedIndex);
                    if (ucNav.listviewNavigation.SelectedIndex == -1) return;
                    frame.Content = Activator.CreateInstance((ucNav.listviewNavigation.SelectedItem as NavigationItem).PageType);
                }
                else
                {
                    if (frame.CanGoBack) frame.GoBack();
                }
            }
        }

        private void FrameMain_Navigated(object sender, NavigationEventArgs e) => CheckFrameCanGoBack();

        /// <summary>
        /// 检查能否后退
        /// </summary>
        private void CheckFrameCanGoBack()
        {
            titleBar.btnGoBack.Visibility = frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameMain_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
                (this.TryFindResource("SB_IN") as Storyboard).Begin(e.Content as Page);
            else
                (this.TryFindResource("SB_BACK") as Storyboard).Begin(e.Content as Page);
        }
        #endregion

        private bool openSetting = false;
        private void OpenCloseSetting(object sender, RoutedEventArgs e)
        {
            if (openSetting) (this.FindResource("SettingClose") as Storyboard).Begin();
            else (this.FindResource("SettingOpen") as Storyboard).Begin();

            openSetting = !openSetting;
        }

        private bool open = true;
        private void OpenCloseNavigation(object sender, RoutedEventArgs e)
        {
            if (open) (this.FindResource("Close") as Storyboard).Begin(ucNav);
            else (this.FindResource("Open") as Storyboard).Begin(ucNav);

            open = !open;
        }

        DispatcherTimer timer = null;
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

        private bool openpanel = false;
        private void BtnPanel_Click(object sender, RoutedEventArgs e)
        {
            if (openpanel) (this.FindResource("ClosePanel") as Storyboard).Begin();
            else (this.FindResource("OpenPanel") as Storyboard).Begin();

            openpanel = !openpanel;
            btnPanel.Content = openpanel ? "\uE1D8" : "\uE1D9";
        }

        private bool needleFlag = true;
        private void BtnNeedle_Click(object sender, RoutedEventArgs e)
        {
            if (needleFlag) (this.FindResource("NeedleOn") as Storyboard).Begin();
            else (this.FindResource("NeedleOff") as Storyboard).Begin();

            if (needleFlag)
            {
                if (!AnimationStart)
                {
                    CoverRotateStoryBoard.Begin();
                    AnimationStart = true;
                }
                else
                    CoverRotateStoryBoard.Resume();
            }
            else
                CoverRotateStoryBoard.Pause();

            needleFlag = !needleFlag;
        }

        private Storyboard CoverRotateStoryBoard;
        private bool AnimationStart = false;

        private bool playing = true;
        private void PauseOrResume(object sender, RoutedEventArgs e)
        {
            if (playing)
            {
                thumbButtonPauseOrResume.ImageSource = this.FindResource("Image_Play") as BitmapImage;
                media.Pause();
                timer?.Stop();
            }
            else
            {
                thumbButtonPauseOrResume.ImageSource = this.FindResource("Image_Pause") as BitmapImage;
                media.Play();
            }

            timer?.Start();

            playing = !playing;

            txtPlayOrPause.Text = playing ? "\uE103" : "\uE102";
        }

        private bool changeSliderFlag = false;


        private void Slider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            media.Position = TimeSpan.FromMilliseconds(slider.Value);
            changeSliderFlag = false;
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed) changeSliderFlag = true;
        }

        private void GridMask_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenCloseSetting(null, null);
        }

        private void btnList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (musicListView.SelectedItem != null)
                musicListView.ScrollIntoView(musicListView.SelectedItem);
        }

        private void ShowInfo(string info)
        {
            txtInfo.Text = info;
            (this.FindResource("ShowInfo") as Storyboard).Begin();
        }

        private void ComboboxSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            media.SpeedRatio = double.Parse((comboboxSpeed.SelectedItem as ComboBoxItem).Content.ToString());
        }

        private void thumbButton_PauseOrResume(object sender, EventArgs e)
        {
            PauseOrResume(null, null);
        }

        private void OpenClosePlayingList(object sender, RoutedEventArgs e)
        {
            borderPlayingList.Visibility = borderPlayingList.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
