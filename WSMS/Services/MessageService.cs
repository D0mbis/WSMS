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
            3. Test previous start button wich checking all selectors paths*
        */
        private static readonly string FolderPath = $"{Environment.CurrentDirectory}\\data\\messages";
        public static ObservableCollection<MessageWrapper> LoadMessages()
        {
            try
            {
                if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); }
                string messagesFilePath = FolderPath + "\\all messages.json";
                if (!File.Exists(messagesFilePath))
                {
                    using FileStream stream = new(messagesFilePath, FileMode.Create);
                    return new ObservableCollection<MessageWrapper>() { new MessageWrapper(new Message()) };
                }
                else
                {
                    string jsonData = string.Empty;
                    using (StreamReader reader = new(messagesFilePath))
                    {
                        jsonData = reader.ReadToEnd();
                    }
                    // ObservableCollection<MessageWrapper>[] temp = Array.Empty<ObservableCollection<MessageWrapper>>();
                    var temp = JsonConvert.DeserializeObject<ObservableCollection<MessageWrapper>>(jsonData);
                    if (temp != null)
                        foreach (var item in temp)
                        {
                            item.Message.Image = GetImage(item.Message.ImagePath);
                        }
                    return temp ?? new ObservableCollection<MessageWrapper>() { new(new Message()) };
                }
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "MessageService", "LoadMessages error.");
                return new ObservableCollection<MessageWrapper>() { new(new Message()) };
            }
        }
        public static ObservableCollection<CustomersCategoryFull> RemoveUnselectedCategories(ObservableCollection<CustomersCategoryFull> categories)
        {
            var sortedCategories = categories
                        .OrderBy(c => c.MainCategory).Where(c => c.IsChecked);
            var finaly = new ObservableCollection<CustomersCategoryFull>();
            foreach (var fullCategory in sortedCategories)
            {
                var temp = fullCategory.SubCategories.OrderBy(c => c.Name).Where(c => c.IsChecked);
                finaly.Add(new CustomersCategoryFull(fullCategory.MainCategory, new ObservableCollection<SubCategory>(temp)));
            }
            return finaly;
        }
        /// <summary>
        /// True = delete selected message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public static bool UpdateMessages(MessageWrapper message, bool delete = false)
        {
            try
            {
                var allMessages = LoadMessages();
                if (allMessages.Count == 1 && allMessages[0].Message.Name == string.Empty)
                {
                    allMessages[0] = message;
                }
                else
                {
                    for (int i = 0; i < allMessages.Count; i++)
                    {
                        if (allMessages[i].Message.Name == message.Message.Name)
                        {
                            if (delete)
                            {
                                allMessages.Remove(allMessages[i]);
                                if (allMessages.Count == 0)
                                {
                                    allMessages.Add(new MessageWrapper(new Message()));
                                }
                            }
                            else
                            {
                                allMessages[i] = message;
                            }
                            break;
                        }
                        else if (i == allMessages.Count - 1)
                        {
                            allMessages.Add(message);
                        }
                    }
                }
                if (!delete)
                {
                    message.Message.ImagePath = SaveImage(message.Message.Image);
                }
                string json = JsonConvert.SerializeObject(allMessages, Formatting.Indented);

                if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); }
                using StreamWriter stream = new($"{FolderPath}\\all messages.json", false);
                stream.Write(json);
                return true;
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "MessageService", "SaveMessage method error.");
                return false;
            }
        }
        private static string SaveImage(BitmapSource bitmap)
        {
            BitmapEncoder encoder;
            string filePath = bitmap.ToString();
            // Select encoder based on file extension
            string extension = Path.GetExtension(filePath.ToLower());
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
                    return string.Empty;
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // Save the encoded image to the file
            try
            {
                string imageFolderPath = FolderPath + "\\images";
                if (!Directory.Exists(imageFolderPath)) { Directory.CreateDirectory(imageFolderPath); }
                string fileName = filePath.Substring(filePath.LastIndexOf("/"));
                fileName = (fileName[0] == '/') ? fileName.Remove(0, 1) : fileName;
                string newImagePath = imageFolderPath + "\\" + fileName;
                if (!File.Exists(newImagePath))
                    using (var fileStream = new FileStream(newImagePath, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                MessageBox.Show($"Message saved to {FolderPath}");
                return newImagePath;
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "MessageService", "SaveImage");
                return string.Empty;
            }
        }
        public static BitmapSource GetImage(string url)
        {
            if (url != string.Empty)
            {
                BitmapImage bi = new();
                bi.BeginInit();
                bi.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                //Message.Image = bi;
                return bi;
            }
            return new BitmapImage(new Uri("pack://application:,,,/data/messages/placeholder.png"));
        }
        public static ObservableCollection<CustomersCategoryFull> ChangeIsCheck(ObservableCollection<CustomersCategoryFull> customersCategories, bool flag)
        {
            foreach (var mainCategory in customersCategories)
            {
                foreach (var category in mainCategory.SubCategories)
                {
                    category.IsChecked = flag;
                }
            }
            return customersCategories;
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
                // Logger.SaveSendingLogs(resultSending);
            }
            else
            {
                MessageBox.Show("Please, start the browser first.");
            }
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

