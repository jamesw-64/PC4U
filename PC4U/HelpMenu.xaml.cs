using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace PC4U
{
    /// <summary>
    /// Interaction logic for HelpMenu.xaml. Will display debugging information if developer = true
    /// </summary>
    public partial class HelpMenu : Window
    {
        // The following two methods are from:
        // https://stackoverflow.com/questions/6331826/get-os-version-friendly-name-in-c-sharp.
        
        // The following three methods are used to get information on the system that the program 
        // is running on. This will be used by PC4U technicians to try and diagnose issues with 
        // kiosks
        public string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public string FriendlyName()
        {
            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }

        public static double GetTotalMemory()
        {
            ulong RamSize = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            double GBRam = Math.Round((float)RamSize / (1024 * 1024 * 1024), 2);
            return GBRam;
        }

        // initalisation logic
        public HelpMenu(bool developer)
        {
            InitializeComponent();

            if (System.Windows.SystemParameters.PrimaryScreenWidth == 1366 && System.Windows.SystemParameters.PrimaryScreenHeight == 768)
            {
                WindowState = WindowState.Maximized;
            }

            // we will always get the debug information, but we won't always show it. After testing
            // I know that this will not signficantly alter the load speed of the window, so this
            // is okay to do
            if (developer)
            {
                debug_info.Visibility = Visibility.Visible;
            }

            try
            {
                RegistryKey Rkey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
                SystemInfo.Content =
                    "----- SYSTEM INFORMATION -----"
                    + "\nOS Name: " + FriendlyName()
                    + "\nOS Version: " + System.Environment.OSVersion.ToString()
                    + "\nWindows Dir: " + System.Environment.SystemDirectory
                    + "\nMachine Name: " + System.Environment.MachineName
                    + "\nOS Uptime: " + ((System.Environment.TickCount / 1000) / 60).ToString() + " Minute(s)"
                    + "\nInstalled RAM: " + GetTotalMemory() + "GB"
                    + "\nProcessor: " + (string)Rkey.GetValue("ProcessorNameString") + " (" + Rkey.GetValue("~MHZ") + "MHZ)"
                    + "\nScreen resolution: " + Screen.PrimaryScreen.Bounds.Width + "*" + Screen.PrimaryScreen.Bounds.Height
                    + "\n\n----- PROGRAM INFORMATION -----"
                    + "\nProgram Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception EX)
            {
                MessageBox.Show("Error getting getting Debug information");
            }
        }

        private void backArrow_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
