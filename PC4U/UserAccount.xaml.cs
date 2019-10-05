using System;
using System.Data.SQLite;
using System.Linq;
using System.Net.Mail;
using System.Windows;

namespace PC4U
{
    public partial class UserAccount : Window
    {
        public string username_global;
        bool editingMode = false;
        Int64 id = 0;

        private string isEmpty(string field)
        {
            //do this to show the user the error when they are changing data 
            throw new Exception(field + " not provided");
            return null;
        }

        // classes for list views, using the same idea as seen in Browse.cs
        class WIP
        {
            public Int64 ID { get; set; }
            public string Issue { get; set; }
        }

        class Repairs
        {
            public Int64 ID { get; set; }
            public string Issue { get; set; }
        }

        class Purchases
        {
            public Int64 ID { get; set; }
            public Int64 ItemID { get; set; }
            public string ItemName { get; set; }
        }

        private void saveInfo()
        {
            // similar logic to saving updated client information as seen in Register.cs, refer
            // to it for detailed commentary
            
            //check if fields are empty and get information from text boxes
            var address = addressInput.Text ;

            try
            {
                var firstName = string.IsNullOrEmpty(firstNameInput.Text) ? isEmpty("First name") : firstNameInput.Text;
                var lastName = string.IsNullOrEmpty(lastNameInput.Text) ? isEmpty("Last name") : lastNameInput.Text;

                if ((firstName.Length + lastName.Length + 1) > 27)
                {
                    MessageBox.Show("Total length of data in the name fields is too long. For the name, you have 26 characters in total (meaning, for example, you can have 10 characters in the first name field and 16 in the last name field). If this is your actual name, you will need to update your information with the receptionist.\n\nYou have " + firstName.Length + " characters in the first name field and " + lastName.Length + " in the last name field, for a total of " + (firstName.Length + lastName.Length) + " characters.", "Error updating infromation!", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    return;
                }

                var email = string.IsNullOrEmpty(emailInput.Text) ? isEmpty("Email") : (new MailAddress(emailInput.Text)).ToString();
                var password = string.IsNullOrEmpty(passwordInput.Password) ? isEmpty("Password") :  passwordInput.Password;
                var dataSharing = dataSharingInput.IsChecked == true ? 1 : 0;

                if (!firstName.All(Char.IsLetter)) { throw new Exception("First name contains non-letter character(s)"); }
                if (!lastName.All(Char.IsLetter)) { throw new Exception("Last name contains non-letter character(s)"); }

                //push data to database
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string query = "UPDATE users SET FirstName = '" + firstName + "', LastName = '" + lastName + "', Password = '" + password + "', Address = '" + address + "', Email = '" + email + "', DataSharing = '" + dataSharing + "' WHERE username = '" + username_global + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    cnn.Close();
                }

                // disable controls as save button has been pressed
                firstNameInput.IsEnabled = false;
                lastNameInput.IsEnabled = false;
                emailInput.IsEnabled = false;
                dobPicker.IsEnabled = false;
                passwordInput.IsEnabled = false;
                addressInput.IsEnabled = false;
                dataSharingInput.IsEnabled = false;

                enableEdit.IsEnabled = true;
                enableEdit.Visibility = Visibility.Visible;

                save.IsEnabled = false;
                save.Visibility = Visibility.Hidden;

                editingMode = false;
            }
            catch (Exception Ex)
            {
                // if one of the isEmpty is called, then this message is shown.
                // This will continue to leave editing mode on
                if (Convert.ToString(Ex.Message) == "The specified string is not in the form required for an e-mail address.") { MessageBox.Show("There was a problem saving the input data. Here's the problem: Email address is not valid, missing an @ symbol", "Error saving data!", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                else { MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error saving data!", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
            }

            // open connection to database for later use
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM users WHERE username = '" + username_global + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        // getting information from database rather than textboxes as
                        // it means that if the data shows up correctly data
                        // was pushed properly. there is no up or down side to doing 
                        // this
                        nameLabel.Content = (string)rdr["FirstName"]
                            + " " + (string)rdr["LastName"]
                            + " (Client ID: " + (Int64)rdr["ClientID"] + ")";
                        cnn.Close();
                    }
                }
            }
        }

        public UserAccount()
        {
            InitializeComponent();

            // Make the Password input box to be a password input field
            // and make the concealment character '•'
            passwordInput.PasswordChar = '•';

            if (System.Windows.SystemParameters.PrimaryScreenWidth == 1366 && System.Windows.SystemParameters.PrimaryScreenHeight == 768)
            {
                WindowState = WindowState.Maximized;
            }
        }

        internal void Login(string username, bool newUser)
        {
            // open database
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                
                // get the user information and populate the fields
                string stm = "SELECT * FROM users WHERE username = '" + username + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        // gets all information from that database
                        nameLabel.Content = (string)rdr["FirstName"] + " " + (string)rdr["LastName"] + " (Client ID: " + ((Int64)rdr["ClientID"]).ToString() + ")";

                        this.Title = (string)nameLabel.Content;

                        firstNameInput.Text = (string)rdr["FirstName"];
                        lastNameInput.Text = (string)rdr["LastName"];

                        dobPicker.Content = (string)rdr["DOB"];

                        emailInput.Text = (string)rdr["Email"];
                        addressInput.Text = (string)rdr["Address"];

                        usernameInput.Content = username;
                        passwordInput.Password = (string)rdr["Password"];

                        dataSharingInput.IsChecked = (Int64)rdr["dataSharing"] == 1 ? true : false;

                        username_global = username;

                        id = (Int64)rdr["ClientID"];
                    }
                }

                // get purchases and populate the list view
                stm = "SELECT * FROM purchases WHERE ClientID = '" + id + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            no_purchase_history.Visibility = Visibility.Hidden;
                            while (rdr.Read())
                            {
                                if((Int64)rdr["Purchased"] == 1)
                                {
                                    purchase_table.Items.Add(new Purchases()
                                    {
                                        ID = (Int64)rdr["PurchaseID"],
                                        ItemID = (Int64)rdr["ItemID"],
                                        ItemName = (string)rdr["ItemName"],
                                    });
                                }
                            }
                        }
                    }
                }

                // get repairs and populate the list view
                stm = "SELECT * FROM repairs WHERE ClientID = '" + id + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                if((Int64)rdr["Finished"] == 1)
                                {
                                    no_repair_history.Visibility = Visibility.Hidden;
                                    repair_history.Items.Add(new Repairs()
                                    {
                                        ID = (Int64)rdr["RepairID"],
                                        Issue = (string)rdr["Issue"]
                                    });
                                }
                                else
                                {
                                    no_wip_alert.Visibility = Visibility.Hidden;
                                    current_repairs.Items.Add(new WIP()
                                    {
                                        ID = (Int64)rdr["RepairID"],
                                        Issue = (string)rdr["Issue"]
                                    });
                                }
                            }
                        }
                    }
                }

                // close database and show the window
                cnn.Close();
                this.Show();

                if (newUser == true)
                {
                    MessageBox.Show("Welcome to your new account, " + firstNameInput.Text + "! Here you can change your details, view currently in progress (WIP) orders and much more. The interface is rather intuitive, click around and see what's up!",
                        "Welcome!",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void backArrow_Click(object sender, EventArgs e)
        {
            if (editingMode)
            {
                MessageBoxResult result = MessageBox.Show("Do you wish to save your changes?",
                    "Alert",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

                if(result == MessageBoxResult.Yes)
                {
                    saveInfo();
                    Close();
                } else if(result == MessageBoxResult.No)
                {
                    this.Close();
                }
            }
            this.Close();
        }

        private void save_Click(object sender, EventArgs e)
        {
            saveInfo();
        }

        private void deleteAccount_Click(object sender, EventArgs e)
        {
            // show a warning to the user to make sure they want to delete their account
            MessageBoxResult dialogResult = MessageBox.Show("This is your last chance to go back! Are you sure you want to delete your account?", "Careful!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (dialogResult == MessageBoxResult.No)
            {
                return;
            }
            else if (dialogResult == MessageBoxResult.Yes)
            {
                // delete user account and replace almost all data with the redacted string, as
                // per PC4U practice
                string redacted = "█████";

                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string query = "UPDATE users SET FirstName = '" + redacted + "', LastName = '" + redacted  + "', Password = '" + redacted + "', Address = '" + redacted + "', DOB = '" + redacted  + "', Email = '" + redacted  + "', DataSharing = '0' WHERE username = '" + username_global + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
                
                MessageBox.Show("Your account has been deleted. Thank you for shopping with us!", "Operation Complete!", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void enableEdit_Click(object sender, EventArgs e)
        {
            // enable controls to allow editing
            firstNameInput.IsEnabled = true;
            lastNameInput.IsEnabled = true;
            emailInput.IsEnabled = true;
            dobPicker.IsEnabled = true;
            passwordInput.IsEnabled = true;
            addressInput.IsEnabled = true;
            dataSharingInput.IsEnabled = true;

            enableEdit.IsEnabled = false;
            enableEdit.Visibility = Visibility.Hidden;

            save.IsEnabled = true;
            save.Visibility = Visibility.Visible;

            editingMode = true;
        }

        // similar sort of idea to Browse.cs when opening that. See that file fior more detailed
        // commentary
        private void see_purchase(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemPage next_nav = new ItemPage();
                Purchases selected_id = (Purchases)purchase_table.SelectedItems[0];
                next_nav.show_system_info((Int64)selected_id.ItemID);
            }
            catch (Exception Ex)
            {
                // Commented out data in here allows for an on the fly debugging mode. This would
                // have been better with the developer_mode variable but due to time constrains
                // this will have to do
                MessageBox.Show("You must have selected an item before proceeding!"/*Error: " + Ex + ". Actual Error Message: " + Ex.Message*/, "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void requestRepair_Click(object sender, RoutedEventArgs e)
        {
            requestRepair requestRepair_nav = new requestRepair((int)id, username_global);
            requestRepair_nav.Show();
            this.Close();
        }

        private void repair_see(object sender, RoutedEventArgs e)
        {
            // again, same as Browse.cs
            try
            {
                var repairTracking = new requestRepair((int)id, username_global);

                Repairs selected_id = (Repairs)repair_history.SelectedItems[0];
                repairTracking.tracking_show((int)selected_id.ID);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("You must have selected an item before proceeding!"/*Error: " + Ex + ". Actual Error Message: " + Ex.Message*/, "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Wip_see_Click(object sender, RoutedEventArgs e)
        {
            // See Browse.cs
            try
            {
                var repairTracking = new requestRepair((int)id, username_global);

                WIP selected_id = (WIP)current_repairs.SelectedItems[0];
                repairTracking.tracking_show((int)selected_id.ID);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("You must have selected an item before proceeding!"/*Error: " + Ex + ". Actual Error Message: " + Ex.Message*/, "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
