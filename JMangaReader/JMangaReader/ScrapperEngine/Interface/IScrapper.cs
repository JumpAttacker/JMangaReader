using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JMangaReader.ScrapperEngine.Interface
{
    public interface IScrapper
    {
        IManga SelectedManga { get; set; }
        void SetWebView(WebView webView);
        Task<string> GetHtml();
        Task<IList<Manga>> Search(string args);
        Task<bool> SelectManga(IManga manga);
    }
}