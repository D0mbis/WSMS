using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WSMS.Infrastructure.Commands;
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
        private string messageText = default;

        public string MessageText { get => messageText; set => Set(ref messageText, value); }
        private BitmapSource image = MessageService.GetImage("D:\\Notes\\Работа Вова\\Discount\\DubleRotor.png");
        public BitmapSource Image { get => image; set => Set(ref image, value); }
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
                WebService.StartBrowser();
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
            Message message = new(messageText, image);
            WebService.StartSending(message);
        }
        #endregion
        #endregion
        public MainWindowViewModel()
        {
            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            StartBrowserCommand = new ActionCommand(OnStartBrowserCommandExecuted, CanStartBrowserCommandExecute);
            StartSendingCommand = new ActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
        }



    }
}
