using System.Collections.Generic;

namespace JMangaReader.Models
{
    public class MangaFavoriteViewModel
    {
        public string MangaName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int CountOfChapters { get; set; }
        public List<ChapterViewModel> Chapters { get; set; }
    }
}