namespace NeteaseCloudMusic.Wpf.Services.GetArtistDetail
{
    public class ArtistDetail
    {
        public Artist artist { get; set; }
        public Hotsong[] hotSongs { get; set; }
        public bool more { get; set; }
        public int code { get; set; }
    }

    public class Artist
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int albumSize { get; set; }
        public long picId { get; set; }
        public object[] alias { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public string trans { get; set; }
        public string picUrl { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public long publishTime { get; set; }
        public int mvSize { get; set; }
    }

    public class Hotsong
    {
        public bool starred { get; set; }
        public float popularity { get; set; }
        public int starredNum { get; set; }
        public int playedNum { get; set; }
        public int dayPlays { get; set; }
        public int hearTime { get; set; }
        public string mp3Url { get; set; }
        public object rtUrls { get; set; }
        public long mark { get; set; }
        public int status { get; set; }
        public int fee { get; set; }
        public Album album { get; set; }
        public string[] alias { get; set; }
        public Artist3[] artists { get; set; }
        public int mvid { get; set; }
        public int no { get; set; }
        public Hmusic hMusic { get; set; }
        public Mmusic mMusic { get; set; }
        public Lmusic lMusic { get; set; }
        public int score { get; set; }
        public int copyrightId { get; set; }
        public object audition { get; set; }
        public string copyFrom { get; set; }
        public string ringtone { get; set; }
        public string disc { get; set; }
        public object crbt { get; set; }
        public Bmusic bMusic { get; set; }
        public object rtUrl { get; set; }
        public int ftype { get; set; }
        public int rtype { get; set; }
        public object rurl { get; set; }
        public int duration { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Album
    {
        public object[] songs { get; set; }
        public bool paid { get; set; }
        public bool onSale { get; set; }
        public int mark { get; set; }
        public int status { get; set; }
        public string blurPicUrl { get; set; }
        public string commentThreadId { get; set; }
        public string briefDesc { get; set; }
        public long picId { get; set; }
        public Artist1 artist { get; set; }
        public string[] alias { get; set; }
        public Artist2[] artists { get; set; }
        public long publishTime { get; set; }
        public int copyrightId { get; set; }
        public int companyId { get; set; }
        public long pic { get; set; }
        public string picUrl { get; set; }
        public string company { get; set; }
        public string subType { get; set; }
        public string description { get; set; }
        public string tags { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string[] transNames { get; set; }
        public string picId_str { get; set; }
    }

    public class Artist1
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int albumSize { get; set; }
        public int picId { get; set; }
        public object[] alias { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public string trans { get; set; }
        public string picUrl { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string img1v1Id_str { get; set; }
    }

    public class Artist2
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int albumSize { get; set; }
        public int picId { get; set; }
        public object[] alias { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public string trans { get; set; }
        public string picUrl { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string img1v1Id_str { get; set; }
    }

    public class Hmusic
    {
        public int playTime { get; set; }
        public long dfsId { get; set; }
        public float volumeDelta { get; set; }
        public int sr { get; set; }
        public int bitrate { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public string dfsId_str { get; set; }
    }

    public class Mmusic
    {
        public int playTime { get; set; }
        public long dfsId { get; set; }
        public float volumeDelta { get; set; }
        public int sr { get; set; }
        public int bitrate { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public string dfsId_str { get; set; }
    }

    public class Lmusic
    {
        public int playTime { get; set; }
        public long dfsId { get; set; }
        public float volumeDelta { get; set; }
        public int sr { get; set; }
        public int bitrate { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public string dfsId_str { get; set; }
    }

    public class Bmusic
    {
        public int playTime { get; set; }
        public long dfsId { get; set; }
        public float volumeDelta { get; set; }
        public int sr { get; set; }
        public int bitrate { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public string dfsId_str { get; set; }
    }

    public class Artist3
    {
        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string briefDesc { get; set; }
        public bool followed { get; set; }
        public int albumSize { get; set; }
        public int picId { get; set; }
        public object[] alias { get; set; }
        public int musicSize { get; set; }
        public string img1v1Url { get; set; }
        public string trans { get; set; }
        public string picUrl { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string img1v1Id_str { get; set; }
    }

}
