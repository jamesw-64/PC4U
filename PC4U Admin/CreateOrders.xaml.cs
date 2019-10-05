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
using System.Windows.Shapes;

namespace PC4U_Admin
{
    /// <summary>
    /// Interaction logic for CreateOrders.xaml
    /// </summary>
    public partial class CreateOrders : Window
    {
        int ClientID_global = 0;
        string username_global;

        bool tracking_review = false;

        // initalise window
        public CreateOrders(int ClientID)
        {
            ClientID_global = ClientID;
            InitializeComponent();

            this.Title = "Request Repair (Client ID: " + ClientID + ")";

            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            { 
                cnn.Open();
                string query = "SELECT FirstName, LastName FROM users WHERE ClientID = '" + ClientID + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        nameLabel.Text = "Request Repair (" + (string)rdr["FirstName"] + " " + (string)rdr["LastName"] + ", Client ID: " + ClientID + ")";
                    }
                }
            }
        }

        private void submitRepair_Click(object sender, RoutedEventArgs e)
        {
            // validate input
            if (issue.Text == "")
            {
                MessageBox.Show("You didn't provide any issue information!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (machine_type.Text == "")
            {
                MessageBox.Show("You didn't provide any device type information!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                // open database
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    int warrenty_checked = warrenty.IsChecked == true ? 1 : 0;
                    cnn.Open();
                    string Issue = issue.Text.Replace("'", "''");
                    Int64 repair_id = 0;
                    string query = "INSERT INTO repairs (ClientID, Issue, inWarrenty) VALUES ('" + ClientID_global + "', '" + "Device Type: " + machine_type.Text + ".\nClient Defined Issue: " + Issue + "', '" + warrenty_checked + "')";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string stm = "SELECT * FROM repairs";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                repair_id = (Int64)rdr["RepairID"];
                            }
                        }

                        cnn.Close();

                        MessageBox.Show("Committed to database.", "Information!", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
            }
        }
    }
}
