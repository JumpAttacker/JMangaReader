using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using JMangaReader.ScrapperEngine.Interface;
using Xamarin.Forms;

namespace JMangaReader.ViewModels
{
    public class ChapterSelectorViewModel : INotifyPropertyChanged
    {
        public ChapterSelectorViewModel(IManga manga, List<IChapter> selectedMangaChapters)
        {
            Manga = manga;
            Chapters.Clear();
            foreach (var chapter in selectedMangaChapters) Chapters.Add(chapter);
        }

        public IManga Manga { get; }
        public ObservableCollection<IChapter> Chapters { get; set; } = new ObservableCollection<IChapter>();

        public IOrderedEnumerable<IChapter> GetChapters => IsSort
            ? Chapters.OrderByDescending(x => x.Id)
            : Chapters.OrderBy(x => x.Id);

        public string ChapterCountText => $"Total chapters: {Chapters.Count}";
        public bool IsFavorite { get; set; } = false;
        public bool IsSort { get; set; } = false;
        public bool IsLoading { get; set; } = false;
        public string FavoriteIcon => IsFavorite ? "favorite.png" : "not_favorite.png";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}