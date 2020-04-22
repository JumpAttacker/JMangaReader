using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Forms;
using JMangaReader.ScrapperEngine;
using JMangaReader.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    public class ChapterViewModel : INotifyPropertyChanged
    {
        private readonly IChapter _chapter;

        public ChapterViewModel(IChapter chapter)
        {
            _chapter = chapter;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<ImageUrl> Images { get; set; } = new ObservableCollection<ImageUrl>();
        public bool IsBusy { get; set; }
        public int MaxCountOfImages { get; set; }
        public int LoadedImages { get; set; }
        public double GetProgressBar => (float) LoadedImages / (float) MaxCountOfImages;

        public string GetLoadingText =>
            IsBusy ? $"Loading... {LoadedImages}/{MaxCountOfImages}" : $"Глава: {_chapter.ChapterName}";
    }

    class ImageFit : Image
    {
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            SizeRequest sizeRequest = base.OnMeasure(double.PositiveInfinity, double.PositiveInfinity);

            var innerRatio = sizeRequest.Request.Width / sizeRequest.Request.Height;

            // Width needs to be adjusted
            if (double.IsInfinity(heightConstraint))
            {
                // Height needs to be adjusted
                if (double.IsInfinity(widthConstraint))
                {
                    widthConstraint = sizeRequest.Request.Width;
                    heightConstraint = sizeRequest.Request.Height;
                }
                else
                {
                    // Adjust height
                    heightConstraint = widthConstraint * sizeRequest.Request.Height / sizeRequest.Request.Width;
                }
            }
            else if (double.IsInfinity(widthConstraint))
            {
                // Adjust width
                widthConstraint = heightConstraint * sizeRequest.Request.Width / sizeRequest.Request.Height;
            }
            else
            {
                // strech the image to make it fit while conserving it's ratio
                var outerRatio = widthConstraint / heightConstraint;

                var ratioFactor = (innerRatio >= outerRatio)
                    ? (widthConstraint / sizeRequest.Request.Width)
                    : (heightConstraint / sizeRequest.Request.Height);

                widthConstraint = sizeRequest.Request.Width * ratioFactor;
                heightConstraint = sizeRequest.Request.Height * ratioFactor;
            }

            sizeRequest = new SizeRequest(new Size(widthConstraint, heightConstraint));
            return sizeRequest;
        }
    }

    public class ImageUrl
    {
        public string Url { get; set; }

        public ImageUrl(string url)
        {
            Url = url;
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChapterView : ContentPage
    {
        private readonly IReadOnlyList<IChapter> _chapters;
        private readonly int _currentChapterIndex;
        public IChapter Chapter { get; set; }

        public ChapterViewModel ViewModel { get; set; }
        // public ObservableCollection<ImageUrl> ImagesObservableCollection { get; } = new ObservableCollection<ImageUrl>();

        public int Left = -1;

        public ChapterView(IReadOnlyList<IChapter> chapters, int currentChapterIndex)
        {
            _chapters = chapters;
            _currentChapterIndex = currentChapterIndex;
            Chapter = chapters[currentChapterIndex];
            InitializeComponent();
            BindingContext = ViewModel = new ChapterViewModel(Chapter);
            CollectionView.ItemsSource = ViewModel.Images;
            FreshLoading(Chapter);
        }

        public void FreshLoading(IChapter chapter)
        {
            ViewModel.Images.Clear();
            ViewModel.IsBusy = false;
            ViewModel.LoadedImages = 0;
            ViewModel.MaxCountOfImages = 0;
            Chapter = chapter;
            StartPreLoading();
        }

        private void StartPreLoading()
        {
            ViewModel.IsBusy = true;
            Browser.Navigated += BrowserOnNavigated;
            Browser.Source = Chapter.Url;
        }

        private async void BrowserOnNavigated(object sender, WebNavigatedEventArgs e)
        {
            Browser.Navigated -= BrowserOnNavigated;
            var pageCount =
                await Browser.EvaluateJavaScriptAsync("document.body.querySelector('span.pages-count').innerText");
            var imageUrl = await Browser.EvaluateJavaScriptAsync(
                "Array.from(document.body.querySelectorAll('#mangaPicture')).map(a => a.src)[0];");
            Left = int.Parse(pageCount);
            ViewModel.MaxCountOfImages = Left;
            ViewModel.Images.Add(new ImageUrl(imageUrl));
            ViewModel.LoadedImages++;
            // var path = await DownloadImageAsync(new Uri(imageUrl));
            if (imageUrl == null || string.IsNullOrEmpty(imageUrl))
                return;
            var data = await new WebClient().DownloadDataTaskAsync(imageUrl);
            var fileService = DependencyService.Get<IFileService>();
            fileService.SavePicture($"{Chapter.ChapterName}_{0}.jpg", data, Chapter.Manga.MangaName);
            for (var i = 0; i < Left; i++)
            {
                await Browser.EvaluateJavaScriptAsync("document.getElementById('mangaPicture').click()");
                imageUrl = await Browser.EvaluateJavaScriptAsync(
                    "Array.from(document.body.querySelectorAll('#mangaPicture')).map(a => a.src)[0];");
                data = await new WebClient().DownloadDataTaskAsync(imageUrl);
                fileService.SavePicture($"{Chapter.ChapterName}_{i + 1}.jpg", data, Chapter.Manga.MangaName);
                ViewModel.Images.Add(new ImageUrl(imageUrl));
                ViewModel.LoadedImages++;
                await Task.Delay(15);
            }

            ViewModel.IsBusy = false;
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Browser.IsVisible = !Browser.IsVisible;
        }

        private void CollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((CollectionView) sender).SelectedItem = null;
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            DisplayAlert("move", "move", "ook");
        }

        enum ChapterChangeType
        {
            Back,
            Next
        }

        private async void ChangeChapter(ChapterChangeType type)
        {
            var nextChapterId = _currentChapterIndex + (type == ChapterChangeType.Back ? -1 : 1);
            var nextChapter = _chapters[nextChapterId];
            if (nextChapter == null)
            {
                await DisplayAlert("Info", "There is no more chapters", "ok :(");
                return;
            }

            FreshLoading(nextChapter);
        }

        private void SwipeGestureRecognizer_OnSwiped(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Right:
                {
                    ChangeChapter(ChapterChangeType.Back);
                    break;
                }
                case SwipeDirection.Left:
                {
                    ChangeChapter(ChapterChangeType.Next);
                    break;
                }
                case SwipeDirection.Up:
                    //TODO: settings
                    break;
                case SwipeDirection.Down:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}