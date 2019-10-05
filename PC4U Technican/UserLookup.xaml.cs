using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U_Technican
{
    /// <summary>
    /// Interaction logic for UserLookup.xaml
    /// </summary>
    public partial class UserLookup : Window
    {
        Int64 ID = 0;
        public UserLookup()
        {
            InitializeComponent();
        }

        private void search(object sender, RoutedEventArgs e)
        {
            try
            {
                ID = Convert.ToInt64(Input.Text);
            }
            catch(Exception ex)
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
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM users WHERE ClientID = '" + ID + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            ClientInfo loader = new ClientInfo();
                            loader.load_info(ID);
                            this.Close();
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Unable to find client from provided ID!",
                    "Alert",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                }
            }
        }
    }
}
