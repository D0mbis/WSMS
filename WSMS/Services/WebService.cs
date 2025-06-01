using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WSMS.Services
{
    public class WebService
    {
        private static IWebDriver Driver { get; set; }
        public static string Errors { get; set; }
        public static bool IsRunning { get; set; } = false;
        private static readonly string Url = "https://web.whatsapp.com/";
        private static readonly Dictionary<string, string> ElementsPaths = new()
        {
            { "Search field", ".x1n2onr6.xh8yej3.lexical-rich-text-input div"},
            { "Message input", "div[aria-label='Введите сообщение'][role='textbox'][contenteditable='true']" },
            { "Send button", "div[aria-label='Отправить']" },
            { "Delete img btn", "div[aria-label='Закрыть']" },
            { "Delete SearchText btn", "button[aria-label='Отменить поиск']" },
            { "QRcode", "canvas[aria-label='Scan this QR code to link a device!']" }
        };

        //
        public static void OpenBrowser(string accountName)
        {
            /*"As of Selenium 4.6, Selenium downloads the correct driver for you. You shouldn’t need to do anything. (from ducumentation)
            ChromeDriverService service = ChromeDriverService.CreateDefaultService("PATH of chromedriver.exe folder");
            */


            // НУЖНО СДЕЛАТЬ УНИКАЛЬНЫЙ ЭКЗ DRIVER для каждой сессии чтобы можно было использовать одновременно, открывать и закрывать,
            // ТОЛЬКО В СЛУЧАЕ ЕСЛИ УДАСТСЯ избавиться от работы с буфером обмена
            ChromeOptions options = new();
            string profileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users", accountName);
            options.AddArgument($"--user-data-dir={profileDirectory}");
            options.AddUserProfilePreference("intl.accept_languages", "ru-RU");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
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
            if (!CheckAuthorization())
                CloseBrowser(accountName);
        }

        private static bool CheckAuthorization()
        {
            bool notFound = true;
            while (notFound)
            {
                var searchField = FindElementWithWait(ElementsPaths["Search field"], 2000);
                if (searchField != null)
                {
                    return true;
                }
                else
                {
                    var QRcode = FindElementWithWait((ElementsPaths["QRcode"]), 2000);
                    if (QRcode != null)
                    {
                        MessageBoxResult result = CustomMessageBox.ShowTopMost($"The previous session has expired. To continue, please scan the QR code with your phone and press \"OK.\"",
                        "Account is not authorized.", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (result == MessageBoxResult.OK)
                        {
                            searchField = FindElementWithWait(ElementsPaths["Search field"], 2000);
                            if (searchField != null)
                            { return true; }
                            else
                            {
                                CustomMessageBox.ShowTopMost($"Authorization error, please try again.", "Account is not authorized.", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
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
            IWebElement element = FindElementWithWait(ElementsPaths["Search field"], 2000);
            element.SendKeys(contact);
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
                InsertTextWithWait(ElementsPaths["Search field"], contact);
            }

            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector($"[title='{contact}']"))).Click();
        }
        public static bool ToSend(string contact, string text, string imagePath)
        {
            string[] newtextMessage = text?.Replace("\\n", "\n").Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None) ?? Array.Empty<string>();

            WebDriverWait wait5sec = new(Driver, TimeSpan.FromMilliseconds(5000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            WebDriverWait wait2sec = new(Driver, TimeSpan.FromMilliseconds(2000)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
            try
            {
                SearchContact(wait5sec, contact);
                try
                {
                    InsertTextWithWait(ElementsPaths["Message input"], text);
                    var resault = wait2sec.Until(d => d.FindElement(By.CssSelector(ElementsPaths["Message input"])).Text.Contains(text));
                    if (!resault)
                    {
                        CustomMessageBox.ShowTopMost($"Error while insert text to {contact}.\n", "WebServiceErrors", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                    // Эмулируем клик по кнопке "Прикрепить" через JavaScript
                    string script = @"
                                      const attachButton = document.querySelector('button[title=""Прикрепить""]');
                                      if (attachButton) {
                                          attachButton.click();
                                          return true;
                                      }
                                      return false;
                                    ";
                    bool attachSuccess = (bool)js.ExecuteScript(script);

                    // Ждём появления меню
                    wait2sec.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[role='application']")));
                    IWebElement fileInput3 = FindElementWithWait("input[accept='image/*,video/mp4,video/3gpp,video/quicktime']", 200, 10);
                    fileInput3.SendKeys(imagePath);

                    // Ждём появления элемента с изображением
                    FindElementWithWait("div .x1n2onr6.xvungr5.xminmjj", 200, 10);

                }
                catch (Exception ex)
                {
                    Errors += $"Contact name: {contact} {DateTime.Now}\nClipboard error:\n{ex.Message}\n";
                }
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(1, 3)));
                try
                {
                    var sendButton = FindElementWithWait(ElementsPaths["Send button"], 2000, 3);
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", sendButton);
                    //sendButton.Click();
                    try
                    {
                        var r = IsTextInputed(ElementsPaths["Message input"], text);
                        if (r) { return true; }
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.ShowTopMost($"Error while checking message inputed to {contact}.\n{ex.Message}", "WebServiceErrors", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            catch { return false; }
        }
        private static bool IsTextInputed(string locator, string text)
        {
            // Получаем первое слово из сообщения
            var sendTime = DateTime.Now;
            string firstWord = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];
            try
            {
                var messages = Driver.FindElements(By.CssSelector("div.x9f619.x1hx0egp"))
                    .Where(msg => msg.FindElements(By.CssSelector("span._ao3e.selectable-text.copyable-text span"))
                                   .Any(span => span.Text.Contains(firstWord)))
                    .ToList();

                foreach (var message in messages)
                {
                    // Находим элемент с временем
                    var timeElement = message.FindElement(By.CssSelector("span.x1rg5ohu.x16dsc37"));
                    string timeText = timeElement.GetAttribute("dir") == "auto" ? timeElement.Text : "";
                    IWebElement delivered = null;
                    try
                    {
                        delivered = FindElementWithWait("div.x1pn4fmt.x1rg5ohu.x1w4ip6v", 200, 10);

                      //  CustomMessageBox.ShowTopMost($"delivered: {delivered.Enabled}", "WebServiceErrors", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.ShowTopMost($"Error while checking delivery status: {ex.Message}", "WebServiceErrors", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (string.IsNullOrEmpty(timeText))
                        continue;

                    // Парсим время сообщения   x1pn4fmt x1rg5ohu x1w4ip6v
                    if (DateTime.TryParseExact(timeText, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime messageTime))
                    {
                        // Устанавливаем сегодняшнюю дату
                        messageTime = DateTime.Today.Add(messageTime.TimeOfDay);

                        // Проверяем условия:
                        // 1. Сообщение отправлено сегодня
                        // 2. Разница во времени не более 2 минут
                        // 3. Сообщение содержит блок с галочками о доставке
                        var timeDiff = sendTime - messageTime;
                        if (messageTime.Date == DateTime.Today &&
                            Math.Abs(timeDiff.TotalMinutes) <= 2 && delivered != null)
                        {
                            CustomMessageBox.ShowTopMost($"delivered: {delivered.Enabled}", "WebServiceErrors", MessageBoxButton.OK, MessageBoxImage.Information);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                CustomMessageBox.ShowTopMost($"Error while checking message inputed to {locator}.\n{DateTime.Now}", "WebServiceErrors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        /// <summary>
        /// Improved web element search with smart waits and retry attempts at configurable intervals.
        /// </summary>
        /// <param name="locator">String for only CSS locator</param>
        /// <param name="time">Wait time for WebDriverWait (in milliseconds)</param>
        /// <param name="interval">Time interval between repetitions (in milliseconds)</param>
        /// <param name="attempts">Number of attempts</param>
        /// <returns></returns>
        private static IWebElement FindElementWithWait(string locator, int time, int attempts = 2, int interval = 200)
        {
            IWebElement? element = default;
            int counter = 0;
            string? caller = null;
            var stackTrace = new StackTrace();
            if (stackTrace.FrameCount > 1)
                caller = stackTrace.GetFrame(1)?.GetMethod()?.Name;
            while (counter < attempts)
            {
                try
                {
                    WebDriverWait wait = new(Driver, TimeSpan.FromMilliseconds(time));
                    element = wait.Until(d => d.FindElement(By.CssSelector(locator)));
                    wait.Until(ExpectedConditions.ElementToBeClickable(element));
                    return element;
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(interval));
                    counter++;
                }
            }
            Logger.ShowMyReportMessageBox("Element was not found:", "WebServiceErrors", $" {locator} (called from: {caller})", false);
            return element;
        }
        private static void InsertTextWithWait(string locator, string content = default)
        {
            int counter = 0;

            while (counter < 2)
            {
                try
                {
                    IWebElement element = FindElementWithWait(locator, 2000);
                    WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(2));
                    wait.Until(ExpectedConditions.ElementToBeClickable(element));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                    Actions actions = new(Driver);
                    actions.Click(element)
                          .KeyDown(Keys.Control)
                          .SendKeys("a")
                          .KeyUp(Keys.Control)
                          .SendKeys(Keys.Delete)
                          .Perform();
                    // Ждем очистки
                    wait.Until(d => string.IsNullOrEmpty(element.Text));
                    string script = $@"const editor = document.querySelector(""{locator}"");
                                       if (editor) {{
                                           editor.focus();
                                           document.execCommand('insertText', false, arguments[0]);
                                           return true;
                                       }}
                                       return false;
                                     ";
                    if (content != null && content != "")
                    {
                        int retryCount = 0;
                        while (retryCount < 5)
                        {
                            try
                            {
                                js.ExecuteScript(script, content);
                                Task.Delay(100); // Краткая задержка для стабильности
                                break;
                            }
                            catch (StaleElementReferenceException)
                            {
                                element = FindElementWithWait(locator, 2000);
                                retryCount++;
                                Task.Delay(200);
                            }
                            catch (Exception ex)
                            {
                                retryCount++;
                                Task.Delay(200);
                                if (retryCount == 5)
                                {
                                    Logger.ShowMyReportMessageBox(ex.Message, "WebServiceErrors", $"SendKeysWithWait->locator: {locator}", false);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                catch (WebDriverTimeoutException ex)
                {
                    Logger.ShowMyReportMessageBox(ex.Message, "WebServiceErrors", $"WebService->SendKeysWithWait->locator: {locator}", false);
                    counter++;
                }
                catch (Exception ex)
                {
                    Logger.ShowMyReportMessageBox(ex.Message, "WebServiceErrors", $"SendKeysWithWait->locator: {locator}", false);
                    counter++;
                }
            }
        }
        public static void CloseBrowser(string accountName)
        {
            if (Driver != default && Driver != null)
            {
                Driver.Close(); Driver.Quit(); Driver.Dispose(); Driver = default;
                IsRunning = false;
                //string accountId = "+79953781761";
                CleanAccountFolders(accountName);
            }
        }
        private static void CleanAccountFolders(string accountId)
        {
            string profileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users", accountId);
            List<string> list = new()
            {
                "Default\\Cache",
                "Default\\GPUCache",
                "Default\\Code Cache",
                "optimization_guide_model_store",
                "GrShaderCache"
            };
            foreach (string s in list)
            {
                string unneccesaryFolder = Path.Combine(profileDirectory, s);
                if (Directory.Exists(unneccesaryFolder))
                {
                    Directory.Delete(unneccesaryFolder, true);
                }
            }
        }
    }

}

