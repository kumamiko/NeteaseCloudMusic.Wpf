using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class AlbumViewModel : ViewModelBase
    {
        #region 字段
        private AlbumInfo _album = new AlbumInfo();
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private MainViewModel _mainVM;
        private SearchViewModel _searchVM;
        private ObservableCollection<MusicInfo> _musics = new ObservableCollection<MusicInfo>();
        #endregion

        #region 属性
        public SearchViewModel SearchVM
        {
            get => _searchVM;
            set => Set(ref _searchVM, value);
        }
        public AlbumInfo Album
        {
            get => _album;
            set => Set(ref _album, value);
        }

        public ObservableCollection<MusicInfo> Musics
        {
            get => _musics;
            set => Set(ref _musics, value);
        }
        #endregion

        #region 命令
        #endregion

        public AlbumViewModel()
        {

        }

        public AlbumViewModel(int albumId)
        {
            _mainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();
            SearchVM = SimpleIoc.Default.GetInstance<SearchViewModel>();

            Task.Run(async () =>
            {
                var albumInfo_songs = await _NeteaseCloudMusicService.GetAlbumInfoAndSongs(albumId);
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Album = albumInfo_songs.AlbumInfo;
                    albumInfo_songs.Songs.ForEach(t => Musics.Add(t));
                });
            });
        }
    }
}
