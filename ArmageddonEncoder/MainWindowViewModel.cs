using ArmageddonEncoder.Encoders;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        public Visibility DragDropTextVisibility => Rows.Count == 0 ? Visibility.Visible : Visibility.Hidden;

        public DelegateCommand SelectDestinationFolderCommand { get; }
        public DelegateCommand ConvertToPngCommand { get; }
        public DelegateCommand ConvertToImgCommand { get; }

        public MainWindowViewModel() : base()
        {
            SelectDestinationFolderCommand = new DelegateCommand(SelectDestinationFolder);
            ConvertToPngCommand = new DelegateCommand(async () => await PerformConversion(new PngEncoder()));
            ConvertToImgCommand = new DelegateCommand(async () => await PerformConversion(new ImgEncoder()));

            Rows.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(DragDropTextVisibility));
        }

        void SelectDestinationFolder()
        {
        }

        void Reset()
        {
            foreach (var row in Rows)
            {
                row.StateIcon = StateIcons.Pending;
                row.StateIconToolTip = null;
            }
        }

        async ValueTask PerformConversion(IMediaEncoder enc)
        {
            if (!isConversionAllowed)
                return;

            isConversionAllowed = false;

            Reset();

            foreach(var row in Rows)
            {
                row.StateIcon = StateIcons.Processing;
                if (!enc.AcceptableExtensions.Contains(Path.GetExtension(row.FilePath)))
                {
                    row.StateIcon = StateIcons.Skipped;
                    continue;
                }

                try
                {
                    File.WriteAllBytes(Path.Combine(DestinationFolder,
                        Path.ChangeExtension(Path.GetFileName(row.FilePath), enc.TargetExtension)),
                        await enc.Encode(File.ReadAllBytes(row.FilePath)));

                    row.StateIcon = StateIcons.Success;
                }

                catch(Exception e)
                {
                    row.StateIcon = StateIcons.Error;
                    row.StateIconToolTip = e.GetType().ToString() + "\n\n" + e.Message;
                }
            }

            isConversionAllowed = true;
        }
    }
}
