namespace NeteaseCloudMusic.Wpf.Services.SearchSongList
{
    public class SongList
    {
        public Result result { get; set; }
        public int code { get; set; }
    }

    public class Result
    {
        public Song[] songs { get; set; }
        public int songCount { get; set; }
    }

    public class Song
    {
        public int id { get; set; }
        public string name { get; set; }
        public Artist1[] artists { get; set; }
        public Album album { get; set; }
        public int duration { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public string[] alias { get; set; }
        public int rtype { get; set; }
        public int ftype { get; set; }
        public int mvid { get; set; }
        public int fee { get; set; }
        public object rUrl { get; set; }
    }

    public class Album
    {
        public int id { get; set; }
        public string name { get; set; }
        public Artist artist { get; set; }
        public long publishTime { get; set; }
        public int size { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public long picId { get; set; }
        public string[] alia { get; set; }
    }

    public class Artist
    {
        public int id { get; set; }
        public string name { get; set; }
        public object picUrl { get; set; }
        public object[] alias { get; set; }
        public int albumSize { get; set; }
        public int picId { get; set; }
        public string img1v1Url { get; set; }
        public int img1v1 { get; set; }
        public object trans { get; set; }
    }
    public class Artist1
    {
        public int id { get; set; }
        public string name { get; set; }
        public object picUrl { get; set; }
        public object[] alias { get; set; }
        public int albumSize { get; set; }
        public int picId { get; set; }
        public string img1v1Url { get; set; }
        public int img1v1 { get; set; }
        public object trans { get; set; }
    }
}
