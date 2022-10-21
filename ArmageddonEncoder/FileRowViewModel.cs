using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace ArmageddonEncoder
{
    public partial class FileRowViewModel : ObservableObject
    {
        ImageSource stateIcon = StateIcons.Pending;

        [ObservableProperty] Visibility staticStateIconVisibility = Visibility.Visible;
        [ObservableProperty] Visibility animatedStateIconVisibility = Visibility.Hidden;
        [ObservableProperty] string? stateIconToolTip;
        [ObservableProperty] string fileName;

        public ImageSource StateIcon
        {
            get => stateIcon;
            set
            {
                stateIcon = value;
                OnPropertyChanged(nameof(StateIcon));

                StaticStateIconVisibility = value == StateIcons.Processing ? Visibility.Hidden : Visibility.Visible;
                AnimatedStateIconVisibility = value == StateIcons.Processing ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public string FilePath { get; }

        public FileRowViewModel(string path)
        {
            FilePath = path;
            fileName = Path.GetFileName(path);
        }
    }
}
