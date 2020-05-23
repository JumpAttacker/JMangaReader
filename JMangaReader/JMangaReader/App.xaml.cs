using Akavache;
using JMangaReader.ScrapperEngine;
using JMangaReader.Services;
using JMangaReader.Views;
using Xamarin.Forms;

namespace JMangaReader
{
    public partial class App : Application
    {
        public App()
        {
            DependencyService.Register<MockDataStore>();
            DependencyService.Register<ReadMangaScrapper>();
            InitializeComponent();
            MainPage = new MainPage();
            Registrations.Start("JMangaParser");
        }

        public static string BaseImageUrl { get; } =
            "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}