namespace NeteaseCloudMusic.Wpf.Services.SearchPlayList
{
    public class PlaylistList
    {
        public Result result { get; set; }
        public int code { get; set; }
    }

    public class Result
    {
        public Playlist[] playlists { get; set; }
        public int playlistCount { get; set; }
    }

    public class Playlist
    {
        public long id { get; set; }
        public string name { get; set; }
        public string coverImgUrl { get; set; }
        public Creator creator { get; set; }
        public bool subscribed { get; set; }
        public int trackCount { get; set; }
        public int userId { get; set; }
        public int playCount { get; set; }
        public int bookCount { get; set; }
        public string description { get; set; }
        public bool highQuality { get; set; }
        public string alg { get; set; }
    }

    public class Creator
    {
        public string nickname { get; set; }
        public int userId { get; set; }
        public int userType { get; set; }
        public int authStatus { get; set; }
        public object expertTags { get; set; }
        public object experts { get; set; }
    }

}
