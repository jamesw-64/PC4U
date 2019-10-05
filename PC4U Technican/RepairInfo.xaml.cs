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

namespace PC4U_Technican
{
    /// <summary>
    /// Interaction logic for RepairInfo.xaml
    /// </summary>
    public partial class RepairInfo : Window
    {
        // initalise global variable
        Int64 RepairID = 0;

        public RepairInfo(int ID)
        {
            InitializeComponent();

            // set global variable to passed in ID
            RepairID = ID;

            // set title as ID
            Title.Content = "Repair ID: " + ID;

            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM repairs WHERE RepairID = '" + ID + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        // input text from database into fields
                        client_id.Text = ((Int64)rdr["ClientID"]).ToString();
                        issue.Text = (string)rdr["Issue"];
                        tracking.Text = (string)rdr["Tracking"];
                    }
                }

                stm = "SELECT * FROM users WHERE ClientID = '" + client_id.Text + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        // set client name
                        rdr.Read();

                        client_name.Content = (string)rdr["FirstName"] + " " + (string)rdr["LastName"];
                    }
                }
                cnn.Close();
            }
        }

        private void save(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                
                // update information in the database
                string query = "UPDATE repairs SET Tracking = '" + tracking.Text.Replace("'", "''") + "' WHERE RepairID = '" + RepairID + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                {
                    cmd.ExecuteNonQuery();
                }
                cnn.Close();
            }
            this.Close();
        }

        private void finish(object sender, RoutedEventArgs e)
        {
            // flagging in the database that this item is finished with
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to flag this repair as finished? If will no longer show up on the unfinished repairs list.",
                    "Alert",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string query = "UPDATE repairs SET Tracking = '" + tracking.Text.Replace("'", "''") + "' WHERE RepairID = '" + RepairID + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    query = "UPDATE repairs SET Finished = '1' WHERE ID = '" + RepairID + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
                this.Close();
            }
        }
    }
}
