using System.Collections.Generic;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine.Interface
{
    public interface IChapter
    {
        string ChapterName { get; set; }
        string Url { get; set; }
        bool Downloaded { get; set; }
        List<IPage> Pages { get; set; }
        IManga Manga { get; set; }
        void Load();
        void Save(bool value);
        bool InHistory { get; set; }
        Color Color { get; }
        int Id { get; set; }
    }
}