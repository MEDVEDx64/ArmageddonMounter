using DevExpress.Mvvm;
using System.Collections.ObjectModel;

namespace ArmageddonEncoder
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<FileRowViewModel> Rows { get; } = new ObservableCollection<FileRowViewModel>();
    }
}
