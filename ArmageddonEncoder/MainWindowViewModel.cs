using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace ArmageddonEncoder
{
    public class MainWindowViewModel : ViewModelBase
    {
        string destFolder = ".\\";
        bool isConversionAllowed = true;

        public string DestinationFolder
        {
            get => destFolder;
            set
            {
                destFolder = value;
                RaisePropertyChanged(nameof(DestinationFolder));
            }
        }

        public ObservableCollection<FileRowViewModel> Rows { get; } = new ObservableCollection<FileRowViewModel>();

        public DelegateCommand SelectDestinationFolderCommand { get; }
        public DelegateCommand ConvertToPngCommand { get; }
        public DelegateCommand ConvertToImgCommand { get; }

        public MainWindowViewModel() : base()
        {
            SelectDestinationFolderCommand = new DelegateCommand(SelectDestinationFolder);
            ConvertToPngCommand = new DelegateCommand(ConvertToPng);
            ConvertToImgCommand = new DelegateCommand(ConvertToImg);
        }

        void SelectDestinationFolder()
        {
        }

        void Reset()
        {
            foreach (var row in Rows)
            {
                row.StateIcon = StateIcons.Pending;
            }
        }

        void PerformConversion(Action<FileRowViewModel> convAct)
        {
            if (!isConversionAllowed)
                return;

            isConversionAllowed = false;

            Reset();

            foreach(var row in Rows)
            {
                convAct(row);
            }

            isConversionAllowed = true;
        }

        void ConvertToPng()
        {
        }

        void ConvertToImg()
        {
        }
    }
}
