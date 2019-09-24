using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using NeteaseCloudMusic.Wpf.Services;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LocalMusicViewModel>();
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<DownloadViewModel>();
            SimpleIoc.Default.Register<DataService>();
            SimpleIoc.Default.Register<NeteaseCloudMusicService>();
            SimpleIoc.Default.Register<DownloadService>();
        }

        public LocalMusicViewModel LocalMusic => ServiceLocator.Current.GetInstance<LocalMusicViewModel>();
        public SearchViewModel Search => ServiceLocator.Current.GetInstance<SearchViewModel>();
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public DownloadViewModel Download => ServiceLocator.Current.GetInstance<DownloadViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}