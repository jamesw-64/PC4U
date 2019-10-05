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
    /// Interaction logic for ClientInfo.xaml. Shows
    /// </summary>
    public partial class ClientInfo : Window
    {
        public ClientInfo()
        {
            InitializeComponent();
        }

        // get the client to load from the passed in arguments
        public void load_info(Int64 ID)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "SELECT * FROM users WHERE ClientID = '" + ID + "'";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();

                        first_name.Text = (string)rdr["FirstName"];
                        last_name.Text = (string)rdr["LastName"];
                        email.Text = (string)rdr["Email"];
                        address.Text = (string)rdr["Address"];

                        Title.Content = first_name.Text + " " + last_name.Text + " (Client ID: " + ID + ")";
                    }
                }
            }

            this.Show();
        }

        private void back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
