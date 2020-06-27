using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using JMangaReader.ScrapperEngine.Interface;
using JMangaReader.Services;
using JMangaReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JMangaReader.Views
{
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

            var historyService = DependencyService.Get<IHistory>();
            historyService.AddChapterToHistory(chapter);
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
                    await DisplayAlert("Ошибка", $"{exception.Message}", "ok");
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