using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public class Manga : IManga
    {
        public Manga(string name, string url, string imageUrl)
        {
            MangaName = name;
            Url = url;
            ImageUrl = imageUrl;
            Chapters = new List<IChapter>();
            
        }

        public string PageUrl { get; set; } = "/vol1/1";

        public string MangaName { get; set; }
        public string GetDisplayName => Url.StartsWith("/") ? MangaName : $"{MangaName} (mint manga)";
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        
        public List<IChapter> Chapters { get; set; }
        public int CountOfChapters => Chapters.Count;

        public async Task<IList<IChapter>> LoadChaptersListAsync(bool firstTime = true)
        {
            var basePath = Url.StartsWith("/") ? "https://readmanga.me/" : "";
            var client = new HttpClient();
            var response = await client.GetAsync(basePath + Url + PageUrl + (firstTime ? string.Empty : "?mtr=1"));
            if (response.StatusCode != HttpStatusCode.OK) return null;
            var html = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(html);
            // var results = document.GetElementbyId("mangaResults");
            var docNode = document.DocumentNode;
            var select = docNode.Descendants("select").FirstOrDefault(x => x.Id == "chapterSelectorSelect");
            if (select == null)
                return firstTime ? await LoadChaptersListAsync(false) : new List<IChapter>();
            Chapters = new List<IChapter>(select?.Descendants("option").Select(x =>
                new Chapter(x.InnerText,
                    basePath + x.GetAttributeValue("value", string.Empty),
                    this)).ToList() ?? throw new InvalidOperationException());
            return Chapters;
        }

        public override string ToString()
        {
            return MangaName;
        }
    }
}