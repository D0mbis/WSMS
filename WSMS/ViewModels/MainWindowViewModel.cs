using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Services;
using WSMS.ViewModels.Base;
using WSMS.Views.Windows;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System;

namespace WSMS.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title Window
        private string _Title = "WSMS";
        /// <summary Header MainWindow </summary>
		public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion
        #region driverButton Content
        private string driverBtnContent = "Start browser";
        public string DriverBtnContent { get => driverBtnContent; set => Set(ref driverBtnContent, value); }
        #endregion
        private ExpandMessage message;
        private string messageText;
        private BitmapSource image;
        private List<CustomersGroup> customerGroups;
        public List<CustomersGroup> CustomerGroups
        {
            get => customerGroups; set => Set(ref customerGroups, value);
        }
        public BitmapSource Image
        {
            get => image;
            set => Set(ref image, value);
        }
        public ObservableCollection<Message> MessagesCollection { get; set; }
        public ExpandMessage Message
        {
            get => message;
            set
            {
                Set(ref message, value);
            }
        }
        public string MessageText
        {
            get => messageText;
            set
            {
                if (value == string.Empty)
                {
                    message.IsChanged = false;
                }
                else if (messageText == string.Empty)
                {
                    message.Text = value;
                    if (message.OldTextMessage != value)
                    {
                        message.IsChanged = true;
                    }
                }
                else if (message.OldTextMessage != value)
                {
                    message.IsChanged = true;
                    message.Text = value;
                }
                else
                {
                    message.IsChanged = false;
                }
                Set(ref messageText, value);
            }
        }

        #region Comands
        #region OpenContactsCommand

        public ICommand OpenContactsCommand { get; }

        private bool CanOpenContactsCommandExecute(object p)
        {
            if (!CustomersWindow.IsOpen) return true;
            return false;
        }

        private void OnOpenContactsCommandExecuted(object p)
        {
            CustomersWindow customersWindow = new();
            customersWindow.Show();
        }
        #endregion
        #region LoadCustomersCommand
        public ICommand LoadCustomersCommand { get; }

        private bool CanLoadCustomersCommandExecute(object p)
        {
            if (CustomerGroups == null) { return true; }
            return false;
        }
        private void OnStartLoadCustomersCommandExecuted(object p)
        {
            CustomerGroups = CustomersService.AllCustomersInGroups;
        }

        #endregion
        #region Start/Close browser Comand
        public ICommand StartBrowserCommand { get; }
        private bool CanStartBrowserCommandExecute(object p) => true;
        private void OnStartBrowserCommandExecuted(object p)
        {

            if (DriverBtnContent == "Start browser")
            {
                WebService.OpenBrowser();
                DriverBtnContent = "Close browser";
            }
            else
            {
                WebService.CloseBrowser();
                DriverBtnContent = "Start browser";
            }
        }
        #endregion
        #region Start sending Command
        public ICommand StartSendingCommand { get; }
        private bool CanStartSendingCommandExecute(object p)
        {
            return false;
        }
        private void OnStartSendingCommandExecuted(object p)
        {
            /* Message message = new(contacts, messageText);
             MessageService.StartSending(message);*/
        }
        #endregion
        #region Check delivery command
        private string identifierText;
        public string IdentifierText { get => identifierText; set => Set(ref identifierText, value); }
        public ICommand CheckDeliveryCommand { get; }
        private bool CanStartCheckDeliveryCommandExecute(object p)
        {
            return false;
        }
        private void OnStartCheckDeliveryCommandExecuted(object p)
        {
            // Contacts = string.Join("\n", WebService.GetNotDeliveredContacts(contacts, IdentifierText));
        }
        #endregion
        #region SaveMessage Command
        public ICommand SaveMessageCommand { get; }
        private bool CanSaveMessageCommandExecute(object p)
        {
            if (message.IsChanged && message.Image != null)
            {
                return true;
            }
            return false;
        }
        private void OnSaveMessageCommandExecuted(object p)
        {
            SaveMessageWindow saveMessageWindow = new();
            saveMessageWindow.Show();
            /*if (MessageService.SaveMesage(Message))
                message.IsChanged = false;*/
        }
        #endregion
        #region ImageDrop Command
        public ICommand ImageDropCommand { get; }
        private void OnImageDropCommandExecuted(object p)
        {
            if (p is DragEventArgs e && e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    var filePath = files[0];
                    try
                    {
                        Image = MessageService.GetImage(filePath);
                        message.Image = Image;
                        message.ImagePath = filePath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image: {ex.Message}");
                    }
                }
            }
        }
        #endregion
        #endregion
        public MainWindowViewModel()
        {
            MessagesCollection = new ObservableCollection<Message>
            {
                new() { Text = "First message", Image = MessageService.GetImage("D:/Notes/Работа Вова/Discount/39.png") },
                new () { Text = "Second messageSecond messageSecond messageSecond messageSecond messageSecond message", Image = MessageService.GetImage("D:/Notes/Работа Вова/Discount/35.png") }
            };
            Image = MessageService.GetImage("D:/Notes/Работа Вова/Discount/39.png");
            message = new ExpandMessage();
            message.OldTextMessage = messageText = message.Text;
            StartBrowserCommand = new Infrastructure.Commands.Base.ActionCommand(OnStartBrowserCommandExecuted, CanStartBrowserCommandExecute);
            StartSendingCommand = new Infrastructure.Commands.Base.ActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
            CheckDeliveryCommand = new Infrastructure.Commands.Base.ActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
            LoadCustomersCommand = new Infrastructure.Commands.Base.ActionCommand(OnStartLoadCustomersCommandExecuted, CanLoadCustomersCommandExecute);
            OpenContactsCommand = new Infrastructure.Commands.Base.ActionCommand(OnOpenContactsCommandExecuted, CanOpenContactsCommandExecute);
            SaveMessageCommand = new Infrastructure.Commands.Base.ActionCommand(OnSaveMessageCommandExecuted, CanSaveMessageCommandExecute);
            ImageDropCommand = new Infrastructure.Commands.Base.ActionCommand(OnImageDropCommandExecuted);
        }
    }
}