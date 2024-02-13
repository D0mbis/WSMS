using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;
using WSMS.Models;

namespace WSMS.Services
{
    internal class MessageService
    {
        /* TODO:
            1. Interactive progress sending
            2. upgrade Errors message (like hide "time out waitings")
            3. Test start button wich checking all selectors paths*
        */

        public static BitmapSource GetImage(string url)
        {
            BitmapImage bi = new();
            bi.BeginInit();
            bi.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            Message.Image = bi;
            return bi;
        }

        public static void StartSending(Message message)
        {
            Dictionary<string, List<string>> resultSending = new();
            if (WebService.IsRunning)
            {
                resultSending = SendMessage(message);
                if (resultSending["Not sent"].Count > 0)
                {
                    var result = MessageBox.Show($"Was not sent {resultSending["Not sent"].Count} messages, do you want to resend for them?",
                                   "Resemding not sent messages", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        message.Contacts = resultSending["Not sent"].ToArray();
                        Dictionary<string, List<string>> tempResultD = SendMessage(message);
                        resultSending["Successful sent"].AddRange(tempResultD["Successful sent"]);
                        resultSending["Not sent"] = tempResultD["Not sent"];
                        WebService.CloseBrowser();
                    }
                }
                //logs
                resultSending["Message Text"] = message.Text.Split("\n").ToList();
                Logger.SaveSendingLogs(resultSending);
            }
            else
            {
                MessageBox.Show("Please, start the browser first.");
            }
        }
        
        private static Dictionary<string, List<string>> SendMessage(Message message)
        {
            Dictionary<string, List<string>> outputD = new();
            outputD["Successful sent"] = new List<string>();
            outputD["Not sent"] = new List<string>();
            //string[] contacts = message.Contacts.Split("\r\n");
            for (int i = 0; i < message.Contacts.Length; i++)
            {
                string contact = message.Contacts[i];
                if (WebService.ToSend(contact, message.Text, Message.Image))
                {
                        outputD["Successful sent"].Add(contact);
                }
                else
                {
                        outputD["Not sent"].Add(contact);
                }
            }
            return outputD;
        }
    }
}

