using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateCustomer.xaml
    /// </summary>
    public partial class CreateCustomer : Window
    {
        public CreateCustomer()
        {
            InitializeComponent();
        }

		//TODO: add the ability to view purchase and repair info for each user (may just reimpliement the User Account stuff from the KIOSK program)

        private void registerNavButton_Click(object sender, EventArgs e)
        {
            string isEmpty(string field)
            {
                throw new Exception(field + " not provided");
                return null;
            }

            try
            {
                //getting information from text boxes

                //strips any white space from username textbox
                usernameInput.Text = Regex.Replace(usernameInput.Text, @"\s+", "");

                //required information
                var firstName = string.IsNullOrEmpty(firstNameInput.Text) ? isEmpty("First name") : firstNameInput.Text;
                var lastName = string.IsNullOrEmpty(lastNameInput.Text) ? isEmpty("Last name") : lastNameInput.Text;
                var email = string.IsNullOrEmpty(emailInput.Text) ? isEmpty("Email") : Convert.ToString(new MailAddress(emailInput.Text));
                var username = string.IsNullOrEmpty(usernameInput.Text) ? isEmpty("Username") : usernameInput.Text;
                var password = string.IsNullOrEmpty(passwordInput.Password) ? isEmpty("Password") : passwordInput.Password;

                //extra validation
                if (!firstName.All(Char.IsLetter)) { throw new Exception("First name contains non-letter character(s)"); }
                if (!lastName.All(Char.IsLetter)) { throw new Exception("Last name contains non-letter character(s)"); }

                //not required information
                var address = addressInput.Text;
                var dob = "";

                DateTime? selectedDate = dobPicker.SelectedDate;
                if (selectedDate.HasValue)
                {
                    dob = selectedDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                int dataSharing = dataSharingInput.IsChecked == true ? dataSharing = 1 : dataSharing = 0;

                //FFR: these one liners mean "if <condition> then do this otherwise do that
                //                        createVar ==    ?  var = "on"   :     var = "off"

                //check to see if the username is unique
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string stm = "SELECT * FROM users WHERE username = '" + username + "'";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            rdr.Read();
                            if (rdr.HasRows == false)
                            {
                                string query = "INSERT INTO users (FirstName, LastName, Username, Password, Address, DOB, Email, DataSharing) VALUES ('" + firstName + "', '" + lastName + "', '" + username + "', '" + password + "', '" + address + "', '" + dob + "', '" + email + "', '" + dataSharing + "')";
                                using (SQLiteCommand cmd2 = new SQLiteCommand(query, cnn))
                                {
                                    cmd2.ExecuteNonQuery();
                                }

                                MessageBox.Show("Account Created!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

                                cnn.Close();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("That username is already taken!", "Error creating account!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                if (Convert.ToString(Ex.Message) == "The specified string is not in the form required for an e-mail address.") { MessageBox.Show("There was a problem saving the input data. Here's the problem: Email address is not valid, missing an @ symbol", "Error creating account!", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                else { MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error creating account!", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
            }
        }
    }
}
