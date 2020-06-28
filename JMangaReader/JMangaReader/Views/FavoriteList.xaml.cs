using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JMangaReader.ScrapperEngine;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using JMangaReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoriteList : ContentPage
    {
        public FavoriteList()
        {
            InitializeComponent();
            Scrapper = DependencyService.Get<ReadMangaScrapper>();
            if (Scrapper != null)
            {
                Scrapper.SetWebView(Browser);
            }
            else
            {
                DisplayAlert("Error", "Scrapper service is broken", "okay :(");
                return;
            }

            BindingContext = MangaSelectorModel = new MangaListModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MangaSelectorModel.MangaList.Clear();

            await GetData();
        }

        private MangaListModel MangaSelectorModel { get; set; }

        public IScrapper Scrapper { get; set; }

        private async Task GetData()
        {
            MangaSelectorModel.IsBusy = true;
            MangaSelectorModel.MangaList.Clear();
            var historyService = DependencyService.Get<IHistory>();
            if (historyService == null)
            {
                return;
            }

            var mangaHistory = await historyService.GetFavoriteListManga();
            // var history = await historyService.GetListOfLastManga();
            // Console.WriteLine("kek");

            foreach (var model in mangaHistory.Favorites.OrderByDescending(x => x.Created))
            {
                try
                {
                    var manga = new Manga(model.MangaName, model.Url, model.ImageUrl);
                    var chapters =
                        new List<IChapter>(
                            model.Chapters.Select(x => new Chapter(x.ChapterName, x.Url, manga)).ToList());
                    var chaptersInHistory = await historyService.GetChapterListOfManga(manga);
                    foreach (var chapter in chapters.Where(x =>
                        chaptersInHistory.ChapterHistoryViewModels.FirstOrDefault(z =>
                            z.Url.Replace("?mtr=", "") == x.Url.Replace("?mtr=", "")) != null))
                    {
                        chapter.InHistory = true;
                    }

                    manga.Chapters = chapters;
                    // var task = new Thread(async o =>
                    // {
                    //     var chapters = await manga.LoadChaptersListAsync();
                    //     var chaptersInHistory = await historyService.GetChapterListOfManga(manga);
                    //     if (chaptersInHistory?.ChapterHistoryViewModels == null) return;
                    //     foreach (var chapter in chapters.Where(x =>
                    //         chaptersInHistory.ChapterHistoryViewModels.FirstOrDefault(z =>
                    //             z?.Url?.Replace("?mtr=", "") == x?.Url?.Replace("?mtr=", "")) != null))
                    //     {
                    //         chapter.InHistory = true;
                    //     }
                    // }) ;
                    // task.Start() ;

                    MangaSelectorModel.MangaList.Add(manga);
                }
                catch (Exception e)
                {
                    await DisplayAlert("Ошибка",
                        $"Ошибка при обновлении информации по манге {model.MangaName}. \r{e.Message}\r{e.Source}\r{e.StackTrace}",
                        "И шо?");
                    var manga = new Manga(model.MangaName, model.Url, model.ImageUrl);
                    MangaSelectorModel.MangaList.Add(manga);
                }
            }

            MangaSelectorModel.IsBusy = false;
            MangaSelectorModel.IsErrorMessageVisible = MangaSelectorModel.MangaList.Count == 0;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MangaSelectorModel.IsBusy = true;
            var selectedManga = e.Item as Manga;
            var done = await Scrapper.SelectManga(selectedManga);
            MangaSelectorModel.IsBusy = false;
            if (done)
                await Navigation.PushAsync(new ChapterSelector(Scrapper));
            else
                await DisplayAlert("Ошибка", "Ошибка при выборе манги", "И шо?");
        }

        private async void ClearHistory(object sender, EventArgs e)
        {
            var historyService = DependencyService.Get<IHistory>();
            await historyService.ClearFavoriteList();
            await DisplayAlert("Info", "Список успешно очищен", "Got it");
        }
    }
}