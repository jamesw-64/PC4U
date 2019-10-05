using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PC4U_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Readme_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", "bin/readme.txt");
        }

        private void Kiosk_Click(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory() + "\\bin\\Kiosk\\PC4U.exe";
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Process.Start(path, "-debug");
                System.Environment.Exit(0);
            }

            Process.Start(path);
            System.Environment.Exit(0);
        }

        private void Technician_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Directory.GetCurrentDirectory() + "\\bin\\Technician\\PC4U Technican.exe");
            System.Environment.Exit(0);
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Directory.GetCurrentDirectory() + "\\bin\\Admin\\PC4U Admin.exe");
            System.Environment.Exit(0);
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This opeartion creates the hard links between the database instances with the individual exectutables and the main data.db database in the parent directory of the systems. If you DO NOT wish this to happen, do not click yes but bear in mind that the system may not work as intended if you do this. If you click yes a command prompt window will appear and create the links.\n\nAre you sure you want to continue?",
                    "Caution",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start("mklink.bat");
            }
        }
    }
}
