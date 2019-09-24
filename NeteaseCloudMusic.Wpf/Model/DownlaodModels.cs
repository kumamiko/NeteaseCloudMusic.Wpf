using GalaSoft.MvvmLight;
using NeteaseCloudMusic.Wpf.Services;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.Model
{
    public class DownloadItemViewModel : ObservableObject
    {
        #region 字段
        private CancellationTokenSource _cancellationTokenSource;
        private WebClient _client;
        private DownloadService _downloadService;
        private string _filePath;
        private string _url;
        private int _no;
        private long _receivedBytes = 0;
        private long _totalBytes = 100;
        private double _speed;
        private string _result;
        private DownloadProgress _downloadProgress;
        private bool _isCompleted = true;
        #endregion

        #region 属性
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { this.Set(ref _isCompleted, value); }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { this.Set(ref _filePath, value); }
        }

        public int No
        {
            get { return _no; }
            set { this.Set(ref _no, value); }
        }

        public string Url
        {
            get { return _url; }
            set { this.Set(ref _url, value); }
        }

        public long ReceivedBytes
        {
            get { return _receivedBytes; }
            set { this.Set(ref _receivedBytes, value); }
        }
        public long TotalBytes
        {
            get { return _totalBytes; }
            set { this.Set(ref _totalBytes, value); }
        }
        public double Speed
        {
            get { return _speed; }
            set { this.Set(ref _speed, value); }
        }
        public string Result
        {
            get { return _result; }
            set { this.Set(ref _result, value); }
        }
        #endregion

        public DownloadItemViewModel(DownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        public void Start()
        {
            Result = "等待";
            IsCompleted = false;
            _client = new WebClient();
            _cancellationTokenSource = new CancellationTokenSource();
            _downloadProgress = new DownloadProgress
            {
                FilePath = _filePath,
                Url = _url,
                No = _no
            };
            _downloadProgress.DownloadReport += GetReport;
            Task.Run(async () =>
            {
                await _downloadService.DownloadAsync(_client, _downloadProgress, _cancellationTokenSource.Token);
                _downloadProgress.DownloadReport -= GetReport;
            });
        }

        private void GetReport(DownloadInfo obj)
        {
            this.ReceivedBytes = obj.RecievedBytes;
            this.TotalBytes = obj.TotalBytes;
            this.Speed = obj.Speed;
            if (obj.IsCompleted == true)
            {
                Result = obj.Result;
                IsCompleted = true;
            }
        }

        public void Cancel()
        {
            _client?.CancelAsync();
            _cancellationTokenSource?.Cancel();
        }
    }

    public class DownloadProgress : IProgress<DownloadInfo>
    {
        public DownloadInfo Info;
        public DateTime StartedAt { get; set; }
        public string FilePath { get; set; }
        public string Url { get; set; }
        public int No { get; set; }

        public event Action<DownloadInfo> DownloadReport;
        public void Report(DownloadInfo value)
        {
            DownloadReport?.Invoke(value);
        }
    }

    public struct DownloadInfo
    {
        public long RecievedBytes;
        public long TotalBytes;
        public double Speed;
        public string Result;
        public bool IsCompleted;
    }
}
