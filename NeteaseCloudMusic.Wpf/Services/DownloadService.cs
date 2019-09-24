using NeteaseCloudMusic.Wpf.Helper;
using NeteaseCloudMusic.Wpf.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.Services
{
    public class DownloadService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private int _concurrentDownloadCount;
        private int _maxConcurrentDownloadCount = 3;
        private NeteaseCloudMusicService _NeteaseCloudMusicService;

        public int MaxConcurrentDownloadCount => _maxConcurrentDownloadCount;

        //public DownloadService(int MaxConcurrentDownloadCount = 3)
        //{
        //    _maxConcurrentDownloadCount = MaxConcurrentDownloadCount;
        //}

        public DownloadService(NeteaseCloudMusicService NeteaseCloudMusicService)
        {
            _NeteaseCloudMusicService = NeteaseCloudMusicService;
        }

        private async Task EnsureThrottlingAsync(CancellationToken cancellationToken)
        {
            // Gain lock
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                // Spin-wait until other downloads finish so that the number of concurrent downloads doesn't exceed the maximum
                while (_concurrentDownloadCount >= _maxConcurrentDownloadCount)
                    await Task.Delay(350, cancellationToken);

                // Increment concurrent download count
                Interlocked.Increment(ref _concurrentDownloadCount);
            }
            finally
            {
                // Release the lock
                _semaphore.Release();
            }
        }

        public async Task DownloadAsync(WebClient client, DownloadProgress downloadProgress, CancellationToken cancellationToken)
        {
            // Ensure throttling and increment concurrent download count
            await EnsureThrottlingAsync(cancellationToken);

            try
            {
                client.DownloadFileCompleted += (s, e) => DownloadFileCompleted(downloadProgress, s, e);
                client.DownloadProgressChanged += (_, e) => DownloadProgressChanged(downloadProgress, e.BytesReceived, e.TotalBytesToReceive);

                downloadProgress.Report(new DownloadInfo
                {
                    IsCompleted = true,
                    RecievedBytes = 0,
                    TotalBytes = 100,
                    Speed = 0,
                    Result = "正在下载"
                });

                await client.DownloadFileTaskAsync(new Uri(downloadProgress.Url), downloadProgress.FilePath);
            }
            //catch (Exception ex)
            //{
            //    Console.WriteLine();
            //}
            finally
            {
                // Decrement concurrent download count
                Interlocked.Decrement(ref _concurrentDownloadCount);
            }
        }

        private void DownloadFileCompleted(DownloadProgress downloadProgress, object s, AsyncCompletedEventArgs e)
        {
            (s as WebClient).DownloadFileCompleted -= (sender, ee) => DownloadFileCompleted(downloadProgress, sender, ee);
            (s as WebClient).DownloadProgressChanged -= (_, ee) => DownloadProgressChanged(downloadProgress, ee.BytesReceived, ee.TotalBytesToReceive);

            if (e.Cancelled)
            {
                downloadProgress.Report(new DownloadInfo
                {
                    IsCompleted = true,
                    RecievedBytes = downloadProgress.Info.RecievedBytes,
                    TotalBytes = downloadProgress.Info.TotalBytes,
                    Speed = 0,
                    Result = "取消"
                });
                return;
            }

            if (e.Error != null)
            {
                downloadProgress.Report(new DownloadInfo
                {
                    IsCompleted = true,
                    RecievedBytes = downloadProgress.Info.RecievedBytes,
                    TotalBytes = downloadProgress.Info.TotalBytes,
                    Speed = 0,
                    Result = $"错误:{e.Error.Message}"
                });
                return;
            }

            Task.Run(async () =>
            {
                if (!File.Exists(downloadProgress.FilePath)) return;
                var coverAndDetail = await _NeteaseCloudMusicService.GetCoverAndDetailAsync(downloadProgress.No);

                try
                {
                    TagLib.File file = TagLib.File.Create(downloadProgress.FilePath);
                    TagLib.Id3v2.AttachedPictureFrame cover = new TagLib.Id3v2.AttachedPictureFrame
                    {
                        Type = TagLib.PictureType.FrontCover,
                        Description = "Cover",
                        MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                        Data = coverAndDetail.cover.ToBytes(),
                        TextEncoding = TagLib.StringType.UTF16
                    };

                    file.Tag.Pictures = new TagLib.IPicture[] { cover };
                    file.Tag.Performers = coverAndDetail.detail.songs[0].artists.Select(t => t.name).ToArray();
                    file.Tag.Title = coverAndDetail.detail.songs[0].name;
                    file.Tag.Album = coverAndDetail.detail.songs[0].album.name;

                    file.Save();
                }
                catch { }
            });

            downloadProgress.Report(new DownloadInfo
            {
                IsCompleted = true,
                RecievedBytes = downloadProgress.Info.RecievedBytes,
                TotalBytes = downloadProgress.Info.TotalBytes,
                Speed = 0,
                Result = "完成"
            });
        }

        private void DownloadProgressChanged(DownloadProgress downloadProgress, long bytesReceived, long totalBytesToReceive)
        {
            if (downloadProgress.StartedAt == default(DateTime))
            {
                downloadProgress.StartedAt = DateTime.Now;
            }
            else
            {
                var timeSpan = DateTime.Now - downloadProgress.StartedAt;
                if (timeSpan.TotalSeconds > 0)
                {
                    downloadProgress.Info.Speed = bytesReceived / timeSpan.TotalMilliseconds;
                    downloadProgress.Info.TotalBytes = totalBytesToReceive;
                    downloadProgress.Info.RecievedBytes = bytesReceived;
                }
            }

            downloadProgress.Report(new DownloadInfo
            {
                IsCompleted = false,
                RecievedBytes = downloadProgress.Info.RecievedBytes,
                TotalBytes = downloadProgress.Info.TotalBytes,
                Speed = downloadProgress.Info.Speed,
            });
        }
    }
}
