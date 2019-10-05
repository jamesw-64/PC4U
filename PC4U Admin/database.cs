using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC4U_Admin
{
    /// <summary>
    /// This allows me to load the ConnectionString without having to write this whole thing out
    /// evey time I want to access the database. Reduces the amount of code in the program
    /// by a small amount, but is a huge help when interacting with the database.
    /// </summary>
    class database
    {
        public static string LoadConnectionString(string id = "data")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
