using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WSMS.Services
{
    public class WebService
    {
        private static IWebDriver Driver { get; set; }
        public static string Errors { get; set; }
        public static bool IsRunning { get; set; } = false;
        private static readonly string Url = "https://web.whatsapp.com/";
        private static readonly string SessionsPath = $"{Environment.CurrentDirectory}\\Sessions"; //is possible to add different accounts like "Cookies\\Account name(number phone)
        private static readonly Dictionary<string, string> ElementsPaths = new()
        {
            { "Search field", "div[aria-label='Текстовое поле поиска']"},
            { "Message input", "div[aria-label='Введите сообщение']" },
            { "Send button", "span[data-icon='send']" },
            { "Delete img btn", "button[aria-label='Закрыть']" },
            { "Delete SearchText btn", "button[aria-label='Отменить поиск']" }
        };

        //
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
        public static string[] GetNotDeliveredContacts(string[] contactsArray, string checkText)
        {
            List<string> result = new();
            WebDriverWait wait2sec = new(Driver, TimeSpan.FromMilliseconds(2000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            foreach (string contact in contactsArray)
            {
                bool found = false;
                try
                {
                    SearchContact(wait2sec, contact);
                    found = true;
                }
                catch
                {
                }
                if (found)
                {
                    IWebElement? messageFull = default;
                    try { messageFull = wait2sec.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//*[text()='{checkText}']/../../../../.."))); } catch { }
                    try { messageFull?.FindElement(By.XPath("//*[text()='msg-dblcheck']")); } catch { result.Add(contact); }
                }
            }
            Logger.SaveNotDeliveryReport(result.ToArray());
            return result.ToArray();
        }
        private static void SearchContact(WebDriverWait wait, string contact)
        {
            SendKeysWithWait(By.CssSelector(ElementsPaths["Search field"]), new string[] { Keys.LeftControl + "A", Keys.Backspace, contact });
            int counter = 0;
            // checking contact paste result:
            while (counter < 2)
            {
                bool pasteResult = wait.Until(d =>
                {
                    try { d.FindElement(By.XPath($"//span//span[text()=\'{contact}\']")); return true; }
                    catch { return false; }
                });
                if (pasteResult) { break; }
                counter++;
                SendKeysWithWait(By.CssSelector(ElementsPaths["Search field"]), new string[] { Keys.LeftControl + "A", Keys.Backspace, contact });
            }

            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector($"[title='{contact}']"))).Click();
        }
        public static bool ToSend(string contact, string text, BitmapSource image)
        {
            WebDriverWait wait5sec = new(Driver, TimeSpan.FromMilliseconds(5000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            WebDriverWait wait2sec = new(Driver, TimeSpan.FromMilliseconds(2000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            try
            {
                SearchContact(wait5sec, contact);
                // deleting old image and text if exists 
                try
                {
                    wait2sec.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Delete img btn"]))).Click();
                }
                catch
                {
                    try
                    {
                        SendKeysWithWait(By.CssSelector(ElementsPaths["Message input"]), new string[] { Keys.LeftControl + "A", Keys.Backspace });
                    }
                    catch { }
                }
                try
                {
                    if (text != "" || text != default)
                    {
                        Clipboard.SetText(text);
                        SendKeysWithWait(By.CssSelector(ElementsPaths["Message input"]), new string[] { Keys.LeftShift + Keys.Insert });
                    }
                    if (image != default)
                    {
                        Clipboard.SetImage(image);
                        SendKeysWithWait(By.CssSelector(ElementsPaths["Message input"]), new string[] { Keys.LeftShift + Keys.Insert });
                    }
                    Clipboard.Clear();
                }
                catch (Exception ex)
                {
                    Errors += $"Contact name: {contact} {DateTime.Now}\nClipboard error:\n{ex.Message}\n";
                }
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(3, 10)));
                wait5sec.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Send button"]))).Click();
                try
                {
                    wait5sec.Until(ExpectedConditions.ElementIsVisible(By.CssSelector($"img[alt*=\"{text.Split("\r\n").First()}\"]")));
                }
                catch { } // checking successful sent + some wait
                return true;
            }
            catch { return false; }
            finally
            {
                wait5sec = null;
                wait2sec = null;
            }
        }
        /// <summary>
        /// Searching for a web element using a locator to insert content and checking the element's availability after
        /// </summary>
        private static void SendKeysWithWait(By locator, string[] content = default)
        {
            int counter = 0;
            IWebElement element = default;
            WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(2));
            while (counter < 2)
            {
                try
                {
                    //element = wait.Until(ExpectedConditions.ElementToBeClickable(locator));
                    element = wait.Until(d => { 
                        IWebElement? e = default;
                        try { return d.FindElement(locator); }
                        catch { Errors += $"Selector not found: {locator.Criteria}\n"; return e; }});
                    wait.Until(ExpectedConditions.ElementToBeClickable(element));
                    int counter1 = 0;
                    for (int i = 0; i < content.Length; i++)
                    {
                        element.SendKeys(content[i]);
                        while (counter1 < 5)
                        {
                            Thread.Sleep(200);
                            if (element.Enabled == true) break;
                            else
                            {
                                counter1++;
                                if (counter1 == 5)
                                {
                                    Errors += $"\nSendKeysWithWait error:\n{locator.Criteria}";
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                catch { counter++; }
            }
            wait = default;
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
