using System;
using JMangaReader.ScrapperEngine;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using JMangaReader.Services;
using JMangaReader.Views;

namespace JMangaReader
{
    public partial class App : Application
    {
        public static string BaseImageUrl { get; } = "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";

        public App()
        {
            DependencyService.Register<MockDataStore>();
            DependencyService.Register<ReadMangaScrapper>();

            InitializeComponent();

            MainPage = new MainPage();
        }

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
