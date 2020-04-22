using System.Collections.Generic;
using System.ComponentModel;
using JMangaReader.Models;
using Xamarin.Forms;

namespace JMangaReader.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            var menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title = "Browse"},
                new HomeMenuItem {Id = MenuItemType.MangaSelectorView, Title = "MangaSelectorView"},
                new HomeMenuItem {Id = MenuItemType.About, Title = "About"}
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = ((HomeMenuItem) e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }

        private MainPage RootPage => Application.Current.MainPage as MainPage;
    }
}