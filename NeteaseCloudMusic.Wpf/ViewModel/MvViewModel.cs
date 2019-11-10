using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Config;
using NeteaseCloudMusic.Wpf.Helper;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class MvViewModel : ViewModelBase
    {
        private int _mvNo;
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private MvInfo _mv = new MvInfo();
        private static DownloadViewModel _downloadVM;

        public MvInfo Mv
        {
            get => _mv;
            set => Set(ref _mv, value);
        }

        public RelayCommand<object> AddToDownloadCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(AddToDownload)).Value;

        private void AddToDownload(object obj)
        {
            if (_downloadVM == null || string.IsNullOrEmpty(obj.ToString())) return;
            _downloadVM?.AddAndStart($"{Path.Combine(GlobalData.Config.SaveFolder, _mv.Name.RemoveInvalidFileNameChars())}.mp4", _mv.Url, 0, 1);
            MessengerInstance.Send<string>("已加入下载列表", "ShowInfo");
        }

        public MvViewModel()
        {

        }

        public MvViewModel(int mvNo)
        {
            _mvNo = mvNo;
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();
            if(_downloadVM == null) _downloadVM = SimpleIoc.Default.GetInstance<DownloadViewModel>();

            Task.Run(async () =>
            {
                var mvInfo = await _NeteaseCloudMusicService.GetMvAsync(mvNo);
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Mv.Url = mvInfo.Url;
                    Mv.Name = mvInfo.Name;
                    Mv.Id = mvInfo.Id;
                    Mv.ArtistName = mvInfo.ArtistName;
                    Mv.PlayCount = mvInfo.PlayCount;
                    Mv.Duration = mvInfo.Duration;
                });
            });
        }
    }
}
