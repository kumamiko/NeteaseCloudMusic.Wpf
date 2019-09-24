namespace NeteaseCloudMusic.Wpf.Services.SearchSongDetail
{
    public class SongDetail
    {
        public Song[] songs { get; set; }
        public Equalizers equalizers { get; set; }
        public int code { get; set; }
    }

    public class Equalizers
    {
    }

    public class Song
    {
        public string name { get; set; }
        public int id { get; set; }
        public int position { get; set; }
        public object[] alias { get; set; }
        public int status { get; set; }
        public int fee { get; set; }
        public int copyrightId { get; set; }
        public string disc { get; set; }
        public int no { get; set; }
        public Artist2[] artists { get; set; }
        public Album album { get; set; }
        public bool starred { get; set; }
        public float popularity { get; set; }
        public int score { get; set; }
        public int starredNum { get; set; }
        public int duration { get; set; }
        public int playedNum { get; set; }
        public int dayPlays { get; set; }
        public int hearTime { get; set; }
        public object ringtone { get; set; }
        public object crbt { get; set; }
        public object audition { get; set; }
        public string copyFrom { get; set; }
        public string commentThreadId { get; set; }
        public object rtUrl { get; set; }
        public int ftype { get; set; }
        public object[] rtUrls { get; set; }
        public int copyright { get; set; }
        public object transName { get; set; }
        public object sign { get; set; }
        public int mark { get; set; }
        public int mvid { get; set; }
        public Hmusic hMusic { get; set; }
        public Mmusic mMusic { get; set; }
        public Lmusic lMusic { get; set; }
        public Bmusic bMusic { get; set; }
        public object mp3Url { get; set; }
        public int rtype { get; set; }
        public object rurl { get; set; }
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
        public object[] songs { get; set; }
        public string[] alias { get; set; }
        public int status { get; set; }
        public int copyrightId { get; set; }
        public string commentThreadId { get; set; }
        public Artist1[] artists { get; set; }
        public string subType { get; set; }
        public object transName { get; set; }
        public int mark { get; set; }
    }

    public class Artist
    {
        public string name { get; set; }
        public int id { get; set; }
        public int picId { get; set; }
        public int img1v1Id { get; set; }
        public string briefDesc { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
        public int albumSize { get; set; }
        public object[] alias { get; set; }
        public string trans { get; set; }
        public int musicSize { get; set; }
        public int topicPerson { get; set; }
    }

    public class Artist1
    {
        public string name { get; set; }
        public int id { get; set; }
        public int picId { get; set; }
        public int img1v1Id { get; set; }
        public string briefDesc { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
        public int albumSize { get; set; }
        public object[] alias { get; set; }
        public string trans { get; set; }
        public int musicSize { get; set; }
        public int topicPerson { get; set; }
    }

    public class Hmusic
    {
        public object name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public int sr { get; set; }
        public int dfsId { get; set; }
        public int bitrate { get; set; }
        public int playTime { get; set; }
        public float volumeDelta { get; set; }
    }

    public class Mmusic
    {
        public object name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public int sr { get; set; }
        public int dfsId { get; set; }
        public int bitrate { get; set; }
        public int playTime { get; set; }
        public float volumeDelta { get; set; }
    }

    public class Lmusic
    {
        public object name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public int sr { get; set; }
        public int dfsId { get; set; }
        public int bitrate { get; set; }
        public int playTime { get; set; }
        public float volumeDelta { get; set; }
    }

    public class Bmusic
    {
        public object name { get; set; }
        public long id { get; set; }
        public int size { get; set; }
        public string extension { get; set; }
        public int sr { get; set; }
        public int dfsId { get; set; }
        public int bitrate { get; set; }
        public int playTime { get; set; }
        public float volumeDelta { get; set; }
    }

    public class Artist2
    {
        public string name { get; set; }
        public int id { get; set; }
        public int picId { get; set; }
        public int img1v1Id { get; set; }
        public string briefDesc { get; set; }
        public string picUrl { get; set; }
        public string img1v1Url { get; set; }
        public int albumSize { get; set; }
        public object[] alias { get; set; }
        public string trans { get; set; }
        public int musicSize { get; set; }
        public int topicPerson { get; set; }
    }

}
