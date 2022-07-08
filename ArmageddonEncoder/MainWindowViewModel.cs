using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ArmageddonEncoder
{
    public class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty] public string DestinationFolder;

        public ObservableCollection<FileRowViewModel> Rows { get; } = new ObservableCollection<FileRowViewModel>();

        [ICommand] public void SelectDestinationFolder()
        {
        }
    }
}
