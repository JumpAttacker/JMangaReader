using System.Collections.Generic;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.ScrapperEngine
{
    internal class Page : IPage
    {
        public string ImageUrl { get; set; }
        public List<IComment> Comments { get; set; }
    }
}