using System.Windows;

namespace ArmageddonMounter
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            new MainWindow(e.Args).Show();
        }
    }
}
