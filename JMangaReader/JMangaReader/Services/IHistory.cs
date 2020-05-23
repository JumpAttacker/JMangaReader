using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.Services
{
    public interface IHistory
    {
        Task<HistoryModel> GetListOfLastManga();
        IChapter GetLastChapterOfManga(IManga manga);
        Task AddMangaToHistory(IManga manga);
        void AddChapterToHistory(IChapter chapter);
        List<IChapter> GetListOfWatchedChapters(IManga manga);

        Task ClearMangaHistory();

    }
}