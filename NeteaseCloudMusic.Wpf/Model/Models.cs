using GalaSoft.MvvmLight;
using LiteDB;
using System;

namespace NeteaseCloudMusic.Wpf.Model
{
    public class NavigationItem : ObservableObject
    {
        private int _id;
        private string _header;
        private Type _pageType;
        private string _icon;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string Header
        {
            get => _header;
            set => Set(ref _header, value);
        }

        public Type PageType
        {
            get => _pageType;
            set => Set(ref _pageType, value);
        }

        public string Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }
    }

    public class History
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Keyword { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class MusicInfo : ObservableObject
    {
        private int _id;
        private int _type;
        private string _name;
        private string _artist;
        private string _duration;
        private string _file;
        private string _album;
        private bool _playing;
        private string _artistIds;
        private int _albumId;

        [BsonId]
        public Guid Guid { get; set; }
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        /// <summary>
        /// type : 0 本地音乐， 1 网页音乐
        /// </summary>
        public int Type
        {
            get => _type;
            set => Set(ref _type, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Artist
        {
            get => _artist;
            set => Set(ref _artist, value);
        }

        public string ArtistIds
        {
            get => _artistIds;
            set => Set(ref _artistIds, value);
        }

        public string Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }
        public string File
        {
            get => _file;
            set => Set(ref _file, value);
        }

        public string Album
        {
            get => _album;
            set => Set(ref _album, value);
        }

        public int AlbumId
        {
            get => _albumId;
            set => Set(ref _albumId, value);
        }

        public bool Playing
        {
            get => _playing;
            set => Set(ref _playing, value);
        }
    }

    public class ArtistInfo : ObservableObject
    {
        private string _id;
        private string _artist;
        private string _picUrl;
        private string _trans;
        private int _musicSize;
        private int _albumSize;
        private int _mvSize;

        public string Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string Artist
        {
            get => _artist;
            set => Set(ref _artist, value);
        }

        public string PicUrl
        {
            get => _picUrl;
            set => Set(ref _picUrl, value);
        }

        public string Trans
        {
            get => _trans;
            set => Set(ref _trans, value);
        }

        public int MusicSize
        {
            get => _musicSize;
            set => Set(ref _musicSize, value);
        }

        public int AlbumSize
        {
            get => _albumSize;
            set => Set(ref _albumSize, value);
        }

        public int MvSize
        {
            get => _mvSize;
            set => Set(ref _mvSize, value);
        }
    }

    public class AlbumInfo : ObservableObject
    {
        private int _id;
        private string _picUrl;
        private string _name;
        private string _publishTime;
        private string _artist;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string PicUrl
        {
            get => _picUrl;
            set => Set(ref _picUrl, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string PublishTime
        {
            get => _publishTime;
            set => Set(ref _publishTime, value);
        }

        public string Artist
        {
            get => _artist;
            set => Set(ref _artist, value);
        }
    }

    public class MvInfo : ObservableObject
    {
        private int _id;
        private string _cover;
        private string _name;
        private string _duration;
        private string _artistName;
        private int _playCount;
        private string _url;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public int PlayCount
        {
            get => _playCount;
            set => Set(ref _playCount, value);
        }

        public string Cover
        {
            get => _cover;
            set => Set(ref _cover, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

        public string ArtistName
        {
            get => _artistName;
            set => Set(ref _artistName, value);
        }

        public string Url
        {
            get => _url;
            set => Set(ref _url, value);
        }
    }

    public class Playlist : ObservableObject
    {
        private long _id;
        private string _cover;
        private string _name;
        private string _nickname;
        private int _playCount;
        private int _trackCount;
        private int _bookCount;
        private int _userId;
        private string _description;

        public long Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string Cover
        {
            get => _cover;
            set => Set(ref _cover, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Nickname
        {
            get => _nickname;
            set => Set(ref _nickname, value);
        }

        public int PlayCount
        {
            get => _playCount;
            set => Set(ref _playCount, value);
        }

        public int TrackCount
        {
            get => _trackCount;
            set => Set(ref _trackCount, value);
        }

        public int BookCount
        {
            get => _bookCount;
            set => Set(ref _bookCount, value);
        }

        public int UserId
        {
            get => _userId;
            set => Set(ref _userId, value);
        }

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
    }

    public class ComponentItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebSite { get; set; }
        public string License { get; set; }
    }
}
