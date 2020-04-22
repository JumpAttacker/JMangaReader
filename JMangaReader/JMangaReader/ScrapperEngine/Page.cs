using System.Collections.Generic;

namespace JMangaReader.ScrapperEngine
{
    class Page: IPage
    {
        public string ImageUrl { get; set; }
        public List<IComment> Comments { get; set; }
    }
}