using DokanNet;
using Syroot.Worms;
using System;
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
                fs.Mount("w:\\", DokanOptions.StderrOutput);
            }

            catch(Exception e)
            {
                Panic(e.ToString());
            }

            InitializeComponent();
        }
    }
}
