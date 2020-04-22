using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public class ReadMangaScrapper : IScrapper
    {
        private WebView _webView;
        private string _baseUrl = "https://readmanga.me/";
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
            var response = await client.PostAsync(_baseUrl + "search?q=" + args, null);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            var html = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(html);
            // var results = document.GetElementbyId("mangaResults");
            var docNode = document.DocumentNode;
            var descriptions = docNode.Descendants("div").Where(x => x.HasClass("desc"));
            var mangaLinks = descriptions
                .Select(x => new
                {
                    h3 = x.Descendants("h3").First(),
                    h4 = x.Descendants("h4").FirstOrDefault()
                }).Select(x =>
                    new Manga(
                        $"{x.h3.InnerText} {(x.h4 != null ? $"({x.h4.InnerText})" : string.Empty)}",
                        x.h3.Descendants("a").First().GetAttributeValue("href", string.Empty)));
            return mangaLinks.ToList();
        }

        public async Task<bool> SelectManga(IManga manga)
        {
            SelectedManga = manga;
            var chapters = await SelectedManga.LoadChaptersAsync();
            return chapters?.Count>0;
        }
    }
}