using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Services;
using WSMS.ViewModels.Base;

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
        #region Message 
        private string[] contacts = { };
        public string Contacts
        {
            get { if (contacts.Length > 0) return string.Join("\r\n", contacts); else return default; }
            set { Set(ref contacts, value.Split("\r\n")); }
        }
        private string messageText;
        public string MessageText { get => messageText; set => Set(ref messageText, value); }
        public BitmapSource Image { get; } = Message.Image;
        #endregion
        #region Comands
        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object parameter) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion
        #region Start/Close browser Comand
        public ICommand StartBrowserCommand { get; }
        private bool CanStartBrowserCommandExecute(object parameter) => true;
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
        private bool CanStartSendingCommandExecute(object parameter)
        {
            if (MessageText != default && MessageText != "") { return true; }
            return false;
        }
        private void OnStartSendingCommandExecuted(object p)
        {
            Message message = new(contacts, messageText);
            MessageService.StartSending(message);
        }
        #endregion
        #region Check delivery command
        private string identifierText;
        public string IdentifierText { get => identifierText; set => Set(ref identifierText, value); }
        public ICommand CheckDeliveryCommand { get; }
        private bool CanStartCheckDeliveryCommandExecute(object parameter)
        {
            if (IdentifierText != default && MessageText != "") {
                return true; }
            return false;
        }
        private void OnStartCheckDeliveryCommandExecuted(object p)
        {
           Contacts = string.Join("\n", WebService.GetNotDeliveredContacts(contacts, IdentifierText));
        }
        #endregion
        #endregion
        public MainWindowViewModel()
        {
            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            StartBrowserCommand = new ActionCommand(OnStartBrowserCommandExecuted, CanStartBrowserCommandExecute);
            StartSendingCommand = new ActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
            CheckDeliveryCommand = new ActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
        }
    }
}