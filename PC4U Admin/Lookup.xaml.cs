using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U_Admin
{
    /// <summary>
    /// Interaction logic for Lookup.xaml. This is the small prompt window that appears when you
    /// hold shift on a supported button on the main menu. It is all done in one file to prevent
    /// confusion. The great part of this is that if you know the id number this allows you to 
    /// directly access the id saving the user a couple of clicks
    /// </summary>
    public partial class Lookup : Window
    {
        Int64 ID = 0;
        int mode_global;
        string id_type;
        string table;

        public Lookup(int mode)
        {
            InitializeComponent();

            mode_global = mode;

            // select which mode the window will be in
            switch (mode)
            {
                case 0: id_type = "Client"; table = "users"; break;
                case 1: id_type = "Item"; table = "stock"; break;
                case 2: id_type = "Repair"; table = "repairs"; break;
                
                // case 3 is different to the other cases in that there is no window to be opened
                // with this one
                case 3: id_type = "Duplicate"; title.Content = "Duplicate ID"; SearchButton.Content = "Duplicate"; table = "stock"; return;
                case 4: id_type = "Client"; title.Content = "Look up user"; table = "users"; return;
            }

            title.Content = id_type + "ID LOOKUP:";
        }

        private void search(object sender, RoutedEventArgs e)
        {
            // validation of input
            try
            {
                ID = Convert.ToInt64(Input.Text);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(Input.Text))
                {
                    MessageBox.Show("Input cannot be empty!",
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MessageBox.Show("Input contains non-number characters!",
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                return;
            }
            
            // prepare databas
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm;
                
                // if the 3rd case was selected then we will do this rather than the other logic
                // below
                if(id_type == "Duplicate")
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to create a duplicate of this ID? This will create a copy of the ID and will place it make it in stock.",
                   "Caution",
                   MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

                    try
                    {
                        if (result == MessageBoxResult.Yes)
                        {
                            // get the ID
                            stm = "SELECT * FROM stock WHERE ItemID = '" + ID + "'";
                            using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                            {
                                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                                {
                                    if(!rdr.HasRows)
                                    {
                                        MessageBox.Show("ID does not exist.",
                   "Error",
                   MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Cancel);
                                        return;
                                    }
                                }
                            }

                            // duplicate the item using the exsisting information
                            stm = "INSERT INTO stock (ItemName, Type, Price, RAMSize, Brand, Model, OS, Processor, Graphics, HDD, Bluetooth, WiFi, inStock, Details) SELECT ItemName, Type, Price, RAMSize, Brand, Model, OS, Processor, Graphics, HDD, Bluetooth, WiFi, '1', Details FROM stock WHERE stock.ItemID = '" + ID + "'";
                            using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            MessageBox.Show("Operation canceled, database was not modified", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        MessageBox.Show("Operation complete", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                        
                        // return this method so that we don't continue with the logic below
                        return;
                    }
                    catch(Exception Ex)
                    {
                        // TODO: clean this error up
                        MessageBox.Show("Error: " + Ex,
                   "Error",
                   MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Cancel);
                    }
                }
                
                // a multi-purpose query which can react to whatever case is selected
                stm = "SELECT * FROM " + table + " WHERE " + id_type + "ID = '" + ID + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            //open the respective windows
                            switch (mode_global)
                            {
                                case 0:
                                    ClientInfo clientLoader = new ClientInfo((int)ID);
                                    clientLoader.Login();
                                    this.Close();
                                    break;
                                case 1:
                                    StockInfo stockLoader = new StockInfo((int)ID, false);
                                    stockLoader.Show();
                                    this.Close();
                                    break;
                                case 2:
                                    RepairInfo repairLoader = new RepairInfo((int)ID);
                                    repairLoader.Show();
                                    this.Close();
                                    break;
                                case 4:
                                    CreateOrders orderLoader = new CreateOrders((int)ID);
                                    orderLoader.Show();
                                    this.Close();
                                    break;
                            }
                        }
                        else
                        {
                            // tell the user that ID doesn't exsist
                            MessageBoxResult result = MessageBox.Show("Unable to find " + id_type.ToLowerInvariant() + " from provided ID!",
                            "Alert",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                }
            }
        }
    }
}
