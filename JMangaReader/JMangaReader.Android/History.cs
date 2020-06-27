using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using JMangaReader.Droid;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(History))]

namespace JMangaReader.Droid
{
    public class History : IHistory
    {
        public async Task<HistoryMangaModel> GetListOfLastManga()
        {
            var manga =
                await BlobCache.UserAccount.GetOrCreateObject($"MangaHistory",
                    () => new HistoryMangaModel {MangaHistoryViewModels = new List<MangaHistoryViewModel>()});
            manga.MangaHistoryViewModels ??= new List<MangaHistoryViewModel>();
            return manga;
        }

        public async Task<HistoryChapterModel> GetChapterListOfManga(IManga manga)
        {
            var chapters =
                await BlobCache.UserAccount.GetOrCreateObject($"MangaChapterHistory_{manga.Url}",
                    () => new HistoryChapterModel {ChapterHistoryViewModels = new List<ChapterViewModel>()});
            chapters.ChapterHistoryViewModels ??= new List<ChapterViewModel>();
            return chapters;
        }

        public async Task AddMangaToHistory(IManga manga)
        {
            var model = new MangaHistoryViewModel
            {
                Url = manga.Url,
                ImageUrl = manga.ImageUrl,
                MangaName = manga.MangaName,
                CountOfChapters = manga.CountOfChapters
            };
            var historyMangaModel = await GetListOfLastManga();
            var modelInHistory = historyMangaModel.MangaHistoryViewModels.FirstOrDefault(x => x.Url == model.Url);
            if (modelInHistory != null)
            {
                historyMangaModel.MangaHistoryViewModels.Remove(modelInHistory);
            }

            historyMangaModel.MangaHistoryViewModels.Add(model);
            await BlobCache.UserAccount.InsertObject($"MangaHistory", historyMangaModel);
        }

        public async void AddChapterToHistory(IChapter chapter)
        {
            var model = new ChapterViewModel
            {
                Url = chapter.Url,
                ChapterName = chapter.ChapterName
            };
            chapter.InHistory = true;
            var historyChapterModel = await GetChapterListOfManga(chapter.Manga);
            var modelInHistory = historyChapterModel.ChapterHistoryViewModels.FirstOrDefault(x => x.Url == model.Url);
            if (modelInHistory != null)
            {
                historyChapterModel.ChapterHistoryViewModels.Remove(modelInHistory);
            }

            historyChapterModel.ChapterHistoryViewModels.Add(model);
            await BlobCache.UserAccount.InsertObject($"MangaChapterHistory_{chapter.Manga.Url}", historyChapterModel);
        }

        public async Task ClearMangaHistory()
        {
            await BlobCache.UserAccount.InvalidateObject<HistoryMangaModel>("MangaHistory");
        }

        public async Task AddMangaToFavorite(IManga manga)
        {
            var favoriteList = await BlobCache.UserAccount.GetOrCreateObject("favoriteList",
                () => new FavoriteHistoryModel() {Favorites = new List<MangaHistoryViewModel>()});

            var inFavoriteManga = favoriteList.Favorites.FirstOrDefault(x => x.Url == manga.Url);
            if (inFavoriteManga != null)
            {
                return;
            }

            favoriteList.Favorites.Add(new MangaHistoryViewModel()
            {
                Url = manga.Url, MangaName = manga.MangaName, ImageUrl = manga.ImageUrl,
                CountOfChapters = manga.CountOfChapters
            });

            await BlobCache.UserAccount.InsertObject("favoriteList", favoriteList);
        }

        public async Task RemoveMangaFromFavorite(IManga manga)
        {
            var favoriteList = await BlobCache.UserAccount.GetOrCreateObject("favoriteList",
                () => new FavoriteHistoryModel() {Favorites = new List<MangaHistoryViewModel>()});

            var inFavoriteManga = favoriteList.Favorites.FirstOrDefault(x => x.Url == manga.Url);
            if (inFavoriteManga != null)
            {
                favoriteList.Favorites.Remove(inFavoriteManga);
                await BlobCache.UserAccount.InsertObject("favoriteList", favoriteList);
            }
        }

        public async Task<bool> IsMangaFavorite(IManga manga)
        {
            var favoriteList = await BlobCache.UserAccount.GetOrCreateObject("favoriteList",
                () => new FavoriteHistoryModel() {Favorites = new List<MangaHistoryViewModel>()});

            return favoriteList.Favorites.FirstOrDefault(x => x.Url == manga.Url) != null;
        }

        public async Task<FavoriteHistoryModel> GetFavoriteListManga()
        {
            var favoriteList = await BlobCache.UserAccount.GetOrCreateObject("favoriteList",
                () => new FavoriteHistoryModel() {Favorites = new List<MangaHistoryViewModel>()});
            return favoriteList;
        }
    }
}