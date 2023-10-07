using OpenQA.Selenium;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WSMS.Models;

namespace WSMS.Services
{
    internal class MessageService
    {
        public static BitmapSource GetImage(string url)
        {
            BitmapImage bi = new ();
            bi.BeginInit();
            bi.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            return bi;
        }
        private Dictionary<string, string> ElementsPaths = new()
        {
            { "Search field", ".to2l77zo.gfz4du6o.ag5g9lrv.bze30y65.kao4egtt.qh0vvdkp .selectable-text.copyable-text.iq0m558w.g0rxnol2" },
            { "Message input", "._3Uu1_ .selectable-text.copyable-text.iq0m558w.g0rxnol2" },
            { "Send button", "._2xy_p._3XKXx" }
           // { "Button delete", "//*[@aria-label='Отменить поиск']" }
        };
        public void StartSending(IWebDriver driver, Message message, string contact = default)
        { //@D:\Notes\Работа Вова\Discount\Burs.jpeg
            try
            {
                contact = "Виктор и я";
                var waitTime = TimeSpan.FromMilliseconds(7000);
                WebDriverWait wait = new(driver, waitTime) { PollingInterval = TimeSpan.FromMilliseconds(300) };
                IWebElement searchField = driver.FindElement(By.CssSelector(ElementsPaths["Search field"]));
                wait.Until(e => { searchField.SendKeys(contact); return true; });
                IWebElement foundContact = driver.FindElement(By.XPath($"//span[@class='matched-text _11JPr'][text()='{contact}']")); // css selector can not to search by text into web element
                wait.Until(e => { foundContact.Click(); return true; });
                IWebElement messageInput = driver.FindElement(By.CssSelector(ElementsPaths["Message input"]));
                //Clipboard.SetImage(message.Image.ToString());
                //wait.Until(e => { messageInput.SendKeys(Keys.LeftShift + Keys.Insert); return true; });
                wait.Until(e => { messageInput.SendKeys(message.Image.ToString()); return true; });
                wait.Until(e => { messageInput.SendKeys(message.Text); return true; });
                IWebElement sendButton = driver.FindElement(By.CssSelector(ElementsPaths["Send button"]));
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(5, 15)));
                wait.Until(e => { sendButton.Click(); return true; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WebService.CloseBrowser();
            }
        }
    }
}

