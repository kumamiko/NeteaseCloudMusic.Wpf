using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class RadioViewModel : ViewModelBase
    {
        #region 字段
        private Radio _radio = new Radio();
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
        public Radio Radio
        {
            get => _radio;
            set => Set(ref _radio, value);
        }

        public ObservableCollection<MusicInfo> Musics
        {
            get => _musics;
            set => Set(ref _musics, value);
        }
        #endregion

        #region 命令
        public RelayCommand GetMoreProgramCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(GetMoreProgram)).Value;

        private void GetMoreProgram()
        {
            var id = _radio.Id;
            var offset = _musics.Count;
            Task.Run(async () =>
            {
                var radio_songs = await _NeteaseCloudMusicService.GetRadioAndSongs(id, offset);
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    radio_songs.Songs.ForEach(t => Musics.Add(t));
                });
            });
        } 
        #endregion

        public RadioViewModel()
        {

        }

        public RadioViewModel(long radioId)
        {
            _mainVM = SimpleIoc.Default.GetInstance<MainViewModel>();
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();
            SearchVM = SimpleIoc.Default.GetInstance<SearchViewModel>();

            Task.Run(async () =>
            {
                var radio_songs = await _NeteaseCloudMusicService.GetRadioAndSongs(radioId);
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Radio = radio_songs.Radio;
                    radio_songs.Songs.ForEach(t => Musics.Add(t));
                });
            });
        }
    }
}
