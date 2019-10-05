using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U_Technican
{
    /// <summary>
    /// Interaction logic for UnfinishedRepairs.xaml
    /// </summary>
    public partial class UnfinishedRepairs : Window
    {
        class WIP
        {
            public Int64 ID { get; set; }
            public Int64 ClientID { get; set; }
            public string Issue { get; set; }
        }

        public UnfinishedRepairs()
        {
            InitializeComponent();

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
                            if((Int64)rdr["Finished"] == 0)
                            {
                                current_repairs.Items.Add(new WIP()
                                {
                                    ID = (Int64)rdr["RepairID"],
                                    ClientID = (Int64)rdr["ClientID"],
                                    Issue = (string)rdr["Issue"]
                                });
                            }
                        }
                    }
                    cnn.Close();
                }
            }
        }

        private void see_more(object sender, RoutedEventArgs e)
        {
            try
            {
                WIP selected_id = (WIP)current_repairs.SelectedItems[0];

                RepairInfo repairInfo_nav = new RepairInfo((int)selected_id.ID);
                repairInfo_nav.Show();
                this.Close();
            }
            catch
            {
                MessageBox.Show("You need to select an item first!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
