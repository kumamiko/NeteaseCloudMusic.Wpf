namespace NeteaseCloudMusic.Wpf.Services.GetArtistAlbums
{
    public class ArtistAlbums
    {
        public Artist artist { get; set; }
        public Hotalbum[] hotAlbums { get; set; }
        public bool more { get; set; }
        public int code { get; set; }
    }

    public class Artist
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string picUrl { get; set; }
        public string trans { get; set; }
        public int albumSize { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public long picId { get; set; }
        public object[] alias { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string img1v1Id_str { get; set; }
    }

    public class Hotalbum
    {
        public object[] songs { get; set; }
        public bool paid { get; set; }
        public bool onSale { get; set; }
        public int mark { get; set; }
        public string picUrl { get; set; }
        public string blurPicUrl { get; set; }
        public int companyId { get; set; }
        public long pic { get; set; }
        public int status { get; set; }
        public string briefDesc { get; set; }
        public string commentThreadId { get; set; }
        public long publishTime { get; set; }
        public long picId { get; set; }
        public Artist1 artist { get; set; }
        public string company { get; set; }
        public string subType { get; set; }
        public string description { get; set; }
        public object[] alias { get; set; }
        public string tags { get; set; }
        public Artist2[] artists { get; set; }
        public int copyrightId { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string picId_str { get; set; }
    }

    public class Artist1
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string picUrl { get; set; }
        public string trans { get; set; }
        public int albumSize { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public long picId { get; set; }
        public object[] alias { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string img1v1Id_str { get; set; }
    }

    public class Artist2
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string picUrl { get; set; }
        public string trans { get; set; }
        public int albumSize { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public int picId { get; set; }
        public object[] alias { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string img1v1Id_str { get; set; }
    }

}
