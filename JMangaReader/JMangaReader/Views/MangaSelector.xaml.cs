using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine;
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
            Scrapper = new ReadMangaScrapper();
            if (Scrapper != null)
                Scrapper.SetWebView(Browser);
            else
            {
                DisplayAlert("Error", "scrapper is null", "okay :(");
            }
            SearchInput.Completed += DoSearch;
            SearchButton.Clicked += DoSearch;
            ListView.ItemTapped += ListView_ItemTapped;

            BindingContext = this;
        }

        public IScrapper Scrapper { get; set; }
        private MainPage RootPage => Application.Current.MainPage as MainPage;

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedManga = e.Item as Manga;
            await Scrapper.SelectManga(selectedManga);
            await Navigation.PushAsync(new ChapterSelector(Scrapper));
        }

        private async void DoSearch(object sender, EventArgs e)
        {
            // var scrapper = DependencyService.Get<IScrapper>()
            
            var searchResult = await Scrapper.Search(SearchInput.Text);
            ListView.ItemsSource = searchResult;
            EmptyMessage.IsVisible = searchResult == null || searchResult.Count == 0;
        }


        private async void Button_OnClicked(object sender, EventArgs e)
        {
            // Browser.Navigated
            var element = await Browser.EvaluateJavaScriptAsync("document.body.innerHTML");
            var pageCounts2 =
                await Browser.EvaluateJavaScriptAsync("document.body.querySelector('span.pages-count').innerText");
            var a = await Browser.EvaluateJavaScriptAsync(
                "Array.from(document.body.querySelectorAll('#mangaPicture')).map(a => a.src)[0];");
            var imageArrayBytes = await DownloadImageAsync(a);
            if (imageArrayBytes != null)
            {
                Stream stream = new MemoryStream(imageArrayBytes);
                MangaImage.Source = ImageSource.FromStream(() => stream);
            }
        }

        private const int DownloadImageTimeoutInSeconds = 15;

        private readonly HttpClient _httpClient = new HttpClient
            {Timeout = TimeSpan.FromSeconds(DownloadImageTimeoutInSeconds)};

        async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                using (var httpResponse = await _httpClient.GetAsync(imageUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                //Handle Exception
                return null;
            }
        }
    }
}