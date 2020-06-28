using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine;
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
        Task ClearFavoriteList();
        Task AddMangaToFavorite(IManga manga);
        Task AddMangaToFavorite(List<Manga> mangas);
        Task RemoveMangaFromFavorite(IManga manga);
        Task<bool> IsMangaFavorite(IManga manga);
        Task<FavoriteHistoryModel> GetFavoriteListManga();
    }
}