using DokanNet;
using System;
using System.Threading;
using System.Windows;

namespace ArmageddonMounter
{
    public partial class MainWindow : Window
    {
        DirFS fs;

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

            try
            {
                fs = new DirFS(args[0]);

                new Thread(() =>
                {
                    fs.Mount("w:\\", DokanOptions.StderrOutput);
                }).Start();
            }

            catch(Exception e)
            {
                Panic(e.ToString());
            }

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
    }
}
