using System;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Windows;

namespace WSMS.Services
{
    internal class WebService : IDisposable
    {
        private DriverManager? manager = null;
        public void UpdateChromeDriver()
        {
            try
            {
                manager = new DriverManager();
                manager.SetUpDriver(new ChromeConfig());
                MessageBox.Show("Chromedriver was updated to {} version.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chromedriver was not updated./n Error: {ex.Message}");
            }
        }
        public void Dispose()
        {
            if (manager != null) { manager = default; }
        }
    }
}
