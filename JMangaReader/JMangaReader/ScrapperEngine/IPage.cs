using System.Collections.Generic;

namespace JMangaReader.ScrapperEngine
{
    public interface IPage
    {
        string ImageUrl { get; set; }
        List<IComment> Comments { get; set; }
    }
}