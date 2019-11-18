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
    public class PlayListViewModel : ViewModelBase
    {
        #region 字段
        private PlaylistInfo _playlist = new PlaylistInfo();
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
        public PlaylistInfo Playlist
        {
            get => _playlist;
            set => Set(ref _playlist, value);
        }

        public ObservableCollection<MusicInfo> Musics
        {
            get => _musics;
            set => Set(ref _musics, value);
        } 
        #endregion

        public PlayListViewModel()
        {

        }

        public PlayListViewModel(long playlistId)
        {
            _mainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();
            SearchVM = SimpleIoc.Default.GetInstance<SearchViewModel>();

            Task.Run(async () =>
            {
                var playlistInfo_songs = await _NeteaseCloudMusicService.GetPlaylistInfoAndSongs(playlistId);
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Playlist = playlistInfo_songs.PlaylistInfo;
                    playlistInfo_songs.Songs.ForEach(t => Musics.Add(t));
                });
            });
        }
    }
}
