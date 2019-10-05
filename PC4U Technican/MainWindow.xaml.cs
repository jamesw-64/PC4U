using System.Windows;
using System.Windows.Input;

namespace PC4U_Technican
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

        private void exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lookup_client(object sender, RoutedEventArgs e)
        {
            var Lookup = new UserLookup();
            Lookup.Show();
        }

        private void see_unfinished(object sender, RoutedEventArgs e)
        {
            UnfinishedRepairs Unfinished = new UnfinishedRepairs();
            Unfinished.Show();
        }

        private void create_entry(object sender, RoutedEventArgs e)
        {teEntry createEntry = new CreateEntry();
            createEntry.Show();
        }
    }
}
