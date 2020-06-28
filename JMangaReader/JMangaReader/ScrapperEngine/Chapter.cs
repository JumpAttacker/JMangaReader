using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Akavache;
using JMangaReader.ScrapperEngine.Interface;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public class Chapter : IChapter
    {
        private static int _currentId = 0;

        public Chapter(string chapterName, string url, IManga manga)
        {
            ChapterName = chapterName;
            var index = url.IndexOf("#page", StringComparison.Ordinal);
            var sub = url;//url.Replace("res", "");
            Url = index > 0 ? sub.Remove(index) : sub;
            Manga = manga;
            Id = _currentId++;
        }

        public async void Load()
        {
            // IsWatch = await BlobCache.UserAccount.GetOrCreateObject($"{Manga.MangaName}_{ChapterName}", () => true);
        }

        public async void Save(bool value)
        {
            // await BlobCache.UserAccount.InsertObject($"{Manga.MangaName}_{ChapterName}", value);
        }

        public bool InHistory { get; set; }
        public Color Color => !InHistory ? Color.Default : Color.Gray;
        public int Id { get; set; }

        public string ChapterName { get; set; }
        public string Url { get; set; }
        public bool Downloaded { get; set; }
        public List<IPage> Pages { get; set; }
        public IManga Manga { get; set; }

        public override string ToString()
        {
            return ChapterName;
        }
    }
}