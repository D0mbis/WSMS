using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WSMS.Models;
using WSMS.Services;
using WSMS.Views.Windows;
using System;
using WSMS.Models.Base;
using System.ComponentModel;
using System.Windows.Data;
using WSMS.Infrastructure.Commands.Base;
using System.Linq;
using WSMS.Infrastructure.Other;
using WSMS.Views;

namespace WSMS.ViewModels
{
    public class MainWindowViewModel : Model
    {
        #region Properties
        private string title = "WSMS";
        public string Title { get => title; set => Set(ref title, value); }

        private string driverBtnContent = "Start browser";
        public string DriverBtnContent { get => driverBtnContent; set => Set(ref driverBtnContent, value); }

        private ICollectionView templates;
        public ICollectionView Templates { get => templates; set => Set(ref templates, value); }

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
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(window => window.IsVisible);
            CustomersWindow customersWindow = new()
            {
                Left = mainWindow.Left - mainWindow.Width / 2,
                Top = mainWindow.Top + 50
            };
            customersWindow.Show();
        }
        #endregion
        #region LoadCustomersCommand
        public ICommand LoadCustomersCommand { get; }
        #endregion
        #region Start/Close browser Comand
        public ICommand StartBrowserCommand { get; }
        private bool CanStartBrowserCommandExecute(object p) => true;
        private void OnStartBrowserCommandExecuted(object p)
        {
            CreateSendingWindow createSendingWindow = new();
            createSendingWindow.Show();
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

        #region CloseAppCommand
        public ICommand CloseApplicationCommand { get; }
        private void OnCloseApplicationExecuted(object p)
        {
            var window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(window => window.IsVisible);
            if (window != null)
            {
                WindowPositionSettings.SaveWindowPosition(window);
                window.Hide();
                window.Close();
            }
        }
        #endregion
        #region TemplatesUpdate
        private ICommand templatesUpdateCommand;
        public ICommand TemplatesUpdateCommand => templatesUpdateCommand ??= new MyActionCommand(OnTemplatesUpdateCommandExecuted);

        private void OnTemplatesUpdateCommandExecuted(object obj)
        {
            Templates = CollectionViewSource.GetDefaultView(MessageService.loadMessageTemplates() ?? new ObservableCollection<SendingTemplate>());
        }
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            StartBrowserCommand = new MyActionCommand(OnStartBrowserCommandExecuted, CanStartBrowserCommandExecute);
            StartSendingCommand = new MyActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
            CheckDeliveryCommand = new MyActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
            OpenContactsCommand = new MyActionCommand(OnOpenContactsCommandExecuted, CanOpenContactsCommandExecute);
            CloseApplicationCommand = new MyActionCommand(OnCloseApplicationExecuted);
            Templates = CollectionViewSource.GetDefaultView(MessageService.loadMessageTemplates() ?? new ObservableCollection<SendingTemplate>());
        }
    }
}