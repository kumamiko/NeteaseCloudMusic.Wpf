using Flurl;
using Flurl.Http;
using NeteaseCloudMusic.Wpf.Helper;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusicApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NeteaseCloudMusic.Wpf.Services
{
    public class NeteaseCloudMusicService
    {

        /// <summary>
        /// 网易搜索Api
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <param name="type">type搜索的类型：1歌曲 10专辑 100歌手 1000歌单 1002用户 1004MV 1006歌词 1009主播电台 </param>
        /// <returns></returns>
        public async Task<string> SearchAsync(string keyword, int offset = 0, int type = 1)
        {
            var response = await $"http://music.163.com/api/search/get".WithTimeout(3)
                                    .PostUrlEncodedAsync(new { s = keyword, offset = offset, limit = 20, type = type });

            return await response.Content?.ReadAsStringAsync() ?? string.Empty;
        }

        /// <summary>
        /// 搜音乐
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<MusicInfo>> SearchMusicAsync(string keyword, int offset = 0)
        {
            List<MusicInfo> musics = new List<MusicInfo>();
            try
            {
                var jsonstring = await SearchAsync(keyword, offset, 1);

                if (string.IsNullOrEmpty(jsonstring)) return musics;

                var root = JsonConvert.DeserializeObject<SearchSongList.SongList>(jsonstring ?? string.Empty);

                var songs = root.result.songs;

                foreach (var t in songs)
                {
                    var duration = TimeSpan.FromMilliseconds(t.duration);

                    musics.Add(
                        new MusicInfo
                        {
                            Id = t.id,
                            Name = t.name,
                            Artist = string.Join("/", t.artists.ToList().Select(p => p.name)),
                            Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}",
                            File = @"http://music.163.com/song/media/outer/url?id=" + t.id + ".mp3",
                            Album = t.album.name,
                            Type = 1/*网页音乐*/
                        }); ;
                }
            }
            catch { }

            return musics;
        }

        /// <summary>
        /// 用api搜音乐
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<MusicInfo>> SearchMusicAsync2(string keyword, int offset = 0)
        {
            List<MusicInfo> songs = new List<MusicInfo>();
            using (CloudMusicApi api = new CloudMusicApi())
            {
                try
                {
                    bool isOk;
                    JObject json;
                    (isOk, json) = await api.RequestAsync(CloudMusicApiProviders.Search, new Dictionary<string, string> { { "keywords", keyword.ToString() }, { "offset", offset.ToString() } });
                    if (!isOk)
                        throw new ApplicationException($"获取专辑信息失败： {json}");

                    //var a = json.ToString();

                    var songsTemp = json["result"]["songs"].ToArray();

                    songs = new List<MusicInfo>();
                    foreach (var t in songsTemp)
                    {
                        //var b = t.ToString();

                        var artistsTemp = t["artists"].Select(p => (string)p["name"]).ToArray();
                        var artistsIdsTemp = t["artists"].Select(p => ((int)p["id"]).ToString()).ToArray();
                        var duration = TimeSpan.FromMilliseconds((int)t["duration"]);

                        songs.Add(
                            new MusicInfo
                            {
                                Id = (int)t["id"],
                                Name = t["name"].ToString(),
                                Album = t["album"]?["name"].ToString(),
                                AlbumId = (int)t["album"]["id"],
                                Artist = string.Join("/", artistsTemp),
                                ArtistIds = artistsIdsTemp[0],/*先做成只能搜索单个歌手*/
                                Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}",
                                File = @"http://music.163.com/song/media/outer/url?id=" + t["id"].ToString() + ".mp3",
                                Type = 1/*网页音乐*/
                            });
                    }
                    Console.WriteLine();
                }
                catch { }
            }

            return songs;
        }

        /// <summary>
        /// 搜专辑
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<AlbumInfo>> SearchAlbumAsync(string keyword, int offset = 0)
        {
            List<AlbumInfo> albums = new List<AlbumInfo>();
            try
            {
                var jsonstring = await SearchAsync(keyword, offset, 10);

                if (string.IsNullOrEmpty(jsonstring)) return albums;

                var root = JsonConvert.DeserializeObject<SearchAlbumList.AlbumList>(jsonstring);

                if (root.result.albums == null) return albums;

                foreach (var t in root.result.albums)
                {
                    List<string> artists = new List<string>();
                    if (t.artists != null)
                    {
                        foreach (var p in t.artists)
                        {
                            artists.Add(p.name);
                        }
                    }

                    albums.Add(
                        new AlbumInfo
                        {
                            Id = t.id,
                            Name = t.name,
                            PicUrl = t.picUrl != null ? t.picUrl : "pack://application:,,,/Resources/Image/default_album.png",
                            Artist = t.artists != null ? string.Join("/", artists) : t.artist.name
                        }); ;
                }
            }
            catch { }

            return albums;
        }

        /// <summary>
        /// 搜歌手
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<ArtistInfo>> SearchArtistAsync(string keyword, int offset = 0)
        {
            List<ArtistInfo> artists = new List<ArtistInfo>();
            try
            {
                var jsonstring = await SearchAsync(keyword, offset, 100);

                var root = JsonConvert.DeserializeObject<dynamic>(jsonstring ?? string.Empty);

                var artistsTemp = root.result.artists;

                if (artistsTemp == null) return artists;

                foreach (var t in artistsTemp)
                {
                    artists.Add(
                        new ArtistInfo
                        {
                            Id = ((int)t.id).ToString(),
                            Artist = t.name,
                            PicUrl = t.picUrl != null ? t.picUrl
                                                      : (t.img1v1Url != null ? t.img1v1Url : string.Empty),
                            Trans = t.trans != null ? t.trans : (t.alias is string[] alias && alias.Length > 0 ? alias[0] : string.Empty)
                        });
                }

            }
            catch { }


            return artists;
        }

        /// <summary>
        /// 搜MV
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<MvInfo>> SearchMvAsync(string keyword, int offset = 0)
        {
            List<MvInfo> mvs = new List<MvInfo>();
            try
            {
                var jsonstring = await SearchAsync(keyword, offset, 1004);
                var root = JsonConvert.DeserializeObject<dynamic>(jsonstring ?? string.Empty);

                var mvsTemp = root.result.mvs;

                if (mvsTemp == null) return mvs;

                foreach (var t in mvsTemp)
                {
                    var duration = TimeSpan.FromMilliseconds((int)t.duration);

                    mvs.Add(
                        new MvInfo
                        {
                            Id = t.id,
                            Cover = t.cover,
                            PlayCount = (int)t.playCount,
                            ArtistName = t.artistName,
                            Name = t.name,
                            Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}"
                        });
                }

            }
            catch { }


            return mvs;
        }

        /// <summary>
        /// 搜歌单
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<Playlist>> SearchPlayListAsync(string keyword, int offset = 0)
        {
            List<Playlist> playlists = new List<Playlist>();

            try
            {
                var jsonstring = await SearchAsync(keyword, offset, 1000);
                var root = JsonConvert.DeserializeObject<SearchPlayList.PlaylistList>(jsonstring);

                var playlistsTemp = root.result.playlists;

                if (playlistsTemp == null) return playlists;

                foreach (var t in playlistsTemp)
                {
                    playlists.Add(
                        new Playlist
                        {
                            Id = t.id,
                            Name = t.name,
                            Cover = t.coverImgUrl,
                            UserId = t.creator.userId,
                            Nickname = t.creator.nickname,
                            TrackCount = t.trackCount,
                            BookCount = t.bookCount,
                            Description = t.description,
                            PlayCount = t.playCount
                        });
                }

            }
            catch { }

            return playlists;
        }


        /// <summary>
        /// 搜某个歌手的专辑
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<AlbumInfo>> GetArtistAlbumAsync(int artistNo, int offset = 0)
        {
            List<AlbumInfo> albums = new List<AlbumInfo>();
            try
            {
                var artistAlbums = await $"http://music.163.com/api/artist/albums/{artistNo}".SetQueryParams(new
                {
                    offset = offset,
                    limit = 30
                }).WithTimeout(3)
                .GetJsonAsync<GetArtistAlbums.ArtistAlbums>();

                foreach (var t in artistAlbums.hotAlbums)
                {
                    albums.Add(
                        new AlbumInfo
                        {
                            Id = t.id,
                            Name = t.name,
                            PicUrl = t.picUrl != null ? t.picUrl : "pack://application:,,,/Resources/Image/default_album.png",
                            PublishTime = DateTimeHelper.ConvertTimeStampToDateTime((long)t.publishTime).ToString("yyyy-MM-dd")
                        });
                }
            }
            catch { }

            return albums;
        }

        /// <summary>
        /// 搜某个歌手的MV
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<MvInfo>> GetArtistMvAsync(int artistNo, int offset = 0)
        {
            List<MvInfo> mvs = new List<MvInfo>();
            using (CloudMusicApi api = new CloudMusicApi())
            {
                try
                {
                    bool isOk;
                    JObject json;
                    (isOk, json) = await api.RequestAsync(CloudMusicApiProviders.ArtistMv, new Dictionary<string, string> { { "id", artistNo.ToString() } });
                    if (!isOk)
                        throw new ApplicationException($"获取歌手mv信息失败： {json}");

                    var mvsTemp = json["mvs"].ToArray();

                    foreach (var t in mvsTemp)
                    {
                        var duration = TimeSpan.FromMilliseconds((int)t["duration"]);

                        mvs.Add(
                            new MvInfo
                            {
                                Id = (int)t["id"],
                                Name = t["name"].ToString(),
                                Cover = t["imgurl"].ToString(),
                                PlayCount = (int)t["playCount"],
                                ArtistName = t["artistName"].ToString(),
                                Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}"
                            });
                    }
                }
                catch { }
            }

            return mvs;
        }

        /// <summary>
        /// 歌手基本信息和热门
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<(ArtistInfo ArtistInfo, List<MusicInfo> HotSongs)> GetArtistInfoAndHotSongs(int artistNo)
        {
            ArtistInfo artistInfo = new ArtistInfo();
            List<MusicInfo> hotSongs = new List<MusicInfo>(); ;
            try
            {
                var artistDetail = await $"http://music.163.com/api/artist/{artistNo}".WithTimeout(3).GetJsonAsync<GetArtistDetail.ArtistDetail>();

                artistInfo.Artist = artistDetail.artist.name;
                artistInfo.PicUrl = artistDetail.artist.picUrl;
                artistInfo.MusicSize = artistDetail.artist.musicSize;
                artistInfo.AlbumSize = artistDetail.artist.albumSize;
                artistInfo.MvSize = artistDetail.artist.mvSize;

                foreach (var song in artistDetail.hotSongs)
                {
                    var duration = TimeSpan.FromMilliseconds(song.duration);
                    hotSongs.Add(new MusicInfo
                    {
                        Id = song.id,
                        Name = song.name,
                        AlbumId = song.album.id,
                        ArtistIds = song.artists.Select(t => ((int)t.id).ToString()).First(),
                        Artist = string.Join("/", song.artists.Select(p => p.name)),
                        Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}",
                        File = @"http://music.163.com/song/media/outer/url?id=" + song.id + ".mp3",
                        Album = song.album.name,
                        Type = 1/*网页音乐*/
                    });
                }

            }
            catch { }

            return (artistInfo, hotSongs);
        }

        /// <summary>
        /// 专辑和歌曲
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<(AlbumInfo AlbumInfo, List<MusicInfo> Songs)> GetAlbumInfoAndSongs(int albumId)
        {
            AlbumInfo albumInfo = new AlbumInfo();
            List<MusicInfo> songs = new List<MusicInfo>();
            using (CloudMusicApi api = new CloudMusicApi())
            {
                try
                {
                    bool isOk;
                    JObject json;
                    (isOk, json) = await api.RequestAsync(CloudMusicApiProviders.Album, new Dictionary<string, string> { { "id", albumId.ToString() } });
                    if (!isOk)
                        throw new ApplicationException($"获取专辑信息失败： {json}");

                    //var a = json.ToString();

                    var albumInfoTemp = json["album"];
                    var artists = albumInfoTemp["artists"].Select(t => (string)t["name"]).ToArray();

                    albumInfo.Id = (int)albumInfoTemp["id"];
                    albumInfo.Name = albumInfoTemp["name"].ToString();
                    albumInfo.Artist = string.Join("/", artists);
                    albumInfo.PicUrl = albumInfoTemp["picUrl"].ToString();
                    albumInfo.PublishTime = DateTimeHelper.ConvertTimeStampToDateTime((long)albumInfoTemp["publishTime"]).ToString("yyyy-MM-dd");

                    var songsTemp = json["songs"].ToArray();

                    foreach (var t in songsTemp)
                    {
                        var artistsTemp = t["ar"].Select(p => (string)p["name"]).ToArray();

                        songs.Add(
                            new MusicInfo
                            {
                                Id = (int)t["id"],
                                ArtistIds = t["ar"].Select(p => ((int)p["id"]).ToString()).First(),
                                Name = t["name"]?.ToString() ?? string.Empty,
                                Album = t["al"]["name"]?.ToString() ?? string.Empty,
                                Artist = string.Join("/", artistsTemp),
                                File = @"http://music.163.com/song/media/outer/url?id=" + t["id"] + ".mp3",
                                Type = 1/*网页音乐*/
                            });
                    }
                    Console.WriteLine();
                }
                catch { }
            }

            return (albumInfo, songs);
        }

        public async Task<MvInfo> GetMvAsync(int mvNo)
        {
            MvInfo mvInfo = new MvInfo();
            using (CloudMusicApi api = new CloudMusicApi())
            {
                try
                {
                    bool isOk;
                    JObject json;
                    (isOk, json) = await api.RequestAsync(CloudMusicApiProviders.MvDetail, new Dictionary<string, string> { { "mvid", mvNo.ToString() } });
                    if (!isOk)
                        throw new ApplicationException($"获取mv详细信息失败： {json}");

                    string[] resolutions = { "240", "480", "720", "1080" };

                    int i = 3;
                    while (i-- > 0)
                    {
                        if (json["data"]["brs"][resolutions[i]] != null)
                        {
                            mvInfo.Url = json["data"]["brs"][resolutions[i]].ToString();
                            break;
                        }
                    }

                    var a = json.ToString();

                    var duration = TimeSpan.FromMilliseconds((int)json["data"]["duration"]);
                    mvInfo.Name = json["data"]["name"].ToString();
                    mvInfo.Id = (int)json["data"]["id"];
                    mvInfo.Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}";
                    mvInfo.ArtistName = json["data"]["artistName"].ToString();
                    mvInfo.PlayCount = (int)json["data"]["playCount"];
                }
                catch (Exception ex) { }
            }

            return mvInfo;
        }

        public async Task<(BitmapImage cover, SearchSongDetail.SongDetail detail)> GetCoverAndDetailAsync(int id)
        {
            try
            {
                //type搜索的类型：1歌曲 10专辑 100歌手 1000歌单 1002用户 1006歌词 1009主播电台 
                var response = await $"http://music.163.com/api/song/detail/?id={id}&ids=%5B{id}%5D".WithTimeout(3).GetAsync();

                var jsonstring = await response?.Content?.ReadAsStringAsync();

                var root = JsonConvert.DeserializeObject<SearchSongDetail.SongDetail>(jsonstring ?? string.Empty);

                var coverpath = root?.songs[0]?.album?.picUrl;

                if (coverpath == null) return (null, default(SearchSongDetail.SongDetail));

                Stream stream = await $"{coverpath}?param=500x500".GetStreamAsync();

                BitmapImage bitmap = null;
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return (bitmap, root);
            }
            catch
            {
                return (null, default(SearchSongDetail.SongDetail));
            }
        }

    }
}
