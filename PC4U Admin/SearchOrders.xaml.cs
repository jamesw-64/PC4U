using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U_Admin
{
    /// <summary>
    /// Interaction logic for SearchOrders.xaml
    /// </summary>
    public partial class SearchOrders : Window
    {
        class Repair
        {
            public Int64 ClientID { get; set; }
            public Int64 IssueID { get; set; }
            public string Problem { get; set; }
        }

        class Order
        {
            public Int64 ClientID { get; set; }
            public Int64 ItemID { get; set; }
            public string ItemName { get; set; }
        }

        public SearchOrders()
        {
            InitializeComponent();
        }

        //Called when the button for select repair is pressed
        private void searchRepair(object sender, RoutedEventArgs e)
        {
            RepairTable.Items.Clear();
            if(repairSearchBox.Text == "")
            {
                MessageBox.Show("You did not input a query!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();

                string stm = "SELECT  *  FROM repairs WHERE ClientID = '" + repairSearchBox.Text + "'";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    try
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if ((Int64)rdr["Finished"] == 0)
                                {
                                    RepairTable.Items.Add(new Repair()
                                    {
                                        ClientID = (Int64)rdr["ClientID"],
                                        IssueID = (Int64)rdr["RepairID"],
                                        Problem = (string)rdr["Issue"]
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Unable to find any results with provided ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                cnn.Close();
            }
        }

        //override for the method above
        private void searchAllRepair()
        {
            RepairTable.Items.Clear();
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();

                string stm = "SELECT  *  FROM repairs";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            if ((Int64)rdr["Finished"] == 0)
                            {
                                RepairTable.Items.Add(new Repair()
                                {
                                    ClientID = (Int64)rdr["ClientID"],
                                    IssueID = (Int64)rdr["RepairID"],
                                    Problem = (string)rdr["Issue"]
                                });
                            }
                        }
                    }
                }
                cnn.Close();
            }
        }

        //called when button is pressed
        private void searchOrder(object sender, RoutedEventArgs e)
        {
            if (orderSearchBox.Text == "")
            {
                MessageBox.Show("You did not input a query!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            OrderTable.Items.Clear();
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();

                string stm = "SELECT  *  FROM purchases WHERE ClientID = '" + orderSearchBox.Text + "'";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            if ((Int64)rdr["Purchased"] == 0)
                            {
                                OrderTable.Items.Add(new Order()
                                {
                                    ClientID = (Int64)rdr["ClientID"],
                                    ItemName = (string)rdr["ItemName"]
                                });
                            }
                        }
                    }
                }
                cnn.Close();
            }
        }

        //override
        private void searchOrder()
        {
            OrderTable.Items.Clear();
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();

                string stm = "SELECT  *  FROM purchases ";
                if(orderSearchBox.Text != "")
                {
                    stm = stm + "WHERE ClientID = '" + orderSearchBox.Text + "'";
                }

                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            if ((Int64)rdr["Purchased"] == 0)
                            {
                                OrderTable.Items.Add(new Order()
                                {
                                    ClientID = (Int64)rdr["ClientID"],
                                    ItemID = (Int64)rdr["ItemID"],
                                    ItemName = (string)rdr["ItemName"]
                                });
                            }
                        }
                    }
                }
                cnn.Close();
            }
        }

        private void release(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to erase all reserved orders? This cannot be undone!",
                    "Alert",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    while (true)
                    {
                        using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                        {
                            Int64 ItemID;

                            cnn.Open();

                            string stm = "SELECT * FROM purchases";
                            using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                            {
                                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                                {
                                    rdr.Read();
                                    ItemID = (Int64)rdr["ItemID"];
                                }
                            }

                            stm = "DELETE FROM purchases WHERE ItemID = '" + ItemID + "'";
                            using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            stm = "UPDATE stock SET inStock = '1' WHERE ItemID = '" + ItemID + "'";
                            using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Clean up complete", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    OrderTable.Items.Clear();
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Operation canceled, no data was modified", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void show_all_orders(object sender, RoutedEventArgs e)
        {
            orderSearchBox.Text = "";
            searchOrder();
        }

        private void release_single(object sender, RoutedEventArgs e)
        {

            Int64 ItemID = 0;
            try
            {
                Order selected_id = (Order)OrderTable.SelectedItems[0];
                ItemID = (Int64)selected_id.ItemID;
            }
            catch(Exception Ex)
            {
                MessageBox.Show("No item selected",
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to release this reserved order? This cannot be undone!",
                    "Alert",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();

                    string stm = "DELETE FROM purchases WHERE ItemID = '" + ItemID + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    stm = "UPDATE stock SET inStock = '1' WHERE ItemID = '" + ItemID + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Item released", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    OrderTable.Items.Clear();
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Operation canceled, no data was modified", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void show_all_repairs(object sender, RoutedEventArgs e)
        {
            repairSearchBox.Text = "";
            searchAllRepair();
        }

        private void finish_repair(object sender, RoutedEventArgs e)
        {
            try
            {
                Repair selected_id = (Repair)RepairTable.SelectedItems[0];
            }
            catch
            {
                MessageBox.Show("No item selected",
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to finish this repair? This cannot be undone!",
                    "Alert",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();

                    Repair selected_id = (Repair)RepairTable.SelectedItems[0];

                    string stm = "UPDATE repairs SET Finished = '1' WHERE RepairID = '" + (Int64)selected_id.IssueID + "'";

                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
            }
            if(result == MessageBoxResult.No)
            {
                return;
            }
        }

        private void selectRepair(object sender, RoutedEventArgs e)
        {
            Int64 Nice = 0;

            try
            {
                Repair selected_id = (Repair)RepairTable.SelectedItems[0];
                Nice = (Int64)selected_id.IssueID;
                RepairInfo repairLoader = new RepairInfo((int)Nice);
                repairLoader.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("No item selected! Error: " + Ex,
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                return;
            }
        }

        private void confirm(object sender, RoutedEventArgs e)
        {
            try
            {
                Order selected_id = (Order)OrderTable.SelectedItems[0];
            }
            catch
            {
                MessageBox.Show("No item selected",
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to confirm this order? Ensure that the customer's payment has been processed and has come through. This action cannot be undone!",
                   "Alert",
                   MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();

                    Order selected_id = (Order)OrderTable.SelectedItems[0];

                    string stm = "UPDATE purchases SET Purchased = '1' WHERE ItemID = '" + (Int64)selected_id.ItemID + "'";

                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    OrderTable.Items.Clear();
                    cnn.Close();
                }
            }
            if (result == MessageBoxResult.No)
            {
                MessageBox.Show("Data was not changed",
                   "Alert",
                   MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Cancel);
                return;
            }
        }
    }
}
