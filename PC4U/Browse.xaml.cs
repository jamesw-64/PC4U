using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U
{
    /// <summary>
    /// Interaction logic for Browse.xaml
    /// </summary>
    public partial class Browse : Window
    {
        // Define the class for use in the ListView. Every Item in the ListView is an object from
        // this class
        class Device
        {
            public Int64  ID       { get; set; }
            public string ItemName { get; set; }
            public string Type     { get; set; }
            public string Brand    { get; set; }
            public string Model    { get; set; }
            public string HDDSize  { get; set; }
        } 

        // Initalisation logic for this window
        public Browse()
        {
            InitializeComponent();

            if (System.Windows.SystemParameters.PrimaryScreenWidth == 1366 && System.Windows.SystemParameters.PrimaryScreenHeight == 768)
            {
                WindowState = WindowState.Maximized;
            }
        }

        // For closing this window when backwards navigation occurs
        private void backArrow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Seach logic
        private void search_Click(object sender, EventArgs e)
        {
            // Clear the list view so added items don't get appended onto the end of the current 
            // list in order to reduce confusion
            AllInfo.Items.Clear();
            
            // Hide the error and help messages, incase there was a problem last time or this is
            // the first run of this instance of "Browse.cs"
            help_text.Visibility = Visibility.Hidden;
            empty_text.Visibility = Visibility.Hidden;
            
            // Initalise the SQLiteConnection (in using so if the program crashes we don't corrupt
            // the database
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                // Open the database
                cnn.Open();
                
                // this is here for optimisation, so the program does not have to qurey the window
                // every time we need to access what filter we are using
                string selected_filter = search_filter.Text;

                // create the qurey in a variable so be can increase readability and also 
                // manipulate it based on our current settings
                string stm = "SELECT  *  FROM stock  WHERE ItemName LIKE '%" + SearchTerm.Text + "%'";
                if (brand_search_check.IsChecked == true)
                {
                    stm = "SELECT  *  FROM stock  WHERE  Brand LIKE '%" + SearchTerm.Text + "%'";
                }
                if (selected_filter != "")
                {
                    stm = stm + " AND Type = '" + selected_filter + "'";
                }
                
                // Query the database
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    // Read the result from the query
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        // If there are no results from the query...
                        if (rdr.HasRows == false)
                        {
                            // show the "no results" error
                            empty_text.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // this allows the program to loop through each result of the query
                            while (rdr.Read())
                            {
                                if ((Int64)rdr["inStock"] == 1)
                                {
                                    // add the item to the list view if its in stock flag is set to
                                    // true
                                    AllInfo.Items.Add(new Device()
                                    {
                                        ID = (Int64)rdr["ItemID"],
                                        ItemName = (string)rdr["ItemName"],
                                        Type = (string)rdr["Type"],
                                        Brand = (string)rdr["Brand"],
                                        Model = (string)rdr["Model"],
                                        HDDSize = (string)rdr["HDD"]
                                    });
                                }
                            }
                        }
                    }
                }    
                // close the connection to the database as good practice
                cnn.Close();
            }
        }

        // logic for the select button click
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            // In a try/catch statement as if this fails it could cause the program to crash
            try
            {
                // instanciate the next window
                ItemPage next_nav = new ItemPage();
                
                // get the currently selected item
                Device selected_id = (Device)AllInfo.SelectedItems[0];
                
                // get the ID field from the currently selected itemm
                next_nav.show_system_info((Int64)selected_id.ID);
                
                // clear the list incase the user returns here and they purchased the item, as we 
                // won't know. Prevents double reservations.
                AllInfo.Items.Clear();
                
                // make it seem as if the window has been restarted when they return by showing the
                // the help text
                help_text.Visibility = Visibility.Visible;
            }
            catch (Exception Ex)
            {
                // the only way this should crash is if there is no selected item in the list, so
                // show this error
                MessageBox.Show("You must have selected an item before proceeding! Error: " + Ex + ". Actual Error Message: " + Ex.Message,  "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
