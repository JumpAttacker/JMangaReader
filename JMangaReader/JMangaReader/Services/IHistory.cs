using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.Services
{
    public interface IHistory
    {
        Task<HistoryMangaModel> GetListOfLastManga();
        Task<HistoryChapterModel> GetChapterListOfManga(IManga manga);
        Task AddMangaToHistory(IManga manga);
        void AddChapterToHistory(IChapter chapter);
        Task ClearMangaHistory();

        Task AddMangaToFavorite(IManga manga);
        Task RemoveMangaFromFavorite(IManga manga);
        Task<bool> IsMangaFavorite(IManga manga);
        Task<FavoriteHistoryModel> GetFavoriteListManga();

    }
}