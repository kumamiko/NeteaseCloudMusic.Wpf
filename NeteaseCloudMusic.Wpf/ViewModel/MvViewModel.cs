using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class MvViewModel : ViewModelBase
    {
        private int _mvNo;
        private NeteaseCloudMusicService _NeteaseCloudMusicService;
        private MvInfo _mv = new MvInfo();

        public MvInfo Mv
        {
            get => _mv;
            set => Set(ref _mv, value);
        }


        public MvViewModel()
        {

        }

        public MvViewModel(int mvNo)
        {
            _mvNo = mvNo;
            _NeteaseCloudMusicService = SimpleIoc.Default.GetInstance<NeteaseCloudMusicService>();

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
