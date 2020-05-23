using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
    public class ChapterViewModel : INotifyPropertyChanged
    {
        private IChapter _chapter;

        public ChapterViewModel(IChapter chapter)
        {
            _chapter = chapter;
        }

        public ObservableCollection<ImageUrl> Images { get; set; } = new ObservableCollection<ImageUrl>();
        public bool IsBusy { get; set; }
        public int MaxCountOfImages { get; set; }
        public int LoadedImages { get; set; }
        public double GetProgressBar => LoadedImages / (float) MaxCountOfImages;

        public string GetLoadingText =>
            IsBusy
                ? $"Глава: {_chapter.ChapterName}. [{LoadedImages}/{MaxCountOfImages}]"
                : $"Глава: {_chapter.ChapterName}";

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeChapter(IChapter chapter)
        {
            _chapter = chapter;
        }
    }

    internal class ImageFit : Image
    {
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var sizeRequest = base.OnMeasure(double.PositiveInfinity, double.PositiveInfinity);

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

                var ratioFactor = innerRatio >= outerRatio
                    ? widthConstraint / sizeRequest.Request.Width
                    : heightConstraint / sizeRequest.Request.Height;

                widthConstraint = sizeRequest.Request.Width * ratioFactor;
                heightConstraint = sizeRequest.Request.Height * ratioFactor;
            }

            sizeRequest = new SizeRequest(new Size(widthConstraint, heightConstraint));
            return sizeRequest;
        }
    }

    public class ImageUrl
    {
        public ImageUrl(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChapterView : ContentPage
    {
        private readonly IReadOnlyList<IChapter> _chapters;

        private int _currentChapterIndex;
        // public ObservableCollection<ImageUrl> ImagesObservableCollection { get; } = new ObservableCollection<ImageUrl>();

        public int Left = -1;

        public ChapterView(IReadOnlyList<IChapter> chapters, int currentChapterIndex)
        {
            _chapters = chapters;
            Chapter = chapters[currentChapterIndex];
            InitializeComponent();
            BindingContext = ViewModel = new ChapterViewModel(Chapter);
            // CollectionView.ItemsSource = ViewModel.Images;
            FreshLoading(Chapter, currentChapterIndex);
            DependencyService.Get<IStatusBar>()?.HideStatusBar();
        }

        public IChapter Chapter { get; set; }

        public ChapterViewModel ViewModel { get; set; }

        protected override bool OnBackButtonPressed()
        {
            DependencyService.Get<IStatusBar>()?.ShowStatusBar();
            return base.OnBackButtonPressed();
        }

        public void FreshLoading(IChapter chapter, int currentChapterIndex)
        {
            _currentChapterIndex = currentChapterIndex;
            ViewModel.Images.Clear();
            ViewModel.IsBusy = false;
            ViewModel.LoadedImages = 0;
            ViewModel.MaxCountOfImages = 0;
            ViewModel.ChangeChapter(chapter);
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
            if (string.IsNullOrEmpty(pageCount))
                return;

            Left = int.Parse(pageCount);
            ViewModel.MaxCountOfImages = Left;
            ViewModel.Images.Add(new ImageUrl(imageUrl));
            ViewModel.LoadedImages++;
            // var path = await DownloadImageAsync(new Uri(imageUrl));
            if (string.IsNullOrEmpty(imageUrl))
                return;
            var data = await new WebClient().DownloadDataTaskAsync(imageUrl);
            var fileService = DependencyService.Get<IFileService>();
            fileService.SavePicture($"{Chapter.ChapterName}_{0}.jpg", data, Chapter.Manga.MangaName);
            for (var i = 0; i < Left; i++)
                try
                {
                    await Browser?.EvaluateJavaScriptAsync("document.getElementById('mangaPicture').click()");
                    imageUrl = await Browser?.EvaluateJavaScriptAsync(
                        "Array.from(document.body.querySelectorAll('#mangaPicture')).map(a => a.src)[0];");
                    if (string.IsNullOrEmpty(imageUrl)) return;
                    data = await new WebClient().DownloadDataTaskAsync(imageUrl);
                    fileService?.SavePicture($"{Chapter.ChapterName}_{i + 1}.jpg", data, Chapter.Manga.MangaName);
                    ViewModel.Images.Add(new ImageUrl(imageUrl));
                    ViewModel.LoadedImages++;
                    await Task.Delay(15);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return;
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

        private async void ChangeChapter(ChapterChangeType type)
        {
            var nextChapterId = _currentChapterIndex + (type == ChapterChangeType.Back ? 1 : -1);
            var length = _chapters.Count;
            if (nextChapterId < 0 || length < nextChapterId)
            {
                await DisplayAlert("Info",
                    $"There is no more chapters. {_currentChapterIndex} -> {nextChapterId} [{type}]", "ok :(");
                return;
            }

            var nextChapter = _chapters[nextChapterId];
            FreshLoading(nextChapter, nextChapterId);
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

        private void CollectionView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            ButtonsLayout.IsVisible = e.LastVisibleItemIndex == ViewModel.MaxCountOfImages;
            // ButtonsLayout.IsVisible = true;
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            ChangeChapter(ChapterChangeType.Back);
        }

        private void NextButtonClick(object sender, EventArgs e)
        {
            ChangeChapter(ChapterChangeType.Next);
        }

        private enum ChapterChangeType
        {
            Back,
            Next
        }
    }
}