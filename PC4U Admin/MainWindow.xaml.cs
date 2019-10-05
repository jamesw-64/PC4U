using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace PC4U_Admin
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

        private void search_customer(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Lookup direct_lookup = new Lookup(0);
                direct_lookup.Show();
            }
            else
            {
                SearchCustomer search = new SearchCustomer();
                search.Show();
            }
        }

        private void create_customer(object sender, RoutedEventArgs e)
        {
            CreateCustomer create = new CreateCustomer();
            create.Show();
        }

        private void search_stock(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Lookup direct_lookup = new Lookup(1);
                direct_lookup.Show();
            }
            else
            {
                SearchStock search = new SearchStock();
                search.Show();
            }
        }

        private void create_stock(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Lookup direct_lookup = new Lookup(3);
                direct_lookup.Show();
            }
            else
            {
                StockInfo create = new StockInfo(null, true);
                create.Show();
            }
        }

        private void search_orders(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Lookup direct_lookup = new Lookup(2);
                direct_lookup.Show();
            }
            else
            {
                SearchOrders search = new SearchOrders();
                search.Show();
            }
        }

        private void create_orders(object sender, RoutedEventArgs e)
        {
            Lookup direct_lookup = new Lookup(4);
            direct_lookup.Show();
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        // vaccuming the database can help reduce its file size if data has been deleted via the 
        // DELETE query. Useful if we are cleaning up after say a spam attack where a user tries
        // to mass reserve items. This is also why we have a "release all items" method in the 
        // search order window: to protect against spam attacks. And also to release unreserved
        // items at the end of the day like the reserve prompt from the kisok system says.
        private void vaccumm(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to vacuum the database? This will reduce the size of the database, but any previously deleted data will no longer be recoverable.",
                    "Caution",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    // simple enough. Probably the easiest thing in this entire project...
                    string stm = "VACUUM";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Operation complete", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Operation canceled, no data was modified", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
