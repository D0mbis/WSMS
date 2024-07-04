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
        private List<CustomersGroup> customerGroups;
        public List<CustomersGroup> CustomerGroups
        {
            get => customerGroups; set => Set(ref customerGroups, value);
        }
        #region Message
        private string messageText;
        public string MessageText { get => messageText; set => Set(ref messageText, value); }
        public BitmapSource Image { get; } = Message.Image;
        #endregion
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
            CustomerGroups = CustomersService.LoadCustomersGroup();
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
            if (MessageText != default && MessageText != "") { return true; }
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
            if (IdentifierText != default && MessageText != "")
            {
                return true;
            }
            return false;
        }
        private void OnStartCheckDeliveryCommandExecuted(object p)
        {
            // Contacts = string.Join("\n", WebService.GetNotDeliveredContacts(contacts, IdentifierText));
        }
        #endregion
        #endregion
        public MainWindowViewModel()
        {
            StartBrowserCommand = new ActionCommand(OnStartBrowserCommandExecuted, CanStartBrowserCommandExecute);
            StartSendingCommand = new ActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
            CheckDeliveryCommand = new ActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
            LoadCustomersCommand = new ActionCommand(OnStartLoadCustomersCommandExecuted, CanLoadCustomersCommandExecute);
            OpenContactsCommand = new ActionCommand(OnOpenContactsCommandExecuted, CanOpenContactsCommandExecute);
        }
    }
}