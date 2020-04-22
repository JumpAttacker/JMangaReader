using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public interface IManga
    {
        string MangaName { get; set; }
        string GetDisplayName { get; }
        string Url { get; set; }
        string ImageUrl { get; set; }
        List<IChapter> Chapters { get; set; }
        int CountOfChapters { get; }
        Task<IList<IChapter>> LoadChaptersListAsync(bool firstTime = true);
        
    }
}