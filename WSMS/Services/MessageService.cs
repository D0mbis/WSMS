using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WSMS.Models;

namespace WSMS.Services
{
    internal class MessageService
    {
        private static readonly string LogsDirectoryPath = $"{Environment.CurrentDirectory}\\Logs";
        private static readonly string LogsFilePath = LogsDirectoryPath + $"\\{DateTime.Now.ToString("dd-MM-yy (HH.mm.ss)")}.txt";
        public static BitmapSource GetImage(string url)
        {
            BitmapImage bi = new();
            bi.BeginInit();
            bi.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            bi.EndInit();
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
                        //logs
                        resultSending["Message Text"] = message.Text.Split("\n").ToList();
                        ToWriteLogs(resultSending);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please, start the browser first.");
            }
        }
        private static void ToWriteLogs(Dictionary<string, List<string>> resultSending)
        {
            try
            {
                if (!Directory.Exists(LogsDirectoryPath)) { Directory.CreateDirectory(LogsDirectoryPath); }

                File.Create(LogsFilePath).Close();
                if (resultSending["Successful sent"].Count > 0 || resultSending["Not sent"].Count > 0)
                {
                    using (StreamWriter stream = new(LogsFilePath, true))
                    {
                        if (resultSending["Successful sent"].Count > 0)
                        {
                            stream.WriteLine($"Total was successfully sent {resultSending["Successful sent"].Count} messages for:\n"
                                + string.Join("\n", resultSending["Successful sent"]) + "\n");
                        }
                        if (resultSending["Not sent"].Count > 0)
                        {
                            stream.WriteLine($"Not sent {resultSending["Not sent"].Count} messages:\n" +
                                string.Join("\n", resultSending["Not sent"]) + "\n");
                            stream.WriteLine($"Message text was:\n{string.Join("\n", resultSending["Message Text"])}\n");
                            for (int i = 0; i < 10; i++) stream.Write("_");
                            stream.WriteLine($"\nErrors:\n{WebService.Errors}");
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Logs error:\n{ex.Message}"); }
        }
        private static Dictionary<string, List<string>> SendMessage(Message message)
        {
            Dictionary<string, List<string>> outputD = new();
            outputD["Successful sent"] = new List<string>();
            outputD["Not sent"] = new List<string>();
            //string[] contacts = message.Contacts.Split("\r\n");
            for (int i = 0; i < message.Contacts.Length; i++)
            {
                if (WebService.ToSend(message.Contacts[i].ToString(), message.Text, message.Image))
                {
                    outputD["Successful sent"].Add(message.Contacts[i].ToString());
                }
                else
                {
                    outputD["Not sent"].Add(message.Contacts[i].ToString());
                }
            }
            return outputD;
        }
    }
}

