using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class LocalMusicViewModel : ViewModelBase
    {
        #region 服务
        private DataService _dataService;
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private MainViewModel _mainVM;
        #endregion

        #region 字段
        private ObservableCollection<MusicInfo> _musics = new ObservableCollection<MusicInfo>();
        #endregion

        #region 属性
        public ObservableCollection<MusicInfo> Musics
        {
            get => _musics;
            set => Set(ref _musics, value);
        }

        public MainViewModel MainVM
        {
            get => _mainVM;
            set => Set(ref _mainVM, value);
        }

        #endregion

        #region 命令
        public RelayCommand GetAllMusicInfoCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(GetAllMusicInfo)).Value;

        public RelayCommand<object> RemoveFromListCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(RemoveFromList)).Value;

        public RelayCommand<object> AddToPlaylistCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(AddToPlaylist)).Value;


        public RelayCommand ClearLocalMusicCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ClearLocalMusic)).Value;

        public RelayCommand NotYetCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => { MessengerInstance.Send<string>("没做...", "ShowInfo"); })).Value;

        #endregion

        public LocalMusicViewModel(DataService dataService, NeteaseCloudMusicService NeteaseCloudMusicService)
        {
            _dataService = dataService;
            _NeteaseCloudMusicService = NeteaseCloudMusicService;
            MainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
            _dataService.GetAllMusic().OrderBy(t => t.Id)
                                      .ToList()
                                      .ForEach(t => Musics.Add(t));
        }

        private void ClearLocalMusic()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "MP3文件|*.mp3|所有文件|*.*"
            };


            if ((bool)openfiledialog.ShowDialog())
            {
                var m = Musics.Where(t => t.Type == 0).ToList();
                foreach (var t in m)
                {
                    Musics.Remove(t);
                }
                _dataService.ClearLocalMusic();

                var folderpath = System.IO.Path.GetDirectoryName(openfiledialog.FileName);

                DirectoryInfo currentDir = new DirectoryInfo(folderpath);
                FileInfo[] files = currentDir.GetFiles();

                int i = 1;
                foreach (var t in files)
                {
                    if (t.Extension == ".mp3")
                    {

                        var tfile = TagLib.File.Create(t.FullName);
                        var title = tfile.Tag.Title ?? t.Name;
                        var artist = string.Join("/", tfile.Tag.Performers);
                        var durationTimespan = tfile.Properties.Duration;
                        var duration = $"{(int)durationTimespan.TotalMinutes:00}:{durationTimespan.Seconds:00}";
                        var album = string.IsNullOrEmpty(tfile.Tag.Album) ? tfile.Tag.Album : "未知";

                        Musics.Add(new MusicInfo { Id = i++, Name = title, Artist = artist, Duration = duration, File = t.FullName, Type = 0, Album = album });
                    }
                }

                _dataService.SaveAllMusic(Musics.Where(t => t.Type == 0).ToList());
            }
        }
        private void RemoveFromList(object selectItems)
        {
            var musics = (selectItems as ObservableCollection<object>).Cast<MusicInfo>().ToList();

            foreach (var item in musics)
            {
                Musics.Remove(item);
                _dataService.RemoveMusic(item);
            }
        }

        private void AddToPlaylist(object selectItems)
        {
            var musics = (selectItems as ObservableCollection<object>).Cast<MusicInfo>().ToList();

            var ids = _mainVM.PlaylistMusics.Select(t => t.Id);

            foreach (var item in musics)
            {
                if (ids.Contains(item.Id)) continue;
                _mainVM.PlaylistMusics.Add(item);
            }
        }

        private void GetAllMusicInfo()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "MP3文件|*.mp3|所有文件|*.*"
            };


            if ((bool)openfiledialog.ShowDialog())
            {
                Musics.Clear();

                var folderpath = System.IO.Path.GetDirectoryName(openfiledialog.FileName);

                DirectoryInfo currentDir = new DirectoryInfo(folderpath);
                FileInfo[] files = currentDir.GetFiles();

                int i = 1;
                foreach (var t in files)
                {
                    if (t.Extension == ".mp3")
                    {

                        var tfile = TagLib.File.Create(t.FullName);
                        var title = tfile.Tag.Title ?? t.Name;
                        var artist = string.Join("/", tfile.Tag.Performers);
                        var durationTimespan = tfile.Properties.Duration;
                        var duration = $"{(int)durationTimespan.TotalMinutes:00}:{durationTimespan.Seconds:00}";
                        var album = !string.IsNullOrEmpty(tfile.Tag.Album) ? tfile.Tag.Album : "未知";

                        Musics.Add(new MusicInfo { Id = i++, Name = title, Artist = artist, Duration = duration, File = t.FullName, Type = 0, Album = album, Playing = false });
                    }
                }
                _dataService.ClearAllMusic();

                _dataService.SaveAllMusic(Musics.ToList());
            }
        }

        private void LoadCover()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*"
            };

            if ((bool)openfiledialog.ShowDialog())
            {
                _mainVM.Cover = new BitmapImage(new Uri(openfiledialog.FileName));
            }
        }

    }
}
