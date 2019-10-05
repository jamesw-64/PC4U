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
    /// Interaction logic for SearchStock.xaml
    /// </summary>
    public partial class SearchStock : Window
    {
        class Device
        {
            public Int64 ID { get; set; }
            public string ItemName { get; set; }
            public string Type { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string HDDSize { get; set; }
        }

        public SearchStock()
        {
            InitializeComponent();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            AllInfo.Items.Clear();
            using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
            {
                cnn.Open();
                string selected_filter = search_filter.Text;

                string stm = "SELECT  *  FROM stock  WHERE ItemName LIKE '%" + SearchTerm.Text + "%'";
                if (selected_filter != "")
                {
                    stm = stm + " AND Type = '" + selected_filter + "'";
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
                                if (sold_search_check.IsChecked == true ? true : (Int64)rdr["inStock"] == 1)
                                {
                                    AllInfo.Items.Add(new Device()
                                    {
                                        ID = (Int64)rdr["ItemID"],
                                        ItemName = (string)rdr["ItemName"],
                                        Type = (string)rdr["Type"],
                                        Brand = (string)rdr["Brand"],
                                        Model = (string)rdr["Model"],
                                        HDDSize = (string)rdr["HDD"]
                                    });
                                }
                            }
                        }
                    }
                }
                cnn.Close();
            }
        }

        private void Enter(object sender, RoutedEventArgs e)
        {
            Device selected_id = (Device)AllInfo.SelectedItems[0];
            StockInfo stockLoader = new StockInfo((int)selected_id.ID, false);
            stockLoader.Show();
            this.Close();
        }
    }
}
