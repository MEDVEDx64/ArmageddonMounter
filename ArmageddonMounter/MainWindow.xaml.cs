using DokanNet;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace ArmageddonMounter
{
    enum UnmountingPhase
    {
        NotInitiated,
        Initiated,
        Shutdown,
    }

    public partial class MainWindow : Window
    {
        DirFS fs;
        UnmountingPhase unmountingPhase = UnmountingPhase.NotInitiated;

        readonly char DRIVE_LETTER = 'w';

        void Panic(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(-1);
        }

        public MainWindow(string[] args)
        {
            if(args.Length < 1)
            {
                Panic("Please drop a .dir file into this application.");
            }

#if !DEBUG
            try
            {
#endif
                fs = new DirFS(args[0]);

                new Thread(() =>
                {
                    fs.Mount(DRIVE_LETTER + ":\\", DokanOptions.StderrOutput);
                    Dispatcher.Invoke(() =>
                    {
                        unmountingPhase = UnmountingPhase.Shutdown;
                        Close();
                    });
                }).Start();
#if !DEBUG
            }

            catch(Exception e)
            {
                Panic(e.ToString());
            }
#endif

            InitializeComponent();

            pathRow.Text = args[0];
        }

        private void OnActivated(object sender, EventArgs e)
        {
            // This code fits the path text line into the window width
            // when the path is too long
            new Thread(() =>
            {
                bool exit = false;
                string cut = null;

                Dispatcher.Invoke(() =>
                {
                    cut = pathRow.Text;
                });

                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if(pathRow.ActualWidth > Width - 40)
                        {
                            cut = cut.Substring(1);
                            pathRow.Text = "..." + cut;
                        }
                        else
                        {
                            exit = true;
                        }
                    });

                    Thread.Sleep(1);
                    if(exit) break;
                }
            }).Start();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (unmountingPhase != UnmountingPhase.NotInitiated)
            {
                if (unmountingPhase == UnmountingPhase.Initiated)
                    e.Cancel = true;

                return;
            }

            if (Dokan.Unmount(DRIVE_LETTER))
            {
                e.Cancel = true;
                unmountingPhase = UnmountingPhase.Initiated;

                pathRow.Opacity = 0;
                messageRow.Text = "Unmounting...";
                saveButton.Opacity = 0;
                saveButton.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Due to some reason, unmounting wasn't successful.\n\n"
                    + "The application will be TERMINATED right after the OK button is pressed.\n\nBye.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        private void AnimateSaveFader()
        {
            saveFader.Opacity = 0.75;
            saveButton.IsEnabled = false;
            saveButton.Width = saveButton.ActualWidth;
            
            var btnText = saveButton.Content;
            saveButton.Content = "Success!";

            new Thread(() =>
            {
                bool exit = false;

                while(true)
                {
                    if (exit)
                        break;

                    Thread.Sleep(12);
                    Dispatcher.Invoke(() =>
                    {
                        saveFader.Opacity -= 0.05;
                        if (saveFader.Opacity < 0)
                        {
                            saveButton.Content = btnText;
                            saveButton.IsEnabled = true;
                            saveButton.Width = double.NaN;
                            exit = true;
                        }
                    });
                }
            }).Start();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            if(fs.Save() == DokanResult.Success)
            {
                AnimateSaveFader();
            }
            else
            {
                MessageBox.Show("Saving failed due to I/O error.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
