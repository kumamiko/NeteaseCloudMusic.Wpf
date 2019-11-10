using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Config;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Pages;
using NeteaseCloudMusic.Wpf.Services;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<NavigationItem> NavigationList { get; } = new ObservableCollection<NavigationItem>() {
            new NavigationItem{ Header = "搜索", Icon = "\uE094", PageType = typeof(SearchPage) },
            new NavigationItem{ Header = "本地音乐", Icon = "\uE142", PageType = typeof(LocalMusicPage) },
            new NavigationItem{ Header = "下载管理", Icon = "\uE896", PageType = typeof(DownloadPage) },
            new NavigationItem{ Header = "关于", Icon = "\uE946", PageType = typeof(AboutPage) },
         };

        private LocalMusicViewModel _localMusicVM;
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private TimeSpan _position;
        private ObservableCollection<MusicInfo> _playlistMusics = new ObservableCollection<MusicInfo>();
        private MusicInfo _selectMusicInfo;
        private PlayMode _playingMode = PlayMode.Order;
        private static BitmapImage _defaultcover = new BitmapImage(new Uri("pack://application:,,,/Resources/Image/default_cover.png", UriKind.RelativeOrAbsolute));
        private BitmapImage _cover = _defaultcover;

        #region 属性
        private bool _isThumbButtonEnabled = false;
        
        public bool IsThumbButtonEnabled
        {
            get => _isThumbButtonEnabled;
            set => Set(ref _isThumbButtonEnabled, value);
        }
        public LocalMusicViewModel LocalMusicVM
        {
            get => _localMusicVM;
            set => Set(ref _localMusicVM, value);
        }

        public BitmapImage Cover
        {
            get => _cover;
            set => Set(ref _cover, value);
        }

        public TimeSpan Position
        {
            get => _position;
            set => Set(ref _position, value);
        }

        public ObservableCollection<MusicInfo> PlaylistMusics
        {
            get => _playlistMusics;
            set => Set(ref _playlistMusics, value);
        }

        public string SaveFolder
        {
            get => GlobalData.Config.SaveFolder;
            set
            {
                GlobalData.Config.SaveFolder = value;
                RaisePropertyChanged();
            }
        }

        public PlayMode PlayingMode
        {
            get => _playingMode;
            set => Set(ref _playingMode, value);
        }

        public MusicInfo SelectMusicInfo
        {
            get => _selectMusicInfo;
            set
            {
                Set(ref _selectMusicInfo, value);
                if (value.Type == 0) ChangeCover(value);/*本地音乐读取封面*/
                if (value.Type == 1) Task.Run(async () =>
                {
                    var coverAndDetail = await _NeteaseCloudMusicService.GetCoverAndDetailAsync(value.Id);
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Cover = coverAndDetail.cover;
                        if (string.IsNullOrEmpty(value.Duration))
                        {
                            var duration = TimeSpan.FromMilliseconds(coverAndDetail.detail.songs[0].duration);
                            value.Duration = $"{(int)duration.TotalMinutes:00}:{duration.Seconds:00}";
                        }
                    });
                });
                MessengerInstance.Send<bool>(true, "Play");
                IsThumbButtonEnabled = true;
            }
        }

        #endregion

        #region 命令
        public RelayCommand ChangeSaveFolderCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ChangeSaveFolder)).Value;

        public RelayCommand SavePlaylistMusicsCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(SavePlaylistMusics)).Value;

        public RelayCommand LoadPlaylistMusicsCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(LoadPlaylistMusics)).Value;

        public RelayCommand ClearPlaylistMusicsCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ClearPlaylistMusics)).Value;

        public RelayCommand ClearLocalMusicCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ClearLocalMusic)).Value;

        public RelayCommand<object> RemoveFromListCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(RemoveFromList)).Value;

        public RelayCommand ChangePlayingModeCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ChangePlayingMode)).Value;

        public RelayCommand<int> NextMusicCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(NextMusic)).Value;

        public RelayCommand<int> PrevMusicCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(PrevMusic)).Value;

        public RelayCommand<object> ChangeSelectMusicCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(ChangeSelectMusic)).Value;

        #endregion

        public MainViewModel()
        {
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();
        }

        #region 方法
        private void ChangeSaveFolder()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "请选择下载目录";
            dialog.UseDescriptionForTitle = true;
            if ((bool)dialog.ShowDialog())
                SaveFolder = dialog.SelectedPath;
        }

        public void NavigateTo(Type pageType, long id)
        {
            MessengerInstance.Send<(Type, long)>((pageType, id), "Navigate");
        }

        /// <summary>
        /// 加载播放列表
        /// </summary>
        public void LoadPlaylistMusics()
        {
            Task.Run(() =>
            {
                if (!File.Exists("playlist.json")) return;

                using (var sr = new StreamReader("playlist.json"))
                {
                    var jsonData = sr.ReadToEnd();
                    var musics = JsonConvert.DeserializeObject<List<MusicInfo>>(jsonData);
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        musics.ForEach(t => PlaylistMusics.Add(t));
                    });
                }
            });
        }

        /// <summary>
        /// 保存播放列表
        /// </summary>
        public void SavePlaylistMusics()
        {
            if (SelectMusicInfo is MusicInfo selectedMusic) selectedMusic.Playing = false;
            string jsonData = JsonConvert.SerializeObject(_playlistMusics);

            using (var sw = new StreamWriter("playlist.json"))
            {
                sw.Write(jsonData);
            }
        }

        /// <summary>
        /// 清空当前播放列表
        /// </summary>
        private void ClearPlaylistMusics()
        {
            PlaylistMusics.Clear();
        }

        /// <summary>
        /// 从当前播放列表清除
        /// </summary>
        /// <param name="selectItems"></param>
        private void RemoveFromList(object selectItems)
        {
            var musics = (selectItems as ObservableCollection<object>).Cast<MusicInfo>().ToList();

            foreach (var item in musics)
            {
                PlaylistMusics.Remove(item);
            }
        }

        private void NextMusic(int obj)
        {
            if (SelectMusicInfo == null) return;
            var index = PlaylistMusics.IndexOf(PlaylistMusics.FirstOrDefault(t => t.Id == SelectMusicInfo.Id));

            switch (_playingMode)
            {
                case PlayMode.Order:/*顺序播放*/
                    if (index != -1 && index < PlaylistMusics.Count - 1)
                    {
                        SelectMusicInfo.Playing = false;
                        SelectMusicInfo = PlaylistMusics[index + 1];
                        SelectMusicInfo.Playing = true;
                    }
                    break;
                case PlayMode.RepeatAll:/*重复全部*/
                    if (index == -1) return;

                    SelectMusicInfo.Playing = false;
                    if (index >= PlaylistMusics.Count - 1)
                    {
                        SelectMusicInfo = PlaylistMusics[0];
                        SelectMusicInfo.Playing = true;
                    }
                    else
                    {
                        SelectMusicInfo = PlaylistMusics[index + 1];
                        SelectMusicInfo.Playing = true;
                    }
                    break;
                case PlayMode.RepeatOne:/*单曲循环*/
                    MessengerInstance.Send<bool>(true, "RepeatSingle");
                    break;
            }
        }
        private void PrevMusic(int obj)
        {
            var index = PlaylistMusics.IndexOf(SelectMusicInfo);

            if (index > 0)
            {
                if (SelectMusicInfo != null) SelectMusicInfo.Playing = false;
                SelectMusicInfo = PlaylistMusics[index - 1];
                SelectMusicInfo.Playing = true;
            }

        }

        private void ChangeCover(MusicInfo t)
        {
            var tfile = TagLib.File.Create(t.File);

            BitmapImage bitmap = null;
            var pics = tfile.Tag.Pictures;
            if (pics != null && pics.Length > 0)
            {
                TagLib.IPicture pic = pics[0];
                using (var stream = new MemoryStream(pic.Data.Data))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                Cover = bitmap;
            }
            else
            {
                Cover = _defaultcover;
            }
        }

        public void ChangePlayingMode()
        {
            PlayingMode++;

            if ((int)PlayingMode > 3) PlayingMode = PlayMode.Order;

            string info = string.Empty;
            switch (PlayingMode)
            {
                case PlayMode.Order:
                    info = "顺序播放";
                    break;
                case PlayMode.RepeatOne:
                    info = "单曲循环";
                    break;
                case PlayMode.RepeatAll:
                    info = "全部循环";
                    break;
                case PlayMode.Random:
                    info = "随机播放，还没做_(:з)∠)_";
                    break;
            }
            MessengerInstance.Send<string>(info, "ShowInfo");
        }

        private void ChangeSelectMusic(object selectedItem)
        {
            if (selectedItem is MusicInfo music)
            {
                if (SelectMusicInfo != null) SelectMusicInfo.Playing = false;
                SelectMusicInfo = music;
                SelectMusicInfo.Playing = true;
                if (!PlaylistMusics.Any(t => t.Id == music.Id)) PlaylistMusics.Add(music);
            }
        }

        private void ClearLocalMusic()
        {
            if (_localMusicVM == null) _localMusicVM = SimpleIoc.Default.GetInstance<LocalMusicViewModel>();

            _localMusicVM.ClearLocalMusic();
        }
    }
    #endregion
}