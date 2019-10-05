using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U_Admin
{
    /// <summary>
    /// Interaction logic for CreateEntry.xaml
    /// </summary>
    public partial class StockInfo : Window
    {
        Int64 ID_global;
        bool createMode_global;
        string ItemName_temp;

        private string isEmpty(string field)
        {
            throw new Exception(field + " not provided");
            return null;
        }

        public StockInfo(int? ID, bool createMode)
        {
            createMode_global = createMode;

            InitializeComponent();

            Title.Content = "Create Stock Entry";

            if (!createMode)
            {
                update.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string stm = "SELECT * FROM stock WHERE ItemID = '" + ID + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            rdr.Read();

                            Title.Content = (string)rdr["ItemName"] + " (Item ID: " + (Int64)rdr["ItemID"] + ")";
                             
                            ID_global = (Int64)rdr["ItemID"];

                            ItemName.Text = (string)rdr["ItemName"];
                            ItemName_temp = (string)rdr["ItemName"];
                            type.Text = (string)rdr["Type"];
                            Price.Text = (string)rdr["Price"];
                            RAMSize.Text = (string)rdr["RAMSize"];
                            Brand.Text = (string)rdr["Brand"];
                            Model.Text = (string)rdr["Model"];
                            OS.Text = (string)rdr["OS"];
                            Processor.Text = (string)rdr["Processor"];
                            Graphics.Text = (string)rdr["Graphics"];
                            HDD.Text = (string)rdr["HDD"];
                            Bluetooth.IsChecked = (Int64)rdr["Bluetooth"] == 1 ? true : false;
                            WiFi.IsChecked = (Int64)rdr["WiFi"] == 1 ? true : false;
                            Details.Text = (string)rdr["Details"];

                            update.Content = "Update all items with item name " + ItemName_temp + " in database";
                        }
                    }

                    Int64 ClientID = 0;

                    stm = "SELECT * FROM purchases WHERE ItemID = '" + ID + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            rdr.Read();
                            if (rdr.HasRows)
                            {
                                holdInfo.Visibility = Visibility.Visible;
                                deleteButton.Visibility = Visibility.Hidden;
                                if ((Int64)rdr["Purchased"] == 0)
                                {
                                    holdInfo.Content = "Item is currently reserved by ";
                                }
                                if ((Int64)rdr["Purchased"] == 1)
                                {
                                    holdInfo.Content = "Item has been purchased by ";
                                }
                                ClientID = (Int64)rdr["ClientID"];
                            }
                        }
                    }

                    if (ClientID != 0)
                    {
                        stm = "SELECT * FROM users WHERE ClientID = '" + ClientID + "'";
                        using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                        {
                            using (SQLiteDataReader rdr = cmd.ExecuteReader())
                            {
                                rdr.Read();
                                holdInfo.Content = holdInfo.Content + (string)rdr["FirstName"] + " " + (string)rdr["LastName"] + " (ID: " + ClientID + ")";
                            }
                        }
                    }
                }
            }
        }

        private void commit(object sender, RoutedEventArgs e)
        {
            //vaildation 
            try
            {
                var itemName = string.IsNullOrEmpty(ItemName.Text) ? isEmpty("Item name") : ItemName.Text;
                var type_filter = string.IsNullOrEmpty(type.Text) ? isEmpty("Type") : type.Text;
                var price = string.IsNullOrEmpty(Price.Text) ? isEmpty("Price") : Price.Text;
                var ram_size = string.IsNullOrEmpty(RAMSize.Text) ? isEmpty("RAM Size") : RAMSize.Text;
                var brand = string.IsNullOrEmpty(Brand.Text) ? isEmpty("Brand") : Brand.Text;
                var model = string.IsNullOrEmpty(Model.Text) ? isEmpty("Model") : Model.Text;
                var os = string.IsNullOrEmpty(OS.Text) ? isEmpty("OS") : OS.Text;
                var processor = string.IsNullOrEmpty(Processor.Text) ? isEmpty("Processor") : Processor.Text;
                var graphics = string.IsNullOrEmpty(Graphics.Text) ? isEmpty("Graphics") : Graphics.Text;
                var hdd = string.IsNullOrEmpty(HDD.Text) ? isEmpty("HDD size") : HDD.Text;
                var bluetooth = (bool)Bluetooth.IsChecked ? 1 : 0;
                var wifi = (bool)WiFi.IsChecked ? 1 : 0;
                var details = string.IsNullOrEmpty(Details.Text) ? isEmpty("Details") : Details.Text;


                if (createMode_global)
                {
                    using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                    {
                        cnn.Open();
                        string query = "INSERT INTO stock (ItemName, Type, Price, RAMSize, Brand, Model, OS, Processor, Graphics, HDD, Bluetooth, WiFi, Details, inStock) VALUES ('" + itemName + "', '" + type_filter + "', '" + price + "', '" + ram_size + "', '" + brand + "', '" + model + "', '" + os + "', '" + processor + "', '" + graphics + "', '" + hdd + "', '" + bluetooth + "', '" + wifi + "', '" + details.Replace("'", "''") + "', '1')";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        cnn.Close();

                        MessageBox.Show("Operation finished!", "Ok!", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {
                    using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                    {
                        cnn.Open();
                        string query = "UPDATE stock SET ItemName = '" + itemName + "', Type = '" + type_filter + "', Price = '" + price + "', RAMSize = '" + ram_size + "', Brand = '" + brand + "', Model = '" + model + "', OS = '" + os + "', Processor = '" + processor +"', Graphics = '" + graphics + "', HDD = '" + hdd + "', Bluetooth = '" + bluetooth + "', Details = '" + details.Replace("'", "''") + "' WHERE ItemID = '" + ID_global + "'";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        cnn.Close();

                        MessageBox.Show("Operation finished!", "Ok!", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
            }

            catch (Exception Ex)
            {
                if (createMode_global)
                {
                    MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error creating new entry!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error updating entry!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void updateAll(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to change all entries with the item name of " + ItemName_temp + "? This action effect all entries, both purchased and in stock. This action cannot be undone!",
                    "Caution",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    //vaildation 
                    try
                    {
                        var itemName = string.IsNullOrEmpty(ItemName.Text) ? isEmpty("Item name") : ItemName.Text;
                        var type_filter = string.IsNullOrEmpty(type.Text) ? isEmpty("Type") : type.Text;
                        var price = string.IsNullOrEmpty(Price.Text) ? isEmpty("Price") : Price.Text;
                        var ram_size = string.IsNullOrEmpty(RAMSize.Text) ? isEmpty("RAM Size") : RAMSize.Text;
                        var brand = string.IsNullOrEmpty(Brand.Text) ? isEmpty("Brand") : Brand.Text;
                        var model = string.IsNullOrEmpty(Model.Text) ? isEmpty("Model") : Model.Text;
                        var os = string.IsNullOrEmpty(OS.Text) ? isEmpty("OS") : OS.Text;
                        var processor = string.IsNullOrEmpty(Processor.Text) ? isEmpty("Processor") : Processor.Text;
                        var graphics = string.IsNullOrEmpty(Graphics.Text) ? isEmpty("Graphics") : Graphics.Text;
                        var hdd = string.IsNullOrEmpty(HDD.Text) ? isEmpty("HDD size") : HDD.Text;
                        var bluetooth = (bool)Bluetooth.IsChecked ? 1 : 0;
                        var wifi = (bool)WiFi.IsChecked ? 1 : 0;
                        var details = string.IsNullOrEmpty(Details.Text) ? isEmpty("Details") : Details.Text;

                        using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                        {
                            cnn.Open();
                            string query = "UPDATE stock SET ItemName = '" + itemName + "', Type = '" + type_filter + "', Price = '" + price + "', RAMSize = '" + ram_size + "', Brand = '" + brand + "', Model = '" + model + "', OS = '" + os + "', Processor = '" + processor + "', Graphics = '" + graphics + "', HDD = '" + hdd + "', Bluetooth = '" + bluetooth + "', Details = '" + details.Replace("'", "''") + "', inStock = '1' WHERE ItemName = '" + ItemName_temp + "'";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            cnn.Close();

                            MessageBox.Show("Operation finished!", "Ok!", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                    }
                    catch (Exception Ex)
                    {
                        if (createMode_global)
                        {
                            MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error creating new entry!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error updating entry!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("All items updated", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Operation canceled, no data was modified", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this entry? Data may still be recoverable using SQLite data recovery methods until a vacuum is preformed. (NB: this software suite does not supprot data recovery, an external program will be needed if you wish to preform data recovery)",
                    "Caution",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();

                    string stm = "DELETE FROM stock WHERE ItemID = '" + ID_global + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Operation complete", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    Close();
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Operation canceled, no data was modified", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
