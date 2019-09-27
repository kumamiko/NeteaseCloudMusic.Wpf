using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Collections.Generic;
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
        public RelayCommand GetAllMusicsCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(GetAllMusics)).Value;

        public RelayCommand AddSomeMusicsCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(AddSomeMusics)).Value;

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

        public void ClearLocalMusic()
        {
            Musics.Clear();
            _dataService.ClearAllMusic();
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

        private void AddSomeMusics()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.CheckFileExists = true;//检查文件是否存在
            openFile.CheckPathExists = true;//检查路径是否存在
            openFile.Multiselect = true;//是否允许多选，false表示单选
            //openFile.InitialDirectory = "C:\\";//设置打开时的默认路径，我这里设置为C盘根目录
            string filter = "mp3,wav,aac,flac";
            filter = filter.TrimEnd(',');
            if (filter.Equals(""))
            {
                filter = "*";
            }
            filter = filter.Replace(",", ";*.");
            filter = "*." + filter;
            openFile.Filter = "音频文件 (" + filter + ")|" + filter + "|所有文件 (*.*)|*.*";//这里设置的是文件过滤器，比如选了txt文件，那别的文件就看不到了

            int index = Musics.Count + 1;

            if ((bool)openFile.ShowDialog())//打开文件选择器，并按下选择按钮
            {
                String[] names = openFile.FileNames;
                for (int i = 0; i < names.Length; i++)
                {
                    string[] exts = { ".mp3", ".flac", ".aac", ".wav" };//仅添加这几种格式

                    FileInfo file = new FileInfo(names[i]);

                    if (exts.Contains(file.Extension))
                    {

                        var tfile = TagLib.File.Create(file.FullName);
                        var title = tfile.Tag.Title ?? file.Name;
                        var artist = string.Join("/", tfile.Tag.Performers);
                        var durationTimespan = tfile.Properties.Duration;
                        var duration = $"{(int)durationTimespan.TotalMinutes:00}:{durationTimespan.Seconds:00}";
                        var album = !string.IsNullOrEmpty(tfile.Tag.Album) ? tfile.Tag.Album : "未知";

                        MusicInfo music = new MusicInfo { Id = index++, Name = title, Artist = artist, Duration = duration, File = file.FullName, Type = 0, Album = album, Playing = false };
                        Musics.Add(music);
                        _dataService.AddMusic(music);
                    }
                }
            }
        }

        private void GetAllMusics()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "音频文件|*.mp3;*.aac;*.wav;*.flac|所有文件|*.*"
            };


            if ((bool)openfiledialog.ShowDialog())
            {
                Musics.Clear();

                var folderpath = System.IO.Path.GetDirectoryName(openfiledialog.FileName);

                DirectoryInfo currentDir = new DirectoryInfo(folderpath);
                FileInfo[] files = currentDir.GetFiles();

                int i = 1;
                string[] exts = { ".mp3", ".flac", ".aac", ".wav" };//仅添加这几种格式
                foreach (var t in files)
                {
                    if (exts.Contains(t.Extension))
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
