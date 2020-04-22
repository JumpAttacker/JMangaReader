using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public interface IManga
    {
        string Name { get; set; }
        string Url { get; set; }
        List<IChapter> Chapters { get; set; }
        int CountOfChapters { get; }
        Task<IList<IChapter>> LoadChaptersAsync();
    }
}