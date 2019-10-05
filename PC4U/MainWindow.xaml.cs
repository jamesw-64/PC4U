using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PC4U
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // gloabl developer_mode variable
        bool developer_mode = false;
        
        //initalisation logic
        public MainWindow(bool developer)
        {
            InitializeComponent();

            if(System.Windows.SystemParameters.PrimaryScreenWidth == 1366 && System.Windows.SystemParameters.PrimaryScreenHeight == 768)
            {
                WindowState = WindowState.Maximized;
            }

            // if we are a developer or debugger show the exit button and a message to let the user
            // know they are in debug mode
            if(developer)
            {
                dev_help.Visibility = Visibility.Visible;
                exit.Visibility = Visibility.Visible;
                developer_mode = true;
            }
        }

        //program close logic
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void loginNavButton_Click(object sender, EventArgs e)
        {
            LogIn LogInNav = new LogIn();
            LogInNav.Show();
        }

        private void browseNavButton_Click(object sender, EventArgs e)
        {
            Browse browseNav = new Browse();
            browseNav.Show();
        }

        private void helpNavButton_Click(object sender, EventArgs e)
        {
            HelpMenu helpMenu = new HelpMenu(developer_mode);
            helpMenu.Show();
        }

        private void registerNavButton_Click(object sender, EventArgs e)
        {
            Register registerNav = new Register();
            registerNav.Show();
        }
    }
}
