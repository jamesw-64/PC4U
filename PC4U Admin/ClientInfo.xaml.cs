using System;
using System.Data.SQLite;
using System.Net.Mail;
using System.Windows;
using System.Linq;

namespace PC4U_Admin
{
    /// <summary>
    /// Interaction logic for ClientInfo.xaml
    /// </summary>
    public partial class ClientInfo : Window
    {
        int ID_global = 0;
        string username_global;

        public ClientInfo(int ID)
        {
            InitializeComponent();

            var DateToday = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            dobPicker.DisplayDateEnd = DateToday.AddYears(-18);

            ID_global = ID;
        }

        string isEmpty(string field)
        {
            throw new Exception(field + " not provided");
            return null;
        }

        internal void Login()
        {
            //open database
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM users WHERE ClientID = '" + ID_global + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        //gets all information from that database
                        nameLabel.Content = (string)rdr["FirstName"] + " " + (string)rdr["LastName"] + " (Client ID: " + ((Int64)rdr["ClientID"]).ToString() + ")";

                        this.Title = (string)nameLabel.Content;

                        firstNameInput.Text = (string)rdr["FirstName"];
                        lastNameInput.Text = (string)rdr["LastName"];

                        try
                        {
                            dobPicker.DisplayDate = DateTime.ParseExact((string)rdr["DOB"], "dd/MM/yyyy",
                                                    System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            
                        }
                        

                        emailInput.Text = (string)rdr["Email"];
                        addressInput.Text = (string)rdr["Address"];

                        usernameInput.Text = (string)rdr["Username"];
                        passwordInput.Password = (string)rdr["Password"];

                        username_global = usernameInput.Text;

                        this.Show();
                    }
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            //check if fields are empty and get information from text boxes
            var dob = dobPicker.ToString();

            DateTime? selectedDate = dobPicker.SelectedDate;
            if (selectedDate.HasValue)
            {
                dob = selectedDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            var address = addressInput.Text;

            try
            {
                var firstName = string.IsNullOrEmpty(firstNameInput.Text) ? isEmpty("First name") : firstNameInput.Text;
                var lastName = string.IsNullOrEmpty(lastNameInput.Text) ? isEmpty("Last name") : lastNameInput.Text;
                var email = string.IsNullOrEmpty(emailInput.Text) ? isEmpty("Email") : (new MailAddress(emailInput.Text)).ToString();
                var password = string.IsNullOrEmpty(passwordInput.Password) ? isEmpty("Password") : passwordInput.Password;
                var dataSharing = dataSharingInput.IsChecked == true ? 1 : 0;

                //extra validation
                if (!firstName.All(Char.IsLetter)) { throw new Exception("First name contains non-letter character(s)"); }
                if (!lastName.All(Char.IsLetter)) { throw new Exception("Last name contains non-letter character(s)"); }

                //push data to database
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string query = "UPDATE users SET FirstName = '" + firstName + "', LastName = '" + lastName + "', Password = '" + password + "', Address = '" + address + "', DOB = '" + dob + "', Email = '" + email + "', DataSharing = '" + dataSharing + "' WHERE ClientID = '" + ID_global + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    cnn.Close();
                }
            }
            catch (Exception Ex)
            {
                //if one of the isEmpty is called, then this message is shown.
                //This will continue to leave editing mode on
                MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error creating account!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            //open connection to database for later use
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM users WHERE username = '" + username_global + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        //getting information from database rather than textboxes as
                        //it means that if the data shows up correctly data
                        //was pushed properly. there is no up or down side to doing 
                        //this
                        nameLabel.Content = (string)rdr["FirstName"]
                            + " " + (string)rdr["LastName"]
                            + " (Client ID: " + (Int64)rdr["ClientID"] + ")";
                        cnn.Close();
                    }
                }
            }

            this.Close();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                // delete user account and replace almost all data with the redacted string, as
                // per PC4U practice
                string redacted = "█████";

                cnn.Open();
                string query = "UPDATE users SET FirstName = '" + redacted + "', LastName = '" + redacted + "', Password = '" + redacted + "', Address = '" + redacted + "', DOB = '" + redacted + "', Email = '" + redacted + "', DataSharing = '0' WHERE username = '" + username_global + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                {
                    cmd.ExecuteNonQuery();
                }
                cnn.Close();
            }

            MessageBox.Show("Account has been deleted.", "Operation Complete!", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
