using System.Collections.Generic;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.Models
{
    public class ChapterViewModel
    {
        public string Url { get; set; }
        public string ChapterName { get; set; }
    }

    public class FavoriteHistoryModel
    {
        public List<MangaHistoryViewModel> Favorites { get; set; }
    }
}