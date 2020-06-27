using System.Collections.ObjectModel;
using System.ComponentModel;
using JMangaReader.ScrapperEngine.Interface;

namespace JMangaReader.ViewModels
{
    public class MangaListModel: INotifyPropertyChanged
    {
        public bool IsBusy { get; set; }
        public ObservableCollection<IManga> MangaList { get; set; } = new ObservableCollection<IManga>();
        public bool IsErrorMessageVisible { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}