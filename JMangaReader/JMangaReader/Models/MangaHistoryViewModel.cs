using System;
using System.Collections.Generic;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.Models
{
    public class MangaHistoryViewModel
    {
        public string MangaName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int CountOfChapters { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}