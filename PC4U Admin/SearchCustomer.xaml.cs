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
    /// Interaction logic for SearchCustomer.xaml
    /// </summary>
    public partial class SearchCustomer : Window
    {
        class User
        {
            public Int64 ID { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
        }

        public SearchCustomer()
        {
            InitializeComponent();
        }

        private void search(object sender, RoutedEventArgs e)
        {
            AllInfo.Items.Clear();
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string stm = "";
                
                // I wasn't sure how to refactor this code to make it more efficient or use less
                // lines. However, it works. If you look at the actual window this code runs, 
                // you will see we have more than one way to search for a user. What if all those
                // field are populated? This is what this logic does, it takes all data entered
                // into account and generates a query based on this. If a field is empty, it
                // doesn't make it into the query. Follow the comments above the lines for a 
                // commentary of what each line is doing.
                
                // if there is no data entered to look for, just get all clients
                if (id_search.Text == "" && name_search.Text == "" && username_search.Text == "")
                {
                    stm = "SELECT * FROM users";
                }
                else
                {
                    // create a base statement that will have the rest of the query appended to it
                    // based on the data input
                    stm = "SELECT  *  FROM users WHERE ";
                    
                    // ok, so how this works is that if the field we are checking is not empty...
                    if (id_search.Text != "")
                    {
                        // ...we will check if the stm is the same as the base (this is slightly
                        // redundant here as this is the first check, but it's fine)...
                        if (stm == "SELECT  *  FROM users WHERE ")
                        {
                            // ...if it is the same as the base we will append our first part of 
                            // the query as this means that this is the first feild with data in it
                            stm = stm + "ClientID = '" + id_search.Text + "'";
                        }
                        else
                        {
                            // ...if it is not the same as the base this means that there is 
                            // already a field with data in it and we must append our extra
                            // search requirement onto the end
                            stm = stm + " AND ClientID = '" + id_search.Text + "'";
                        }
                    }
                    
                    // same sort of idea here...
                    if (name_search.Text != "")
                    {
                        if(stm == "SELECT  *  FROM users WHERE ")
                        {
                            // ...if there was nothing in the ID field, this must be the first
                            // field with data in it. So this means that this is the first part
                            // of the data to be searched
                            stm = stm + " LastName LIKE '%" + name_search.Text + "%'";
                        }
                        else
                        {
                            // ...if it's not the same as the base query, that must mean mean
                            // another field had data in it so we need to append ourselves onto
                            // the end of the query
                            stm = stm + " AND LastName LIKE '%" + name_search.Text + "%'";
                        } 
                    }
                    
                    // ditto
                    if (username_search.Text != "")
                    {
                        if (stm == "SELECT  *  FROM users WHERE ")
                        {
                            stm = stm + " Username LIKE '%" + username_search.Text + "%'";
                        }
                        else
                        {
                            stm = stm + " AND Username LIKE '%" + username_search.Text + "%'";
                        }
                    }
                    
                    // as a result, no matter if one field, all fields or no fileds have data in
                    // them we will always end up with a valid query                    
                }

                using (SQLiteCommand cmd = new SQLiteCommand(stm, cnn))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows == false)
                        {
                            return;
                        }
                        else
                        {
                            while (rdr.Read())
                            {
                                // allowing searching of deleted customers so we can revive the
                                // account
                                if ((string)rdr["FirstName"] == "█████" && search_deleted.IsChecked == true)
                                {
                                    AllInfo.Items.Add(new User()
                                    {
                                        ID = (Int64)rdr["ClientID"],
                                        Name = (string)rdr["FirstName"] + " " + (string)rdr["LastName"],
                                        Username = (string)rdr["Username"]
                                    });
                                }
                                else if ((string)rdr["FirstName"] == "█████" && search_deleted.IsChecked == false)
                                {
                                    continue;
                                }
                                else
                                {
                                    AllInfo.Items.Add(new User()
                                    {
                                        ID = (Int64)rdr["ClientID"],
                                        Name = (string)rdr["FirstName"] + " " + (string)rdr["LastName"],
                                        Username = (string)rdr["Username"]
                                    });
                                }
                            }
                        }
                    }
                }
                cnn.Close();
            }
        }

        private void select(object sender, RoutedEventArgs e)
        {
            try
            {
                User selected_id = (User)AllInfo.SelectedItems[0];
                ClientInfo clientLoader = new ClientInfo((int)selected_id.ID);
                clientLoader.Login();
                this.Close();
            }
            catch
            {
                MessageBox.Show("You must select an item from the list before continuing", "Alert!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
