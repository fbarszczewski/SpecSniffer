using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SpecSniffer.Model;

namespace Spec.Sniffer_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NetDrive.RemoveNetShare();
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var result = MessageBox.Show("Yes - Shutdown system.\n" +
                                             "No - Restart system.\n" +
                                             "Cancel - Close program.", "Closing", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                    Process.Start("shutdown", "/s /t 3");
                else if (result == MessageBoxResult.No)
                    Process.Start("shutdown", "/r /t 3");
                else if (result == MessageBoxResult.Cancel) CloseMainWindowNow();
            }
        }
        public static void CloseMainWindowNow()
        {
            //var mainWindow = (Application.Current.MainWindow as MainWindow);
            //if (mainWindow != null)
            //{
            //    Application.Current.Shutdown();
            //}
            Application.Current.Shutdown();

        }
    }
}