using LiteDB;
using NeteaseCloudMusic.Wpf.Model;
using System.Collections.Generic;

namespace NeteaseCloudMusic.Wpf.Services
{
    public class DataService
    {
        private readonly string _path = "Data.db";

        #region Music
        public IEnumerable<MusicInfo> GetAllMusic()
        {
            using (var db = new LiteDatabase(_path))
            {
                return db.GetCollection<MusicInfo>("musics").Find(Query.All());
            }
        }

        public void SaveAllMusic(List<MusicInfo> musics)
        {
            using (var db = new LiteDatabase(_path))
            {
                var col = db.GetCollection<MusicInfo>("musics");
                col.InsertBulk(musics);
            }
        }

        public void RemoveMusic(MusicInfo history)
        {
            using (var db = new LiteDatabase(_path))
            {
                var col = db.GetCollection<MusicInfo>("musics");
                col.Delete(Query.EQ("Id", history.Id));
            }
        }

        public void AddMusic(MusicInfo musics)
        {
            using (var db = new LiteDatabase(_path))
            {
                var col = db.GetCollection<MusicInfo>("musics");
                var item = col.FindOne(Query.EQ("Id", musics.Id));
                if (item == null) col.Insert(musics);
            }
        }

        public void ClearAllMusic()
        {
            using (var db = new LiteDatabase(_path))
            {
                db.DropCollection("musics");
            }
        }

        public void ClearLocalMusic()
        {
            using (var db = new LiteDatabase(_path))
            {
                var col = db.GetCollection<MusicInfo>("musics");
                col.Delete(Query.EQ("Type", 0));
            }
        }
        #endregion

        #region History
        public void AddHistory(History history)
        {
            using (var db = new LiteDatabase(_path))
            {
                var col = db.GetCollection<History>("histories");
                var item = col.FindOne(Query.EQ("Keyword", history.Keyword));
                if (item == null) col.Insert(history);
            }
        }

        public IEnumerable<History> GetAllHistory()
        {
            using (var db = new LiteDatabase(_path))
            {
                return db.GetCollection<History>("histories").Find(Query.All("CreateTime", Query.Descending));
            }
        }

        public void RemoveHistory(History history)
        {
            using (var db = new LiteDatabase(_path))
            {
                var col = db.GetCollection<History>("histories");
                col.Delete(Query.Contains("Keyword", history.Keyword));
            }
        }

        public void ClearAllHistory()
        {
            using (var db = new LiteDatabase(_path))
            {
                db.DropCollection("histories");
            }
        }
        #endregion
    }
}
