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
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace WSMS.ViewModels
{
    public class MainWindowViewModel : Model
    {
        #region Properties
        private string title = "WSMS";
        public string Title { get => title; set => Set(ref title, value); }

        private ICollectionView templates;
        public ICollectionView Templates
        {
            get => templates; set =>
                Set(ref templates, value);
        }
        #endregion

        #region Comands
        #region OpenCustomrsCommand
        public ICommand OpenCustomersCommand { get; }
        private bool CanOpenCustomersCommandExecute(object p)
        {
            var openedWindow = Application.Current.Windows.OfType<CustomersWindow>().FirstOrDefault(w => w.IsVisible);
            return openedWindow == null ? true : false;

        }
        private void OnOpenCustomersCommandExecuted(object p)
        {
            CustomersWindow window = new();
            WindowMenager.OpenWindowCentered<MainWindow>(window);
        }
        #endregion
        #region LoadCustomersCommand
        public ICommand LoadCustomersCommand { get; }
        #endregion
        #region CreateSendingCommand
        ICommand createSendingCommand;
        public ICommand CreateSendingCommand => createSendingCommand ?? new MyActionCommand(OnCreateSendingCommandCommandExecuted, CanOpenCreateSendingWindow);
        private bool CanOpenCreateSendingWindow(object p)
        {
            var openedWindow = Application.Current.Windows.OfType<CreateSendingWindow>().FirstOrDefault(w => w.IsVisible);
            return openedWindow == null ? true : false;
        }
        private void OnCreateSendingCommandCommandExecuted(object p)
        {
            CreateSendingWindow window = new();
            WindowMenager.OpenWindowCentered<MainWindow>(window);
        }
        #endregion
        #region DeleteTemplateCommand
        ICommand deleteTemplateCommand;
        public ICommand DeleteTemplateCommand => deleteTemplateCommand ?? new MyActionCommand(OnDeleteTemplateCommandCommandExecuted);
        private void OnDeleteTemplateCommandCommandExecuted(object p)
        {
            ObservableCollection<SendingTemplate> templates = new(Templates.Cast<SendingTemplate>());
            MessageService.DeleteTemplates(templates);
            OnTemplatesUpdateCommandExecuted(new());
        }
        #endregion
        #region Start sending Command
        ICommand startSendingCommand;
        public ICommand StartSendingCommand => startSendingCommand ?? new MyActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
        private bool CanStartSendingCommandExecute(object p) => Templates.Cast<SendingTemplate>().Any(template => template.IsChecked);
        private void OnStartSendingCommandExecuted(object p)
        {
            MessageService.StartSending(new (Templates.Cast<SendingTemplate>().Where(template => template.IsChecked)));
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
        ICommand сloseApplicationCommand;
        public ICommand CloseApplicationCommand => сloseApplicationCommand ?? new MyActionCommand(OnCloseApplicationExecuted);
        private void OnCloseApplicationExecuted(object p)
        {
            WindowMenager.CloseWindow<MainWindow>(true);
        }
        #endregion
        #region TemplatesUpdate
        private ICommand templatesUpdateCommand;
        public ICommand TemplatesUpdateCommand => templatesUpdateCommand ??= new MyActionCommand(OnTemplatesUpdateCommandExecuted);

        private void OnTemplatesUpdateCommandExecuted(object obj)
        {
            Templates = CollectionViewSource.GetDefaultView(MessageService.loadMessageTemplates() ?? new ObservableCollection<SendingTemplate>());
            MessageBox.Show("Templates updated!");
        }
        #endregion
        #region EditMessages  
        private ICommand editMessagesCommand;
        public ICommand EditMessagesCommand => editMessagesCommand ??= new MyActionCommand(OnEditMessages, CanOpenMessages);
        private bool CanOpenMessages(object p)
        {
            var openedWindow = Application.Current.Windows.OfType<MessagesWindow>().FirstOrDefault(w => w.IsVisible);
            return openedWindow == null ? true : false;
        }
        private void OnEditMessages(object p)
        {
            MessagesWindow window = new(new(new(new())));
            WindowMenager.OpenWindowCentered<MainWindow>(window);
        }
        #endregion
        #region Open Accaunts  
        private ICommand openAccauntsCommand;
        public ICommand OpenAccauntsCommand => openAccauntsCommand ??= new MyActionCommand(OnOpenAccauntsCommand, CanOpenAccaunts);
        private bool CanOpenAccaunts(object p)
        {
            var openedWindow = Application.Current.Windows.OfType<AccountsSettingsWindow>().FirstOrDefault(w => w.IsVisible);
            return openedWindow == null ? true : false;
        }
        private void OnOpenAccauntsCommand(object p)
        {
            var openedWindow = Application.Current.Windows.OfType<AccountsSettingsWindow>().FirstOrDefault(w => w.IsVisible);
            if (openedWindow == null)
            {
                AccountsSettingsWindow window = new();
                WindowMenager.OpenWindowCentered<MainWindow>(window);
            }
        }
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            CheckDeliveryCommand = new MyActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
            OpenCustomersCommand = new MyActionCommand(OnOpenCustomersCommandExecuted, CanOpenCustomersCommandExecute);
            Templates = CollectionViewSource.GetDefaultView(MessageService.loadMessageTemplates() ?? new ObservableCollection<SendingTemplate>());
        }
    }
}