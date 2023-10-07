using System;
using System.Windows;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace WSMS.Services
{
    internal class WebService
    {
        private static IWebDriver Driver { get; set; }
        private static MessageService? MessageService { get; set; } = default;
        private static readonly string Url = "https://web.whatsapp.com/";
        private static readonly string SessionsPath = $"{Environment.CurrentDirectory}\\Sessions"; // allow to add different accounts like "Cookies\\Account name(number phone)
        public static int ProcessID { get; private set; }

        public static void StartBrowser()
        {
            /*"As of Selenium 4.6, Selenium downloads the correct driver for you. You shouldn’t need to do anything. (from ducumentation)
            ChromeDriverService service = ChromeDriverService.CreateDefaultService("PATH of chromedriver.exe folder");
            */

            ChromeOptions options = new ();
            options.AddUserProfilePreference("intl.accept_languages", "ru-RU");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            options.AddArgument("--user-data-dir=" + SessionsPath); // saving every 2 last sessions
            try
            {
                Driver = new ChromeDriver(service, options);
                Driver.Navigate().GoToUrl(Url);
                ProcessID = service.ProcessId;
            }
            catch (Exception ex)
            {
                string link = "https://chromedriver.chromium.org/downloads";
                MessageBoxResult result = MessageBox.Show($"Browser don`t started, do you want to copy in your clipboard the link of download new version?\n{link}" +
                    $"\n\nDetails:\n{ex.Message}",
                    "Your driver is outdated", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    Clipboard.SetText($"{link}");
                    MessageBox.Show("The link has been copied, paste it into your browser.");
                }
            }
        }
        public static void CloseBrowser()
        {
            if (Driver != default)
                Dispose();
        }
        public static void StartSending(Models.Message message)
        {
            if (Driver != default)
            {
                MessageService = new MessageService();
                MessageService.StartSending(Driver, message);
                MessageService = default;
            }
            else
            {
                MessageBox.Show("Please, start the browser first.");
            }
        }
        public static void Dispose()
        {
            if (Driver != default) { Driver.Close(); Driver.Quit(); Driver.Dispose(); Driver = default; }
            //if (MessageService != default) { MessageService = default; }
        }
    }
}
