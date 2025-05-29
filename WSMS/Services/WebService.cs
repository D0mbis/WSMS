using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            { "Message input", "div[aria-placeholder='Введите сообщение']" },
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
                var searchField = FindElementWithWait(ElementsPaths["Search field"], 2);
                if (searchField != null)
                {
                    return true;
                }
                else
                {
                    var QRcode = FindElementWithWait((ElementsPaths["QRcode"]), 2);
                    if (QRcode != null)
                    {
                        MessageBoxResult result = CustomMessageBox.ShowTopMost($"The previous session has expired. To continue, please scan the QR code with your phone and press \"OK.\"",
                        "Account is not authorized.", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (result == MessageBoxResult.OK)
                        {
                            searchField = FindElementWithWait(ElementsPaths["Search field"], 2);
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
            SendKeysWithWait(ElementsPaths["Search field"], new string[] { "", contact });
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
                SendKeysWithWait(ElementsPaths["Search field"], new string[] { "", contact });
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
                    SendKeysWithWait(ElementsPaths["Message input"], new string[] { "", text });
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
                    IWebElement fileInput3 = wait2sec.Until(ExpectedConditions.ElementExists(By.CssSelector("input[accept='image/*,video/mp4,video/3gpp,video/quicktime']")));
                    fileInput3.SendKeys(imagePath);

                }
                catch (Exception ex)
                {
                    Errors += $"Contact name: {contact} {DateTime.Now}\nClipboard error:\n{ex.Message}\n";
                }
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(3, 10)));
                wait5sec.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(ElementsPaths["Send button"]))).Click();
                return true;
            }
            catch { return false; }
        }


        /// <summary>
        /// Searching for a web element using a locator to insert content and checking the element's availability after
        /// </summary>

        private static IWebElement FindElementWithWait(string locator, int time)
        {
            IWebElement? element = default;
            int counter = 0;
            string? caller = null;
            var stackTrace = new StackTrace();
            if (stackTrace.FrameCount > 1)
                caller = stackTrace.GetFrame(1)?.GetMethod()?.Name;
            while (counter < 2)
            {
                try
                {
                    WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(time));
                    element = wait.Until(d => d.FindElement(By.CssSelector(locator)));
                    return element;
                }
                catch
                {
                    counter++;
                }
            }
            Logger.ShowMyReportMessageBox("Element was not found:", "WebServiceErrors", $" {locator} (called from: {caller})", false);
            return element;
        }

        private static void SendKeysWithWait(string locator, string[]? content = default)
        {
            int counter = 0;

            while (counter < 2)
            {
                try
                {
                    //IWebElement element = FindElementWithWait(locator, 2);
                    //WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(2));
                    // wait.Until(ExpectedConditions.ElementToBeClickable(element));
                    // element.Clear(); // Очищаем поле перед вводом (опционально)
                    IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                    string script = $@"
            const editor = document.querySelector(""{locator}"");
            if (editor) {{
                editor.focus();
                document.execCommand('insertText', false, arguments[0]);
                return true;
            }}
            return false;
        ";

                    if (content != null)
                    {
                        foreach (var text in content)
                        {
                            if (text != "")
                            {
                                int retryCount = 0;
                                while (retryCount < 5)
                                {
                                    try
                                    {
                                        //   element.SendKeys(text);
                                        bool success = (bool)js.ExecuteScript(script, text);
                                        //js.ExecuteScript("arguments[0].value = arguments[1];", element, text);
                                        // element.SendKeys(Keys.Shift + Keys.Enter);
                                        Task.Delay(100); // Краткая задержка для стабильности
                                        break; // Успешно, выходим из внутреннего цикла
                                    }
                                    catch (StaleElementReferenceException)
                                    {
                                        // Элемент устарел, пытаемся найти заново
                                        // element = FindElementWithWait(locator, 2);
                                        retryCount++;
                                        Task.Delay(200);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.ShowMyReportMessageBox(ex.Message, "WebServiceErrors", $"SendKeysWithWait->locator: {locator}", false);
                                        retryCount++;
                                        Task.Delay(200);
                                        if (retryCount == 5)
                                        {
                                            // Errors += $"\nSendKeysWithWait error:\n{locator.Criteria}";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break; // Успешно, выходим из внешнего цикла
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
        private static async Task SendKeysWithWait3(string locator, string[]? content = default)
        {
            int counter = 0;

            while (counter < 2)
            {
                try
                {
                    IWebElement element = await Task.Run(() => FindElementWithWait(locator, 2));
                    WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(2));
                    await Task.Run(() => wait.Until(ExpectedConditions.ElementToBeClickable(element)));

                    element.Clear(); // Очищаем поле перед вводом (опционально)
                    if (content != null)
                    {
                        foreach (var text in content)
                        {
                            int retryCount = 0;
                            while (retryCount < 5)
                            {
                                try
                                {
                                    element.SendKeys(text);
                                    element.SendKeys(Keys.Shift + Keys.Enter);
                                    await Task.Delay(100); // Краткая задержка для стабильности
                                    break; // Успешно, выходим из внутреннего цикла
                                }
                                catch (StaleElementReferenceException)
                                {
                                    // Элемент устарел, пытаемся найти заново
                                    element = await Task.Run(() => FindElementWithWait(locator, 2));
                                    retryCount++;
                                    await Task.Delay(200);
                                }
                                catch (Exception ex)
                                {
                                    Logger.ShowMyReportMessageBox(ex.Message, "WebServiceErrors", $"SendKeysWithWait->locator: {locator}", false);
                                    retryCount++;
                                    await Task.Delay(200);
                                    if (retryCount == 5)
                                    {
                                        Errors += $"\nSendKeysWithWait error:\n{locator}";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break; // Успешно, выходим из внешнего цикла
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

        private static void SendKeysWithWait2(string locator, string[]? content = default)
        {
            int counter = 0;
            IWebElement element = FindElementWithWait(locator, 2);
            WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(2));
            while (counter < 2)
            {
                try
                {
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
                                    Errors += $"\nSendKeysWithWait error:\n{locator}";
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                catch { counter++; }
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

