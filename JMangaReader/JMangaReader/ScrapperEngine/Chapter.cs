using System.Collections.Generic;

namespace JMangaReader.ScrapperEngine
{
    public class Chapter : IChapter
    {
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