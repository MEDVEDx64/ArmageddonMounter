using System.Windows;

namespace ArmageddonEncoder
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnDataGridDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.All;
        }

        private void OnDataGridDrop(object sender, DragEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;

            try
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach(var f in files)
                {
                    vm.Rows.Add(new FileRowViewModel(f));
                }
            }

            catch { }
        }
    }
}
