using System;
using System.Windows;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using SeleniumExtras.WaitHelpers;

namespace WSMS.Services
{
    internal class WebService
    {
        private static IWebDriver Driver { get; set; }
        public static string Errors { get; private set; }
        public static bool IsRunning { get; set; } = false;
        private static readonly string Url = "https://web.whatsapp.com/";
        private static readonly string SessionsPath = $"{Environment.CurrentDirectory}\\Sessions"; //is possible to add different accounts like "Cookies\\Account name(number phone)
        private static Dictionary<string, string> ElementsPaths = new()
        {
            { "Search field", ".to2l77zo.gfz4du6o.ag5g9lrv.bze30y65.kao4egtt.qh0vvdkp .selectable-text.copyable-text.iq0m558w.g0rxnol2" },
            { "Message input", "._3Uu1_ .selectable-text.copyable-text.iq0m558w.g0rxnol2" },
            { "Send button", "div.g0rxnol2.thghmljt.p357zi0d.rjo8vgbg.ggj6brxn span[data-icon='send']" },
            //{ "Delete SearchText btn", "._38r4-" },
            {"Delete img btn", "._2QnjM button" },
            { "Delete SearchText btn", "button span[data-icon='x-alt']" }
        };

        public static void OpenBrowser()
        {
            /*"As of Selenium 4.6, Selenium downloads the correct driver for you. You shouldn’t need to do anything. (from ducumentation)
            ChromeDriverService service = ChromeDriverService.CreateDefaultService("PATH of chromedriver.exe folder");
            */

            ChromeOptions options = new();
            options.AddUserProfilePreference("intl.accept_languages", "ru-RU");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            options.AddArgument("--user-data-dir=" + SessionsPath); // saving every 2 last sessions
            try
            {
                Driver = new ChromeDriver(service, options);
                Driver.Navigate().GoToUrl(Url);
                IsRunning = true;
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
        public static bool ToSend(string contact, string text, BitmapSource image)
        {
            WebDriverWait wait = new(Driver, TimeSpan.FromMilliseconds(5000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            WebDriverWait waitMin = new(Driver, TimeSpan.FromMilliseconds(2000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            try
            {
                wait.IgnoreExceptionTypes(typeof(NotSupportedException));
                wait.IgnoreExceptionTypes(typeof(ElementNotInteractableException));
                try
                {
                    waitMin.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Delete SearchText btn"]))).Click();
                }
                catch { }
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Search field"]))).SendKeys(contact);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector($"span[title='{contact}']"))).Click();
                IWebElement messageInput = wait.Until(ExpectedConditions.ElementIsVisible((By.CssSelector(ElementsPaths["Message input"]))));
                try
                {
                    waitMin.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Delete img btn"]))).Click();
                }
                catch
                {
                    try
                    {
                        messageInput.SendKeys(Keys.LeftControl + "A");
                        messageInput.SendKeys(Keys.Backspace);
                    }
                    catch { }
                }
                try
                {
                    //messageInput.SendKeys(text);
                    Clipboard.SetText(text);
                    messageInput.SendKeys(Keys.LeftShift + Keys.Insert);
                    Clipboard.SetImage(image);
                    wait.Until(ExpectedConditions.ElementToBeClickable(messageInput));
                    messageInput.SendKeys(Keys.LeftShift + Keys.Insert);
                    Clipboard.Clear();
                }
                catch (Exception ex)
                {
                    Errors += $"Clipboard error:\n{ex.Message}\n";
                }
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(3, 7)));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Send button"]))).Click();
                return true;
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //CloseBrowser();
                //Errors += $"{ex.Source}: {ex.Message}\n";
                return false;
            }
            finally
            {
                wait = null;
                waitMin = null;
            }
        }
        public static void CloseBrowser()
        {
            if (Driver != default)
                Dispose();
        }
        private static void Dispose()
        {
            if (Driver != default) { Driver.Close(); Driver.Quit(); Driver.Dispose(); Driver = default; }
            //if (MessageService != default) { MessageService = default; }
            IsRunning = false;
        }
    }
}
