using DevExpress.Mvvm;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace ArmageddonEncoder
{
    public class FileRowViewModel : ViewModelBase
    {
        ImageSource stateIcon = StateIcons.Pending;
        Visibility staticIconVis = Visibility.Visible;
        Visibility animatedIconVis = Visibility.Hidden;
        string? iconToolTip;
        string fileName;

        public ImageSource StateIcon
        {
            get => stateIcon;
            set
            {
                stateIcon = value;
                RaisePropertyChanged(nameof(StateIcon));

                StaticStateIconVisibility = value == StateIcons.Processing ? Visibility.Hidden : Visibility.Visible;
                AnimatedStateIconVisibility = value == StateIcons.Processing ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility StaticStateIconVisibility
        {
            get => staticIconVis;
            set
            {
                staticIconVis = value;
                RaisePropertyChanged(nameof(StaticStateIconVisibility));
            }
        }

        public Visibility AnimatedStateIconVisibility
        {
            get => animatedIconVis;
            set
            {
                animatedIconVis = value;
                RaisePropertyChanged(nameof(AnimatedStateIconVisibility));
            }
        }

        public string? StateIconToolTip
        {
            get => iconToolTip;
            set
            {
                iconToolTip = value;
                RaisePropertyChanged(nameof(StateIconToolTip));
            }
        }

        public string FilePath { get; }
        public string FileName => fileName;

        public FileRowViewModel(string path)
        {
            FilePath = path;
            fileName = Path.GetFileName(path);
        }
    }
}
