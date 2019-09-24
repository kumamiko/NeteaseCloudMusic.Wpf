using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Pages;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class ArtistViewModel : ViewModelBase
    {
        #region 字段
        private int _artistNo;
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private DownloadService _downloadService;
        private SearchViewModel _searchVM;
        private MainViewModel _mainVM;
        private ArtistInfo _artistInfo = new ArtistInfo();
        private ObservableCollection<AlbumInfo> _albums = new ObservableCollection<AlbumInfo>();
        private ObservableCollection<MvInfo> _mvs = new ObservableCollection<MvInfo>();
        private ObservableCollection<MusicInfo> _hotSongs = new ObservableCollection<MusicInfo>();
        #endregion

        #region 属性
        public SearchViewModel SearchVM
        {
            get => _searchVM;
            set => Set(ref _searchVM, value);
        }

        public MainViewModel MainVM
        {
            get => _mainVM;
            set => Set(ref _mainVM, value);
        }


        public ArtistInfo ArtistInfo
        {
            get => _artistInfo;
            set => Set(ref _artistInfo, value);
        }


        public ObservableCollection<MusicInfo> HotSongs
        {
            get => _hotSongs;
            set => Set(ref _hotSongs, value);
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

        #endregion

        #region 命令

        public RelayCommand<int> ChangeTabSelectionCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(ChangeTabSelection)).Value;

        public RelayCommand<int> NavigateToMvCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(NavigateToMv)).Value;

        private void NavigateToMv(int id)
        {
            _mainVM?.NavigateTo(typeof(MvPage), id);
        }

        public RelayCommand<int> NavigateToAlbumCmd => new Lazy<RelayCommand<int>>(() =>
            new RelayCommand<int>(NavigateToAlbum)).Value;

        private void NavigateToAlbum(int id)
        {
            _mainVM?.NavigateTo(typeof(AlbumPage), id);
        }
        #endregion

        public ArtistViewModel()
        {

        }

        public ArtistViewModel(int artistNo)
        {
            _artistNo = artistNo;
            MainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
            SearchVM = SimpleIoc.Default.GetInstance<SearchViewModel>();
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();
            _downloadService = SimpleIoc.Default.GetInstance<DownloadService>();

            Task.Run(async () =>
            {
                var artistInfo_hotSongs = await _NeteaseCloudMusicService.GetArtistInfoAndHotSongs(artistNo);
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ArtistInfo.Artist = artistInfo_hotSongs.ArtistInfo.Artist;
                    ArtistInfo.PicUrl = artistInfo_hotSongs.ArtistInfo.PicUrl;
                    ArtistInfo.MusicSize = artistInfo_hotSongs.ArtistInfo.MusicSize;
                    ArtistInfo.AlbumSize = artistInfo_hotSongs.ArtistInfo.AlbumSize;
                    ArtistInfo.MvSize = artistInfo_hotSongs.ArtistInfo.MvSize;
                    artistInfo_hotSongs.HotSongs.ForEach(t => HotSongs.Add(t));
                });
            });
        }

        /// <summary>
        /// 切换tab
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeTabSelection(int index)
        {
            switch (index)
            {
                case 1:
                    if (_albums.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var albums = await _NeteaseCloudMusicService.GetArtistAlbumAsync(_artistNo);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => albums.ForEach(t => Albums.Add(t)));
                    });
                    break;
                case 2:
                    if (_mvs.Count != 0) return;
                    Task.Run(async () =>
                    {
                        var mvs = await _NeteaseCloudMusicService.GetArtistMvAsync(_artistNo);
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => mvs.ForEach(t => Mvs.Add(t)));
                    });
                    break;
            }
        }
    }
}
