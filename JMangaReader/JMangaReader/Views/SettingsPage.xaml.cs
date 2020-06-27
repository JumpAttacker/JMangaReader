using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JMangaReader.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            saveImages.On = DependencyService.Get<IFileService>().IsActive;
        }

        private void SwitchCell_OnOnChanged(object sender, ToggledEventArgs toggledEventArgs)
        {
            var isOn = toggledEventArgs.Value;
            
            var fileService = DependencyService.Get<IFileService>();
            fileService.IsActive = isOn;
        }
    }
}