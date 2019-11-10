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
        private PlaylistInfo _playlist = new PlaylistInfo();
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private MainViewModel _mainVM;

        private ObservableCollection<MusicInfo> _musics = new ObservableCollection<MusicInfo>();


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

        public RelayCommand<object> ChangeSelectMusicCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(ChangeSelectMusic)).Value;

        public RelayCommand<object> AddToPlaylistCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(AddToPlaylist)).Value;


        public PlayListViewModel()
        {

        }

        public PlayListViewModel(long playlistId)
        {
            _mainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();

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
    }
}
