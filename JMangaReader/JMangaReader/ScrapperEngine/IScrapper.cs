using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine
{
    public interface IScrapper
    {
        void SetWebView(WebView webView);
        Task<string> GetHtml();
        Task<IList<Manga>> Search(string args);
        Task<bool> SelectManga(IManga manga);
        IManga SelectedManga { get; set; }

    }
}