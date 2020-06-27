using System.Collections.ObjectModel;
using System.ComponentModel;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.ViewModels
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
}