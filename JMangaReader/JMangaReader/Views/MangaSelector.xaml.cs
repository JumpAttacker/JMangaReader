using System;
using JMangaReader.ScrapperEngine;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MangaSelector : ContentPage
    {
        public MangaSelector()
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

            SearchInput.Completed += DoSearch;
            SearchButton.Clicked += DoSearch;
            BindingContext = MangaSelectorModel = new MangaSelectorModel();
        }

        private MangaSelectorModel MangaSelectorModel { get; }

        public IScrapper Scrapper { get; set; }

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

        private async void DoSearch(object sender, EventArgs e)
        {
            MangaSelectorModel.IsBusy = true;
            MangaSelectorModel.MangaList.Clear();
            var searchResult = await Scrapper.Search(SearchInput.Text);
            foreach (var manga in searchResult) MangaSelectorModel.MangaList.Add(manga);

            MangaSelectorModel.IsBusy = false;
            MangaSelectorModel.IsErrorMessageVisible = MangaSelectorModel.MangaList.Count == 0;
        }
    }
}