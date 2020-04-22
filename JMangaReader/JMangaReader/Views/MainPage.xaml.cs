using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using JMangaReader.Models;

namespace JMangaReader.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        readonly Dictionary<int, NavigationPage> _menuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            _menuPages.Add((int)MenuItemType.MangaSelectorView, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(MenuItemType type)
        {
            await NavigateFromMenu((int) type);
        }
        public async Task NavigateFromMenu(int id)
        {
            if (!_menuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        _menuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.About:
                        _menuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.MangaSelectorView:
                        _menuPages.Add(id, new NavigationPage(new MangaSelector()));
                        break;
                    // case (int)MenuItemType.ChapterSelectorView:
                    //     _menuPages.Add(id, new NavigationPage(new ChapterSelector()));
                    //     break;
                }
            }

            var newPage = _menuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}