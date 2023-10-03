using System;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Windows;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace WSMS.Services
{
    internal class WebService : IDisposable
    {
        private DriverManager? Manager = null;
        private static IWebDriver Driver = default;
        private static readonly string Url = "https://web.whatsapp.com/";
        private static readonly string PATH = $"{Environment.CurrentDirectory}\\Cookies"; // allow to add different accounts like "Cookies\\Account name(number phone)
        public static int ProcessID { get; private set; }

        public static void StartBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            options.AddArgument("--user-data-dir=" + PATH);
            //options.AddArguments("chrome.switches", "--disable-extensions");
            Driver = new ChromeDriver(service, options);
            Driver.Navigate().GoToUrl(Url);
            ProcessID = service.ProcessId;
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
