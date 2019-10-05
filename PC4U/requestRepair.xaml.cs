using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;

namespace PC4U
{
    /// <summary>
    /// Interaction logic for requestRepair.xaml
    /// </summary>
    public partial class requestRepair : Window
    {
        // initalisation of global variables so that they can be used across methods
        int ClientID_global = 0;
        string username_global;

        // if we are reviewing the repair then this will be true
        bool tracking_review = false;

        // initalisation of window
        public requestRepair(int ClientID, string username)
        {
            // set the global variables to the passed in variables for use later
            ClientID_global = ClientID;
            username_global = username;
            InitializeComponent();

            // set window title and title bar as the clientID. looks better than having nothing
            // at the top
            this.Title = "Request Repair (Client ID: " + ClientID + ")";
            nameLabel.Content = "Request Repair (Client ID: " + ClientID + ")";

            if (System.Windows.SystemParameters.PrimaryScreenWidth == 1366 && System.Windows.SystemParameters.PrimaryScreenHeight == 768)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void backArrow_Click(object sender, RoutedEventArgs e)
        {
            // if we aren't reviewing we will want to log back into the user account due to the
            // logic of this system
            if(tracking_review == false)
            {
                var nice = new UserAccount();
                nice.Login(username_global, false);
            }
            this.Close();
        }

        public void tracking_show(int repair_id)
        {
            // if we are reviewing a repair...
            tracking_review = true;

            // ...remove all buttons that allow the data to be editted after a save...
            type_label.Visibility = Visibility.Hidden;
            machine_type.Visibility = Visibility.Hidden;
            warrenty.Visibility = Visibility.Hidden;
            submit.Visibility = Visibility.Hidden;

            problem_label_Copy.Visibility = Visibility.Visible;
            issue_Copy.Visibility = Visibility.Visible;
            trackID.Visibility = Visibility.Visible;

            // ...and repurpose some of the labels
            problem_label.Text = "Tracking";

            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM repairs";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // get tracking infromation
                            trackID.Content = (Int64)rdr["RepairID"];
                            issue.Text = (string)rdr["Tracking"];
                            issue_Copy.Text = (string)rdr["Issue"];
                        }
                    }
                    cnn.Close();
                }
            }
            // set fields as read only to prevent accidental editing
            issue.IsReadOnly = true;
            issue_Copy.IsReadOnly = true;

            this.Show();
        }

        private void submitRepair_Click(object sender, RoutedEventArgs e)
        {
            if(issue.Text == "")
            {
                MessageBox.Show("You didn't provide any issue information!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if(machine_type.Text == "")
            {
                MessageBox.Show("You didn't provide any device type information!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
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

                        MessageBox.Show("Thank you! Please give this ID: " + repair_id + " to the desk along with the item.", "Thanks!", MessageBoxButton.OK, MessageBoxImage.Information);
                        var nice = new UserAccount();

                        nice.Login(username_global, false);
                        this.Close();
                    }
                }
            }
        }
    }
}
