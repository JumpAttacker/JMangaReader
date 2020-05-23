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
        public async Task<HistoryModel> GetListOfLastManga()
        {
            var manga =
                await BlobCache.UserAccount.GetOrCreateObject<HistoryModel>($"MangaHistory",
                    () => new HistoryModel() {MangaHistoryViewModels = new List<MangaHistoryViewModel>()});
            manga.MangaHistoryViewModels ??= new List<MangaHistoryViewModel>();
            return manga;
        }

        public IChapter GetLastChapterOfManga(IManga manga)
        {
            throw new NotImplementedException();
        }

        public async Task AddMangaToHistory(IManga manga)
        {
            var model = new MangaHistoryViewModel()
            {
                Url = manga.Url,
                ImageUrl = manga.ImageUrl,
                MangaName = manga.MangaName,
                CountOfChapters = manga.CountOfChapters
            };
            HistoryModel historyModel = await GetListOfLastManga();
            var modelInHistory = historyModel.MangaHistoryViewModels.FirstOrDefault(x => x.Url == model.Url);
            if (modelInHistory != null)
            {
                historyModel.MangaHistoryViewModels.Remove(modelInHistory);
            }

            historyModel.MangaHistoryViewModels.Add(model);
            await BlobCache.UserAccount.InsertObject($"MangaHistory", historyModel);
        }

        public async void AddChapterToHistory(IChapter chapter)
        {
            var insert =
                await BlobCache.UserAccount.InsertObject($"{chapter.Manga.MangaName}", chapter);
            Console.WriteLine(insert);
        }

        public List<IChapter> GetListOfWatchedChapters(IManga manga)
        {
            throw new NotImplementedException();
        }

        public async Task ClearMangaHistory()
        {
            var keys = BlobCache.UserAccount.GetAllKeys();
            await BlobCache.UserAccount.InvalidateObject<HistoryModel>("MangaHistory");
        }
    }
}