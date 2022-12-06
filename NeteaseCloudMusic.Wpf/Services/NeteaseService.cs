﻿using Flurl;
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
        Lazy<CloudMusicApi> api = new Lazy<CloudMusicApi>(() => new CloudMusicApi());


        /// <summary>
        /// 网易搜索Api
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <param name="type">type搜索的类型：1歌曲 10专辑 100歌手 1000歌单 1002用户 1004MV 1006歌词 1009主播电台 </param>
        /// <returns></returns>
        public async Task<string> SearchAsync(string keyword, int offset = 0, int type = 1)
        {
            return await $"http://music.163.com/api/search/get".WithTimeout(3)
                                    .PostUrlEncodedAsync(new { s = keyword, offset = offset, limit = 20, type = type })
                                    .ReceiveString();
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
                            ArtistIds = t.artists[0].id.ToString(),
                            Duration = $"{duration.TotalMinutes:00}:{duration.Seconds:00}",
                            File = @"http://music.163.com/song/media/outer/url?id=" + t.id + ".mp3",
                            Album = t.album.name,
                            AlbumId = t.album.id,
                            MvId = t.mvid,
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
            try
            {
                bool isOk;
                JObject json;
                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.Search, new Dictionary<string, object> { ["keywords"] = keyword.ToString(), ["offset"] = offset.ToString() });
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
                            MvId = (int)t["mvid"],
                            Type = 1/*网页音乐*/
                        });
                }
                Console.WriteLine();
            }
            catch { }

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
                            TrackCount = t.size,
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
        public async Task<List<PlaylistInfo>> SearchPlayListAsync(string keyword, int offset = 0)
        {
            List<PlaylistInfo> playlists = new List<PlaylistInfo>();

            try
            {
                var jsonstring = await SearchAsync(keyword, offset, 1000);
                var root = JsonConvert.DeserializeObject<SearchPlayList.PlaylistList>(jsonstring);

                var playlistsTemp = root.result.playlists;

                if (playlistsTemp == null) return playlists;

                foreach (var t in playlistsTemp)
                {
                    playlists.Add(
                        new PlaylistInfo
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
        /// 搜radio
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<List<Radio>> SearchRadioAsync(string keyword, int offset = 0)
        {
            List<Radio> radios = new List<Radio>();

            try
            {
                (bool isOK, JObject json) = await api.Value.RequestAsync(CloudMusicApiProviders.Search, new Dictionary<string, object> { ["keywords"] = keyword, ["type"] = 1009, ["offset"] = offset });
                if (!isOK)
                    throw new ApplicationException($"获取主播广播信息失败： {json}");

                foreach (var t in json["result"]["djRadios"].ToArray())
                {
                    radios.Add(new Radio
                    {
                        Id = (long)t["id"],
                        Name = t["name"].ToString(),
                        Cover = t["picUrl"].ToString(),
                        Nickname = t["dj"]["nickname"].ToString(),
                        PlayCount = (int)t["playCount"],
                        ProgramCount = (int)t["programCount"]
                    });
                }
                return radios;
            }
            catch { }

            return radios;
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
            try
            {
                bool isOk;
                JObject json;
                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.ArtistMv, new Dictionary<string, object> { ["id"] = artistNo.ToString() });
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
                artistInfo.PicUrl = artistDetail.artist.img1v1Url;
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
                        MvId = song.mvid,
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
            try
            {
                bool isOk;
                JObject json;
                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.Album, new Dictionary<string, object> { ["id"] = albumId.ToString() });
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

            return (albumInfo, songs);
        }

        public async Task<MvInfo> GetMvAsync(int mvNo)
        {
            MvInfo mvInfo = new MvInfo();
            try
            {
                bool isOk;
                JObject json;
                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.MvDetail, new Dictionary<string, object> { ["mvid"] = mvNo.ToString() });
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
            catch { }

            return mvInfo;
        }

        public async Task<(BitmapImage cover, SearchSongDetail.SongDetail detail)> GetCoverAndDetailAsync(int id)
        {
            try
            {
                //type搜索的类型：1歌曲 10专辑 100歌手 1000歌单 1002用户 1006歌词 1009主播电台 
                var jsonstring = await $"http://music.163.com/api/song/detail/?id={id}&ids=%5B{id}%5D".WithTimeout(3).GetStringAsync();

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

        public async Task<(PlaylistInfo PlaylistInfo, List<MusicInfo> Songs)> GetPlaylistInfoAndSongs(long playlistId)
        {
            PlaylistInfo playlistInfo = new PlaylistInfo();
            List<MusicInfo> songs = new List<MusicInfo>();
            try
            {
                bool isOk;
                JObject json;
                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.PlaylistDetail, new Dictionary<string, object> { ["id"] = playlistId.ToString() });
                if (!isOk)
                    throw new ApplicationException($"获取专辑信息失败： {json}");

                //var a = json.ToString();

                var playlistInfoTemp = json["playlist"];

                playlistInfo.Id = (long)playlistInfoTemp["id"];
                playlistInfo.Name = playlistInfoTemp["name"].ToString();
                playlistInfo.Cover = playlistInfoTemp["coverImgUrl"].ToString();
                playlistInfo.Nickname = playlistInfoTemp["creator"]["nickname"].ToString();
                playlistInfo.PlayCount = (int)playlistInfoTemp["playCount"];
                playlistInfo.Description = playlistInfoTemp["description"].ToString();
                playlistInfo.CreateTime = DateTimeHelper.ConvertTimeStampToDateTime((long)playlistInfoTemp["createTime"]).ToString("yyyy-MM-dd");

                int[] trackIds = json["playlist"]["trackIds"].Select(t => (int)t["id"]).ToArray();

                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.SongDetail, new Dictionary<string, object> { ["ids"] = trackIds });

                if (!isOk)
                    throw new ApplicationException($"获取歌曲详情失败： {json}");

                foreach (JObject t in json["songs"])
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

            return (playlistInfo, songs);
        }

        public async Task<(Radio Radio, List<MusicInfo> Songs)> GetRadioAndSongs(long radioId, int offset = 0)
        {
            Radio radio = new Radio();
            List<MusicInfo> songs = new List<MusicInfo>();
            try
            {
                bool isOk;
                JObject json;

                if (offset == 0)
                {
                    (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.DjDetail, new Dictionary<string, object> { ["rid"] = radioId.ToString() });
                    if (!isOk)
                        throw new ApplicationException($"获取广播信息失败： {json}");

                    //var a = json.ToString();

                    var radioTemp = json["djRadio"];

                    radio.Id = (long)radioTemp["id"];
                    radio.Name = radioTemp["name"].ToString();
                    radio.Cover = radioTemp["picUrl"].ToString();
                    radio.Nickname = radioTemp["dj"]["nickname"].ToString();
                    radio.PlayCount = (int)radioTemp["playCount"];
                    radio.Description = radioTemp["desc"].ToString();
                    radio.CreateTime = DateTimeHelper.ConvertTimeStampToDateTime((long)radioTemp["createTime"]).ToString("yyyy-MM-dd");
                }

                (isOk, json) = await api.Value.RequestAsync(CloudMusicApiProviders.DjProgram, new Dictionary<string, object> { ["rid"] = radioId, ["offset"] = offset });

                if (!isOk)
                    throw new ApplicationException($"获取节目失败： {json}");


                var a = json.ToString();
                foreach (JObject t in json["programs"])
                {
                    var s = t["mainSong"];
                    var artistsTemp = s["artists"].Select(p => (string)p["name"]).ToArray();

                    songs.Add(
                        new MusicInfo
                        {
                            Id = (int)(s["id"]),
                            ArtistIds = s["artists"].Select(p => ((int)p["id"]).ToString()).First(),
                            Name = s["name"]?.ToString() ?? string.Empty,
                            Album = s["album"]["name"]?.ToString() ?? string.Empty,
                            Artist = string.Join("/", artistsTemp),
                            ListenerCount = (int)t["listenerCount"],
                            SerialNum = (int)t["serialNum"],
                            CreateTime = DateTimeHelper.ConvertTimeStampToDateTime((long)t["createTime"]).ToString("yyyy-MM-dd"),
                            File = @"http://music.163.com/song/media/outer/url?id=" + s["id"] + ".mp3",
                            Type = 1/*网页音乐*/
                        });
                }
                Console.WriteLine();
            }
            catch { }

            return (radio, songs);
        }

    }
}
