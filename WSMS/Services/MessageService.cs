using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WSMS.Models;
using WSMS.Models.Base;

namespace WSMS.Services
{
    public class MessageService
    {
        /* TODO:
            1. Interactive progress sending
            2. upgrade Errors message (like hide "time out waitings")
            3. Test previous start button wich checking all selectors paths*
        */
        private static readonly string FolderPath = $"{Environment.CurrentDirectory}\\data\\messages";
        public static void LoadMessages()
        {
            string messagesFilePath = FolderPath + "\\all messages.json";
            if (!File.Exists(messagesFilePath)) { using FileStream stream = new(messagesFilePath, FileMode.Create); }
            using StreamReader reader = new(messagesFilePath);
            var jsonData = reader.ReadToEnd();
        }
        public static bool SaveMesage(MessageWrapper message)
        {
            try
            {
                string json = JsonConvert.SerializeObject(message, Formatting.Indented);
                if (json != null && message.Message.Image != null)
                {
                    if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); } // если ошибка из-за фото, убрать фото из класса Message
                    using StreamWriter stream = new($"{FolderPath}\\all messages.json", true);
                    stream.WriteLine(json);
                    SaveImage(message.Message.Image, message.Message.ImagePath);
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "MessageService", "SaveMessage method error.");
                return false;
            }
        }

        private static void SaveImage(BitmapSource bitmap, string filePath)
        {
            BitmapEncoder encoder;
            // Select encoder based on file extension
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    MessageBox.Show("Unsupported file format");
                    return;
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // Save the encoded image to the file
            try
            {
                string imageFolderPath = FolderPath + "\\images";
                if (!Directory.Exists(imageFolderPath)) { Directory.CreateDirectory(imageFolderPath); }
                string fileName = filePath.Substring(filePath.LastIndexOf("\\"));
                using (var fileStream = new FileStream(imageFolderPath + fileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
                MessageBox.Show($"Message saved to {FolderPath}");
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "MessageService", "SaveImage");
            }
        }

        public static BitmapSource GetImage(string url)
        {
            BitmapImage bi = new();
            bi.BeginInit();
            bi.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            //Message.Image = bi;
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
                        //message.Contacts = resultSending["Not sent"].ToArray();
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
        public static ObservableCollection<CustomersCategoryFull> ChangeIsCheck(ObservableCollection<CustomersCategoryFull> customersCategories, bool flag)
        {
            foreach (var mainCategory in customersCategories)
            {
                foreach(var category in mainCategory.SubCategories)
                {
                    category.IsChecked = flag;
                }
            }
            return customersCategories; 
        }
        private static Dictionary<string, List<string>> SendMessage(Message message)
        {
            int contactsCount = 0;
            Dictionary<string, List<string>> outputD = new();
            outputD["Successful sent"] = new List<string>();
            outputD["Not sent"] = new List<string>();
            //string[] contacts = message.Contacts.Split("\r\n");
            for (int i = 0; i < contactsCount; i++)
            {
                string contact = "contact"; //message.Contacts[i];
                if (WebService.ToSend(contact, message.Text, GetImage("D:/Notes/Работа Вова/Discount/39.png")))
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

