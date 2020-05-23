using System;
using System.Linq;
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
    public partial class MangaSelectorInHistory : ContentPage
    {
        public MangaSelectorInHistory()
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

            BindingContext = MangaSelectorModel = new MangaSelectorModel();

            GetData();
        }


        private MangaSelectorModel MangaSelectorModel { get; }

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

            var mangaHistory = await historyService.GetListOfLastManga();
            // var history = await historyService.GetListOfLastManga();
            // Console.WriteLine("kek");
            foreach (var model in mangaHistory.MangaHistoryViewModels.OrderByDescending(x => x.Created))
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

        /// <summary>
        /// clear history
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            var historyService = DependencyService.Get<IHistory>();
            if (historyService == null)
            {
                return;
            }

            MangaSelectorModel.IsBusy = true;
            await historyService.ClearMangaHistory();
            await GetData();
        }

        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void Refresh(object sender, EventArgs e)
        {
            await GetData();
        }
    }
}