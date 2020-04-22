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
    public class Manga : IManga
    {
        public Manga(string name, string url)
        {
            Name = name;
            Url = url;
            Chapters = new List<IChapter>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public List<IChapter> Chapters { get; set; }
        public int CountOfChapters => Chapters.Count;
        public string PageUrl { get; set; } = "/vol1/1";

        public async Task<IList<IChapter>> LoadChaptersAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://readmanga.me/" + Url + PageUrl);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            var html = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(html);
            // var results = document.GetElementbyId("mangaResults");
            var docNode = document.DocumentNode;
            var select = docNode.Descendants("select").FirstOrDefault(x => x.Id == "chapterSelectorSelect");
            Chapters = new List<IChapter>(@select?.Descendants("option").Select(x => new Chapter
            {
                ChapterName = x.InnerText,
                Url = "https://readmanga.me/" + x.GetAttributeValue("value", string.Empty),
                Manga = this
            }).ToList() ?? throw new InvalidOperationException());
            return Chapters;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}