using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JMangaReader.Models;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public class ReadMangaScrapper : IScrapper
    {
        private const string BaseUrl = "https://readmanga.live/";
        private WebView _webView;
        private readonly IHistory _historyService;

        public ReadMangaScrapper()
        {
            _historyService = DependencyService.Get<IHistory>();
        }

        public IManga SelectedManga { get; set; }

        public void SetWebView(WebView newWebView)
        {
            _webView = newWebView;
        }

        public async Task<string> GetHtml()
        {
            return await _webView.EvaluateJavaScriptAsync("document.body.innerHTML");
        }

        public async Task<IList<Manga>> Search(string args)
        {
            var client = new HttpClient();
            var response = await client.PostAsync(BaseUrl + "search?q=" + args, null);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            var html = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(html);
            // var results = document.GetElementbyId("mangaResults");
            var docNode = document.DocumentNode;
            var tiles = docNode.Descendants("div").Where(x => x.HasClass("tile"));
            var data = tiles.Select(x => new
            {
                description = x.Descendants("div").FirstOrDefault(d => d.HasClass("desc")),
                image = x.Descendants("img").FirstOrDefault()
            });
            // var descriptions = docNode.Descendants("div").Where(x => x.HasClass("desc"));
            var mangaLinks = data
                .Select(x => new
                {
                    h3 = x.description.Descendants("h3").First(),
                    h4 = x.description.Descendants("h4").FirstOrDefault(),
                    imageUrl = x.image != null ? x.image.GetAttributeValue("data-original", string.Empty) : string.Empty
                }).Select(x =>
                    new Manga(
                        $"{x.h3.InnerText} {(x.h4 != null ? $"({x.h4.InnerText})" : string.Empty)}",
                        x.h3.Descendants("a").First().GetAttributeValue("href", string.Empty), x.imageUrl));
            return mangaLinks.Where(x =>
                (x.Url.StartsWith("/") || x.Url.StartsWith("https://mintmanga.live/")) &&
                !x.Url.StartsWith("/list/person/")).ToList();
        }

        public async Task<bool> SelectManga(IManga manga)
        {
            SelectedManga = manga;
            manga.IsFavorite = await _historyService.IsMangaFavorite(manga);
            var chapters = await SelectedManga.LoadChaptersListAsync();
            var chaptersInHistory = await GetChaptersInHistory(manga);
            foreach (var chapter in chapters.Where(x =>
                chaptersInHistory.ChapterHistoryViewModels.FirstOrDefault(z =>
                    z.Url.Replace("?mtr=", "") == x.Url.Replace("?mtr=", "")) != null))
            {
                chapter.InHistory = true;
            }

            await SaveManga(manga);
            return chapters?.Count > 0;
        }
        
        private async Task SaveManga(IManga manga)
        {
            await _historyService.AddMangaToHistory(manga);
            // var history = await historyService.GetListOfLastManga();
            // Console.WriteLine("kek");
        }

        private async Task<HistoryChapterModel> GetChaptersInHistory(IManga manga)
        {
            return await _historyService.GetChapterListOfManga(manga);
        }
    }
}