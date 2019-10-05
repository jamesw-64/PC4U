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
    /// Interaction logic for RepairInfo.xaml
    /// </summary>
    public partial class RepairInfo : Window
    {
        Int64 RepairID = 0;

        public RepairInfo(int ID)
        {
            InitializeComponent();

            RepairID = ID;

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

                        client_id.Text = ((Int64)rdr["ClientID"]).ToString();
                        issue.Text = (string)rdr["Issue"];
                        tracking.Text = (string)rdr["Tracking"];

                        Finished.IsChecked = (Int64)rdr["Finished"] == 1 ? true : false;
                        inWarrenty.IsChecked = (Int64)rdr["inWarrenty"] == 1 ? true : false;
                    }
                }

                stm = "SELECT * FROM users WHERE ClientID = '" + client_id.Text + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
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
                int finished_check = (bool)Finished.IsChecked ? 1 : 0;
                int inwarrenty_check = (bool)inWarrenty.IsChecked ? 1 : 0;

                cnn.Open();
                string query = "UPDATE repairs SET Tracking = '" + tracking.Text.Replace("'", "''") + "', Issue = '" + issue.Text.Replace("'", "''") + "', Finished = '" + finished_check + "', inWarrenty = '" + inwarrenty_check  + "' WHERE RepairID = '" + RepairID + "'";
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
