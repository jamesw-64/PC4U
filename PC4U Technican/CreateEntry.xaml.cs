using System;
using System.Data.SQLite;
using System.Windows;

namespace PC4U_Technican
{
    /// <summary>
    /// Interaction logic for CreateEntry.xaml
    /// </summary>
    public partial class CreateEntry : Window
    {
        private string isEmpty(string field)
        {
            throw new Exception(field + " not provided");
            return null;
        }

        public CreateEntry()
        {
            InitializeComponent();
        }

        private void commit(object sender, RoutedEventArgs e)
        {
            // vaildation 
            try
            {
                var itemName = string.IsNullOrEmpty(ItemName.Text) ? isEmpty("Item name") : ItemName.Text;
                var type_filter = string.IsNullOrEmpty(type.Text) ? isEmpty("Type") : type.Text;
                var price = string.IsNullOrEmpty(Price.Text) ? isEmpty("Price") : Price.Text;
                var ram_size = string.IsNullOrEmpty(RAMSize.Text) ? isEmpty("RAM Size") : RAMSize.Text;
                var brand = string.IsNullOrEmpty(Brand.Text) ? isEmpty("Brand") : Brand.Text;
                var model = string.IsNullOrEmpty(Model.Text) ? isEmpty("Model") : Model.Text;
                var os = string.IsNullOrEmpty(OS.Text) ? isEmpty("OS") : OS.Text;
                var processor = string.IsNullOrEmpty(Processor.Text) ? isEmpty("Processor") : Processor.Text;
                var graphics = string.IsNullOrEmpty(Graphics.Text) ? isEmpty("Graphics") : Graphics.Text;
                var hdd = string.IsNullOrEmpty(HDD.Text) ? isEmpty("HDD size") : HDD.Text;
                var bluetooth = (bool)Bluetooth.IsChecked ? 1 : 0;
                var wifi = (bool)WiFi.IsChecked ? 1 : 0;
                var details = string.IsNullOrEmpty(Details.Text) ? isEmpty("Details") : Details.Text;

                // commiting to the database
                using (SQLiteConnection cnn = new SQLiteConnection(database.LoadConnectionString()))
                {
                    cnn.Open();
                    string query = "INSERT INTO stock (ItemName, Type, Price, RAMSize, Brand, Model, OS, Processor, Graphics, HDD, Bluetooth, WiFi, Details, inStock) VALUES ('" + itemName + "', '" + type_filter + "', '" + price + "', '" + ram_size + "', '" + brand + "', '" + model + "', '" + os + "', '" + processor + "', '" + graphics + "', '" + hdd + "', '" + bluetooth + "', '" + wifi + "', '" + details + "', '1')";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    cnn.Close();

                    MessageBox.Show("Operation finished!", "Ok!", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }

            catch (Exception Ex)
            {
                MessageBox.Show("There was a problem saving the input data. Here's the problem: " + Ex.Message, "Error creating new entry!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
