using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U
{
    /// <summary>
    /// Interaction logic for ItemPage.xaml
    /// </summary>
    public partial class ItemPage : Window
    {
        // initalisation of global variables so that they can be used across methods
        Int64 ID_global = 0;
        string ItemName_global;

        // window initalisation logic
        public ItemPage()
        {
            InitializeComponent();

            if (System.Windows.SystemParameters.PrimaryScreenWidth == 1366 && System.Windows.SystemParameters.PrimaryScreenHeight == 768)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void backArrow_Click(object sender, EventArgs e)
        {
            // closing this open window to go back to main menu
            this.Close();
        }

        // logic for reading in system infromation
        public void show_system_info(Int64 ID)
        {
            // setting the global ID as the ID passed into this method, so we can use it later
            ID_global = ID;
            
            // initalisation of SQLiteConnection
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();

                // putting query as a variable to increase readability
                string stm = "SELECT  *  FROM stock  WHERE ItemID = '" + ID + "'";

                // Query the database
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    
                    // read the input
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        //set the title bar and window title as device name
                        mainBanner.Content = (string)rdr["Brand"] + " " + (string)rdr["ItemName"] + " (ID: " + ID + ")";

                        this.Title = (string)rdr["Brand"] + " " + (string)rdr["ItemName"] + " (ID: " + ID + ")";

                        // Fill in the infromation for the device
                        brand_name.Content = (string)rdr["Brand"];
                        itemname_name.Content = (string)rdr["ItemName"];
                        ItemName_global = (string)rdr["ItemName"];
                        type_name.Content = (string)rdr["Type"];
                        model_name.Content = (string)rdr["Model"];

                        processor_name.Content = (string)rdr["Processor"];
                        os_name.Content = (string)rdr["OS"];
                        hdd_name.Content = (string)rdr["HDD"];
                        ram_name.Content = (string)rdr["RAMSize"];

                        details_name.Text = (string)rdr["Details"];

                        price_name.Content = (string)rdr["Price"];

                        graphics_name.Content = (string)rdr["Graphics"];

                        bluetooth_bool.Content = (Int64)rdr["Bluetooth"] == 1 ? "Yes" : "No";
                        wifi_bool.Content = (Int64)rdr["WiFi"] == 1 ? "Yes" : "No";

                        if ((Int64)rdr["inStock"] == 0)
                        {
                            Buy.Visibility = Visibility.Hidden;
                        }
                    }
                }
                cnn.Close();
            }
            this.Show();
        }

        private void buy_Click(object sender, RoutedEventArgs e)
        {
            LogIn login_nav = new LogIn();
            login_nav.setBuyFlag(1, ID_global, ItemName_global);
            this.Close();
        }
    }
}
