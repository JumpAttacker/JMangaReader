using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using JMangaReader.ScrapperEngine;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChapterSelector : ContentPage
    {
        private readonly IScrapper _scrapper;
        public ObservableCollection<string> Items { get; set; }

        public ChapterSelector(IScrapper scrapper)
        {
            _scrapper = scrapper;
            InitializeComponent();
            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            MyListView.ItemsSource = scrapper.SelectedManga.Chapters;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var chapter = e.Item as Chapter;
            await Navigation.PushAsync(new ChapterView(_scrapper.SelectedManga.Chapters, e.ItemIndex));
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
