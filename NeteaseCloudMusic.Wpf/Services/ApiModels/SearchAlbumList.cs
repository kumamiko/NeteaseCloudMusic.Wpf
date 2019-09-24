namespace NeteaseCloudMusic.Wpf.Services.SearchAlbumList
{
    public class AlbumList
    {
        public Result result { get; set; }
        public int code { get; set; }
    }

    public class Result
    {
        public Album[] albums { get; set; }
        public int albumCount { get; set; }
    }

    public class Album
    {
        public string name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public long picId { get; set; }
        public string blurPicUrl { get; set; }
        public int companyId { get; set; }
        public long pic { get; set; }
        public string picUrl { get; set; }
        public long publishTime { get; set; }
        public string description { get; set; }
        public string tags { get; set; }
        public string company { get; set; }
        public string briefDesc { get; set; }
        public Artist artist { get; set; }
        public object songs { get; set; }
        public string[] alias { get; set; }
        public int status { get; set; }
        public int copyrightId { get; set; }
        public string commentThreadId { get; set; }
        public Artist1[] artists { get; set; }
        public bool paid { get; set; }
        public bool onSale { get; set; }
        public string picId_str { get; set; }
        public string containedSong { get; set; }
        public int mark { get; set; }
        public string alg { get; set; }
    }

    public class Artist
    {
        public string name { get; set; }
        public int id { get; set; }
        public long picId { get; set; }
        public long img1v1Id { get; set; }
        public string briefDesc { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
        public int albumSize { get; set; }
        public string[] alias { get; set; }
        public string trans { get; set; }
        public int musicSize { get; set; }
        public int topicPerson { get; set; }
        public string img1v1Id_str { get; set; }
        public string[] alia { get; set; }
        public string picId_str { get; set; }
        public string[] transNames { get; set; }
    }

    public class Artist1
    {
        public string name { get; set; }
        public int id { get; set; }
        public int picId { get; set; }
        public long img1v1Id { get; set; }
        public string briefDesc { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
        public int albumSize { get; set; }
        public object[] alias { get; set; }
        public string trans { get; set; }
        public int musicSize { get; set; }
        public int topicPerson { get; set; }
        public string img1v1Id_str { get; set; }
    }

}
