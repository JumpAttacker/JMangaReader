using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportPage : ContentPage
    {
        public ImportPage()
        {
            InitializeComponent();

            Button.Clicked += (sender, args) =>
            {
                Button.IsEnabled = false;
                Button.IsVisible = false;
                Browser.Source = BookmarksUrl;
                Browser.IsVisible = true;
                IsLoading.IsVisible = false;
                infoText.IsVisible = false;
                Browser.Navigated += async (o, eventArgs) =>
                {
                    if (eventArgs.Url == "https://grouple.co/private/bookmarks")
                    {
                        IsLoading.IsVisible = true;
                        IsLoading.IsRunning = true;
                        Browser.IsVisible = false;
                        await GetBookmarks();
                        IsLoading.IsVisible = false;
                        await DisplayAlert("Info", "Loaded", "okay");
                    }
                };
            };
        }


        public async Task GetBookmarks()
        {
            var json = await Browser.EvaluateJavaScriptAsync(
                "Array.from(document.body.querySelectorAll('.bookmark-row'))" +
                ".map(x=>{return {href: x.cells[1].children[0].href, name: x.cells[1].children[0].innerHTML, lastChapterUrl: x.cells[1].children[2].href, imageUrl: x.cells[1].children[1].rel}})");
            List<Bookmark> bookmarks = JsonConvert.DeserializeObject<List<Bookmark>>(json);
            var historyService = DependencyService.Get<IHistory>();
            var mangas = bookmarks
                .Select(x =>
                {
                    var index = x.Name.IndexOf("<sup>", StringComparison.Ordinal);
                    var name = index > 0 ? x.Name.Remove(index) : x.Name;
                    var manga = new Manga(name, x.Href.Replace("https://readmanga.me", ""), x.ImageUrl);
                    var chapter = new Chapter("", x.LastChapterUrl, manga);
                    historyService.AddChapterToHistory(chapter);
                    return manga;
                }).ToList();
            var maxLength = mangas.Count;
            var loaded = 1;
            infoText.IsVisible = true;
            foreach (var manga in mangas)
            {
                infoText.Text =
                    $"Загрузка... {loaded}/{maxLength} {(int) ((float) loaded / (float) maxLength * 100f)}%";
                await manga.LoadChaptersListAsync();
                loaded++;
            }

            infoText.Text = "Успешная загрузка!";
            await historyService.AddMangaToFavorite(mangas);
        }

        public string BookmarksUrl { get; } = "https://grouple.co/private/bookmarks";
    }
}