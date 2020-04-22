using System.Collections.Generic;

namespace JMangaReader.ScrapperEngine
{
    public interface IChapter
    {
        string ChapterName { get; set; }
        string Url  { get; set; }
        bool Downloaded { get; set; }
        List<IPage> Pages { get; set; }
        IManga Manga { get; set; }
        bool IsWatch { get; set; }
        void Load();
        void Save(bool value);

    }
}