using System;
using JMangaReader.ScrapperEngine;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using JMangaReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChapterSelector : ContentPage
    {
        private readonly IScrapper _scrapper;

        public ChapterSelector(IScrapper scrapper)
        {
            _scrapper = scrapper;
            InitializeComponent();

            // MyListView.ItemsSource = scrapper.SelectedManga.Chapters;
            ChapterSelectorViewModel =
                new ChapterSelectorViewModel(scrapper.SelectedManga, scrapper.SelectedManga.Chapters)
                {
                    IsFavorite = _scrapper.SelectedManga.IsFavorite
                };

            BindingContext = ChapterSelectorViewModel;
        }
        
        

        public ChapterSelectorViewModel ChapterSelectorViewModel { get; set; }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var chapter = (Chapter) e.Item;
            await Navigation.PushAsync(new ChapterView(_scrapper.SelectedManga.Chapters, e.ItemIndex));
            //Deselect Item
            ((ListView) sender).SelectedItem = null;
        }

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            var historyService = DependencyService.Get<IHistory>();
            ChapterSelectorViewModel.IsFavorite = _scrapper.SelectedManga.IsFavorite = !_scrapper.SelectedManga.IsFavorite;
            ChapterSelectorViewModel.IsLoading = true;
            if (ChapterSelectorViewModel.IsFavorite)
            {
                await historyService.AddMangaToFavorite(_scrapper.SelectedManga);
            }
            else
            {
                await historyService.RemoveMangaFromFavorite(_scrapper.SelectedManga);
            }
            ChapterSelectorViewModel.IsLoading = false;
        }
        private void MenuItem_Sort_OnClicked(object sender, EventArgs e)
        {
            ChapterSelectorViewModel.IsSort = !ChapterSelectorViewModel.IsSort;
        }
    }
}