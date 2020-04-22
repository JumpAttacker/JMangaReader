using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;

namespace JMangaReader.ScrapperEngine
{
    public class Chapter : IChapter
    {
        public Chapter(string chapterName, string url, IManga manga)
        {
            ChapterName = chapterName;
            Url = url;
            Manga = manga;
            Load();
        }

        public async void Load()
        {
            IsWatch = await BlobCache.UserAccount.GetOrCreateObject($"{Manga.MangaName}_{ChapterName}", () => true);
        }

        public async void Save(bool value)
        {
            await BlobCache.UserAccount.InsertObject($"{Manga.MangaName}_{ChapterName}", value);
        }

        public string ChapterName { get; set; }
        public string Url { get; set; }
        public bool Downloaded { get; set; }
        public List<IPage> Pages { get; set; }
        public IManga Manga { get; set; }

        public bool IsWatch { get; set; }

        public override string ToString()
        {
            return ChapterName;
        }
    }
}