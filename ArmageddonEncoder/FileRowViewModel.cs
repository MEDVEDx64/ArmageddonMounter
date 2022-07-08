using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ArmageddonEncoder
{
    public class FileRowViewModel : ObservableObject
    {
        ImageSource stateIcon = StateIcons.Pending;
        Visibility staticIconVis = Visibility.Visible;
        Visibility animatedIconVis = Visibility.Hidden;

        public ImageSource StateIcon
        {
            get => stateIcon;
            set
            {
                SetProperty(ref stateIcon, value);

                StaticStateIconVisibility = value == StateIcons.Processing ? Visibility.Hidden : Visibility.Visible;
                AnimatedStateIconVisibility = value == StateIcons.Processing ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility StaticStateIconVisibility
        {
            get => staticIconVis;
            set => SetProperty(ref staticIconVis, value);
        }

        public Visibility AnimatedStateIconVisibility
        {
            get => animatedIconVis;
            set => SetProperty(ref animatedIconVis, value);
        }
    }
}
