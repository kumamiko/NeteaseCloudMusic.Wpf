using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Config;
using NeteaseCloudMusic.Wpf.Helper;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Pages;
using NeteaseCloudMusic.Wpf.Services;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        private DataService _dataservice;
        private NeteaseCloudMusicService _NeteaseCloudMusicservice;
        private LocalMusicViewModel _localMusicVM;
        private DownloadViewModel _downloadVM;
        private MainViewModel _mainVM;

        private string _keyword;

        private int _searchTabIndex;

        private ObservableCollection<History> _histories = new ObservableCollection<History>();
        private ObservableCollection<MusicInfo> _musics = new ObservableCollection<MusicInfo>();
        private ObservableCollection<ArtistInfo> _artists = new ObservableCollection<ArtistInfo>();
        private ObservableCollection<AlbumInfo> _albums = new ObservableCollection<AlbumInfo>();
        private ObservableCollection<MvInfo> _mvs = new ObservableCollection<MvInfo>();
        private ObservableCollection<PlaylistInfo> _playlists = new ObservableCollection<PlaylistInfo>();
        private ObservableCollection<Radio> _radios = new ObservableCollection<Radio>();

        #region 属性
        public string Keyword
        {
            get => _keyword;
            set => Set(ref _keyword, value);
        }

        public int SearchTabIndex
        {
            get => _searchTabIndex;
            set => Set(ref _searchTabIndex, value);
        }

        public ObservableCollection<History> Histories
        {
            get => _histories;
            set => Set(ref _histories, value);
        }

        public ObservableCollection<MusicInfo> Musics
        {
            get => _musics;
            set => Set(ref _musics, value);
        }

        public ObservableCollection<ArtistInfo> Artists
        {
            get => _artists;
            set => Set(ref _artists, value);
        }

        public ObservableCollection<AlbumInfo> Albums
        {
            get => _albums;
            set => Set(ref _albums, value);
        }

        public ObservableCollection<MvInfo> Mvs
        {
            get => _mvs;
            set => Set(ref _mvs, value);
        }

        public ObservableCollection<PlaylistInfo> Playlists
        {
            get => _playlists;
            set => Set(ref _playlists, value);
        }

        public ObservableCollection<Radio> Radios
        {
            get => _radios;
            set => Set(ref _radios, value);
        }
        #endregion

        #region 命令
        public RelayCommand ClearKeywordCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => Keyword = string.Empty)).Value;

        public RelayCommand SearchCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(Search)).Value;

        public RelayCommand SearchMoreCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(SearchMore)).Value;


        public RelayCommand<object> RemoveCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(Remove)).Value;

        public RelayCommand<string> ChangeSearchKeywordCmd => new Lazy<RelayCommand<string>>(() =>
            new RelayCommand<string>((t) =>
            {
                Keyword = t;
                Search();
            })).Value;

        public RelayCommand ClearHistoryCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ClearHistory)).Value;

        public RelayCommand<object> ChangeSelectMusicCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(ChangeSelectMusic)).Value;

        public RelayCommand<object> AddToListCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(AddToList)).Value;

        public RelayCommand<object> AddToDownloadCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(AddToDownload)).Value;

        public RelayCommand<int> ChangeTabSelectionCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(ChangeTabSelection)).Value;

        public RelayCommand<string> NavigateToArtistCmd => new Lazy<RelayCommand<string>>(() =>
            new RelayCommand<string>(NavigateToArtist)).Value;

        public RelayCommand<long> NavigateToPlayListCmd => new Lazy<RelayCommand<long>>(() =>
            new RelayCommand<long>(NavigateToPlayList)).Value;

        private void NavigateToPlayList(long id)
        {
            _mainVM?.NavigateTo(typeof(PlayListPage), id);
        }

        public RelayCommand<object> AddToPlaylistCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(AddToPlaylist)).Value;

        private void AddToPlaylist(object selectItems)
        {
            var musics = (selectItems as ObservableCollection<object>).Cast<MusicInfo>().ToList();

            var ids = _mainVM.PlaylistMusics.Select(t => t.Id);

            foreach (var item in musics)
            {
                if (ids.Contains(item.Id)) continue;
                _mainVM.PlaylistMusics.Add(item);
            }

            MessengerInstance.Send<string>("已加入播放列表", "ShowInfo");
        }

        private void NavigateToArtist(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            _mainVM?.NavigateTo(typeof(ArtistPage), int.Parse(id));
        }

        public RelayCommand<int> NavigateToAlbumCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(NavigateToAlbum)).Value;

        private void NavigateToAlbum(int id)
        {
            _mainVM?.NavigateTo(typeof(AlbumPage), id);
        }

        public RelayCommand<int> NavigateToMvCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(NavigateToMv)).Value;

        private void NavigateToMv(int id)
        {
            _mainVM?.NavigateTo(typeof(MvPage), id);
        }

        public RelayCommand<long> NavigateToRadioCmd => new Lazy<RelayCommand<long>>(() =>
            new RelayCommand<long>(NavigateToRadio)).Value;

        private void NavigateToRadio(long id)
        {
            _mainVM?.NavigateTo(typeof(RadioPage), id);
        }

        public RelayCommand NotYetCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => { MessengerInstance.Send<string>("没做...", "ShowInfo"); })).Value;
        #endregion


        public SearchViewModel(DataService dataService, NeteaseCloudMusicService NeteaseCloudMusicService)
        {
            _dataservice = dataService;
            _NeteaseCloudMusicservice = NeteaseCloudMusicService;
            _dataservice.GetAllHistory().ToList().ForEach(t => Histories.Add(t));
            _localMusicVM = SimpleIoc.Default.GetInstance<LocalMusicViewModel>();
            _downloadVM = SimpleIoc.Default.GetInstance<DownloadViewModel>();
            _mainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
        }

        private void AddToList(object selectItems)
        {
            var musics = (selectItems as ObservableCollection<object>).Cast<MusicInfo>().ToList();

            foreach (var music in musics)
            {
                if (_localMusicVM.Musics.Any(t => t.Id == music.Id)) continue;/*如果存在就不再添加*/

                _localMusicVM.Musics.Add(music);

                _dataservice.AddMusic(
                    new MusicInfo
                    {
                        Album = music.Album,
                        Artist = music.Artist,
                        Duration = music.Duration,
                        File = music.File,
                        Id = music.Id,
                        AlbumId = music.AlbumId,
                        Playing = false,
                        Name = music.Name,
                        Type = music.Type
                    });

                MessengerInstance.Send<string>("已加入本地音乐列表", "ShowInfo");
            }
        }

        private void AddToDownload(object selectItems)
        {
            if (_downloadVM == null) return;

            var musics = (selectItems as ObservableCollection<object>).Cast<MusicInfo>().ToList();

            if (GlobalData.Config.SaveFolder == string.Empty)
            {
                VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                dialog.Description = "请选择下载目录";
                dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
                if ((bool)dialog.ShowDialog())
                    GlobalData.Config.SaveFolder = dialog.SelectedPath;
            }

            if (!Directory.Exists(GlobalData.Config.SaveFolder)) Directory.CreateDirectory(GlobalData.Config.SaveFolder);

            foreach (var music in musics)
            {
                _downloadVM.AddAndStart($"{Path.Combine(GlobalData.Config.SaveFolder, music.Name.RemoveInvalidFileNameChars())}.mp3", music.File, music.Id);
            }

            MessengerInstance.Send<string>("已加入下载列表", "ShowInfo");
        }

        /// <summary>
        /// 切换tab
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeTabSelection(int index)
        {
            if (string.IsNullOrWhiteSpace(Keyword)) return;

            switch (index)
            {
                case 0:
                    if (_musics.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var musics = await _NeteaseCloudMusicservice.SearchMusicAsync2(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => musics.ForEach(t => Musics.Add(t)));
                    });
                    break;
                case 1:
                    if (_artists.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var artists = await _NeteaseCloudMusicservice.SearchArtistAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => artists.ForEach(t => Artists.Add(t)));
                    });
                    break;
                case 2:
                    if (_albums.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var albums = await _NeteaseCloudMusicservice.SearchAlbumAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => albums.ForEach(t => Albums.Add(t)));
                    });
                    break;
                case 3:
                    if (_mvs.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var mvs = await _NeteaseCloudMusicservice.SearchMvAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => mvs.ForEach(t => Mvs.Add(t)));
                    });
                    break;
                case 4:
                    if (_playlists.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var playlists = await _NeteaseCloudMusicservice.SearchPlayListAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => playlists.ForEach(t => Playlists.Add(t)));
                    });
                    break;
                case 5:
                    if (_radios.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var radios = await _NeteaseCloudMusicservice.SearchRadioAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => radios.ForEach(t => Radios.Add(t)));
                    });
                    break;
            }
        }

        private void ChangeSelectMusic(object selectedItem)
        {
            if (selectedItem is MusicInfo music)
            {
                if (_mainVM.SelectMusicInfo != null) _mainVM.SelectMusicInfo.Playing = false;
                _mainVM.SelectMusicInfo = music;
                _mainVM.SelectMusicInfo.Playing = true;
                if (!_mainVM.PlaylistMusics.Any(t => t.Id == music.Id)) _mainVM.PlaylistMusics.Add(music);
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(Keyword)) return;

            var history = new History { Keyword = Keyword, CreateTime = DateTime.Now };
            if (!Histories.Any(t => t.Keyword == Keyword))
            {
                Histories.Insert(0, history);
                _dataservice.AddHistory(history);
            }

            Musics.Clear();
            Artists.Clear();
            Albums.Clear();
            Mvs.Clear();
            Playlists.Clear();
            Radios.Clear();

            //Todo: 搜索
            switch (_searchTabIndex)
            {
                case 0:
                    Task.Run(async () =>
                    {
                        var musics = await _NeteaseCloudMusicservice.SearchMusicAsync2(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => musics.ForEach(t => Musics.Add(t)));
                    });
                    break;
                case 1:
                    Task.Run(async () =>
                    {
                        var artists = await _NeteaseCloudMusicservice.SearchArtistAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => artists.ForEach(t => Artists.Add(t)));
                    });
                    break;
                case 2:
                    Task.Run(async () =>
                    {
                        var albums = await _NeteaseCloudMusicservice.SearchAlbumAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => albums.ForEach(t => Albums.Add(t)));
                    });
                    break;
                case 3:
                    Task.Run(async () =>
                    {
                        var mvs = await _NeteaseCloudMusicservice.SearchMvAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => mvs.ForEach(t => Mvs.Add(t)));
                    });
                    break;
                case 4:
                    Task.Run(async () =>
                    {
                        var playlists = await _NeteaseCloudMusicservice.SearchPlayListAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => playlists.ForEach(t => Playlists.Add(t)));
                    });
                    break;
                case 5:
                    Task.Run(async () =>
                    {
                        var radios = await _NeteaseCloudMusicservice.SearchRadioAsync(Keyword);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => radios.ForEach(t => Radios.Add(t)));
                    });
                    break;
            }
        }

        private void SearchMore()
        {
            if (string.IsNullOrWhiteSpace(Keyword)) return;

            switch (_searchTabIndex)
            {
                case 0:
                    Task.Run(async () =>
                    {
                        var musics = await _NeteaseCloudMusicservice.SearchMusicAsync2(Keyword, _musics.Count);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => musics.ForEach(t => Musics.Add(t)));
                    });
                    break;
                case 1:
                    Task.Run(async () =>
                    {
                        var artists = await _NeteaseCloudMusicservice.SearchArtistAsync(Keyword, _artists.Count);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => artists.ForEach(t => Artists.Add(t)));
                    });
                    break;
                case 2:
                    Task.Run(async () =>
                    {
                        var albums = await _NeteaseCloudMusicservice.SearchAlbumAsync(Keyword, _albums.Count);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => albums.ForEach(t => Albums.Add(t)));
                    });
                    break;
                case 3:
                    Task.Run(async () =>
                    {
                        var mvs = await _NeteaseCloudMusicservice.SearchMvAsync(Keyword, _mvs.Count);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => mvs.ForEach(t => Mvs.Add(t)));
                    });
                    break;
                case 4:
                    Task.Run(async () =>
                    {
                        var playlists = await _NeteaseCloudMusicservice.SearchPlayListAsync(Keyword, _playlists.Count);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => playlists.ForEach(t => Playlists.Add(t)));
                    });
                    break;
                case 5:
                    Task.Run(async () =>
                    {
                        var radios = await _NeteaseCloudMusicservice.SearchRadioAsync(Keyword, _radios.Count);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => radios.ForEach(t => Radios.Add(t)));
                    });
                    break;
            }

        }

        private void ClearHistory()
        {
            Histories.Clear();
            _dataservice.ClearAllHistory();
        }
        private void Remove(object history)
        {
            Histories.Remove((History)history);
            _dataservice.RemoveHistory((History)history);
        }
    }
}
