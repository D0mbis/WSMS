using System;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Windows;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace WSMS.Services
{
    internal class WebService : IDisposable
    {
        private DriverManager? Manager = null;
        private static IWebDriver Driver = default;
        private static readonly string Url = "https://web.whatsapp.com/";

        public static void StartBrowser()
        {
            Driver = new ChromeDriver();
            Driver.Navigate().GoToUrl(Url);
        }
        public static void CloseBrowser()
        {
            if (Driver != default)
                Driver.Dispose();
        }

        public void UpdateChromeDriver()
        {
            try
            {
                Manager = new DriverManager();
                Manager.SetUpDriver(new ChromeConfig());
                MessageBox.Show("Chromedriver was updated to {} version.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chromedriver was not updated./n Error: {ex.Message}");
            }
        }
        public void Dispose()
        {
            if (Manager != null) { Manager = default; }
            if (Driver != null) { Driver.Dispose(); }
        }
    }
}
