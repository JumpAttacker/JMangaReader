using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using JMangaReader.ScrapperEngine;
using JMangaReader.ScrapperEngine.Interface;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    public class ChapterSelectorViewModel : INotifyPropertyChanged
    {
        public ChapterSelectorViewModel(IManga manga, List<IChapter> selectedMangaChapters)
        {
            Manga = manga;
            Chapters.Clear();
            foreach (var chapter in selectedMangaChapters) Chapters.Add(chapter);
        }

        public IManga Manga { get; }
        public ObservableCollection<IChapter> Chapters { get; set; } = new ObservableCollection<IChapter>();
        public string ChapterCountText => $"Total chapters: {Chapters.Count}";

        public event PropertyChangedEventHandler PropertyChanged;
    }

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
                new ChapterSelectorViewModel(scrapper.SelectedManga, scrapper.SelectedManga.Chapters);
            BindingContext = ChapterSelectorViewModel;
        }

        public ChapterSelectorViewModel ChapterSelectorViewModel { get; set; }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var chapter = (Chapter) e.Item;
            chapter.IsWatch = true;
            await Navigation.PushAsync(new ChapterView(_scrapper.SelectedManga.Chapters, e.ItemIndex));
            //Deselect Item
            ((ListView) sender).SelectedItem = null;
        }
    }
}