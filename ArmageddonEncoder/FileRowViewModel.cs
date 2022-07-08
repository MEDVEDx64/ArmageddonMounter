using DevExpress.Mvvm;
using System.Windows;
using System.Windows.Media;

namespace ArmageddonEncoder
{
    public class FileRowViewModel : ViewModelBase
    {
        ImageSource stateIcon = StateIcons.Pending;
        Visibility staticIconVis = Visibility.Visible;
        Visibility animatedIconVis = Visibility.Hidden;

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
    }
}
