using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var manga = new Manga(model.MangaName, model.Url, model.ImageUrl);
                MangaSelectorModel.MangaList.Add(manga);
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
    }
}