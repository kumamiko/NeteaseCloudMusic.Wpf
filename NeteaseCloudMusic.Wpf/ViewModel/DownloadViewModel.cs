using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class DownloadViewModel : ViewModelBase
    {
        private DownloadService _downloadService;

        public ObservableCollection<DownloadItemViewModel> Downloads { get; } = new ObservableCollection<DownloadItemViewModel>();

        #region 命令
        public RelayCommand<object> CancelCmd => new Lazy<RelayCommand<object>>(() =>
             new RelayCommand<object>(CancelDownload)).Value;

        private void CancelDownload(object obj)
        {
            if (obj is DownloadItemViewModel item)
            {
                item.Cancel();
            }
        }

        public RelayCommand<object> DeleteCmd => new Lazy<RelayCommand<object>>(() =>
            new RelayCommand<object>(DeleteDownload)).Value;

        private void DeleteDownload(object obj)
        {
            if (obj is DownloadItemViewModel item)
            {
                Downloads.Remove(item);
            }
        }

        public RelayCommand<object> StartCmd => new Lazy<RelayCommand<object>>(() =>
             new RelayCommand<object>(StartDownload)).Value;

        private void StartDownload(object obj)
        {
            if (obj is DownloadItemViewModel item)
            {
                item.Start();
            }
        }

        public RelayCommand<object> OpenCmd => new Lazy<RelayCommand<object>>(() =>
             new RelayCommand<object>(Open)).Value;

        #endregion



        public DownloadViewModel(DownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        private void Open(object obj)
        {
            if (obj is DownloadItemViewModel item)
            {
                if (!File.Exists(item.FilePath))
                    return;

                try
                {
                    Process.Start("explorer", $"/select, \"{item.FilePath}\"");
                }
                catch (Exception ex)
                {
                    //Todo 写日志
                }
            }
        }

        public void Add(string filepath, string url, int no)
        {
            Downloads.Add(new DownloadItemViewModel(_downloadService)
            {
                FilePath = filepath,
                Url = url,
                No = no
            });
        }

        public void AddAndStart(string filepath, string url, int no)
        {
            var item = new DownloadItemViewModel(_downloadService)
            {
                FilePath = filepath,
                Url = url,
                No = no
            };

            Downloads.Add(item);

            item.Start();
        }
    }
}
