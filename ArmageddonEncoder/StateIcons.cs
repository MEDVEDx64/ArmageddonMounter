using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArmageddonEncoder
{
    public static class StateIcons
    {
        public static readonly ImageSource Pending = new BitmapImage(new Uri("pack://application:,,,/ArmageddonEncoder;component/Graphics/entity_pending.png"));
        public static readonly ImageSource Processing = new BitmapImage(new Uri("pack://application:,,,/ArmageddonEncoder;component/Graphics/entity_processing.png"));
        public static readonly ImageSource Success = new BitmapImage(new Uri("pack://application:,,,/ArmageddonEncoder;component/Graphics/entity_success.png"));
        public static readonly ImageSource Skipped = new BitmapImage(new Uri("pack://application:,,,/ArmageddonEncoder;component/Graphics/entity_skipped.png"));
        public static readonly ImageSource Error = new BitmapImage(new Uri("pack://application:,,,/ArmageddonEncoder;component/Graphics/entity_error.png"));
    }
}
