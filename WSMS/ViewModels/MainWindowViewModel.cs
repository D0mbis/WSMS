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

namespace WSMS.ViewModels
{
    public class MainWindowViewModel : Model
    {
        #region Properties
        private string title = "WSMS";
        public string Title { get => title; set => Set(ref title, value); }

        private ICollectionView templates;
        public ICollectionView Templates { get => templates; set => Set(ref templates, value); }

        private SendingTemplate selectedTemplate;

        public SendingTemplate SelectedTemplate
        {
            get => selectedTemplate;
            set  { Set(ref selectedTemplate, value); MessageBox.Show($"{selectedTemplate} is selected now");  }
        }


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
            CustomersWindow window = new();
            WindowMenager.OpenWindowCentered<MainWindow>(window);
        }
        #endregion
        #region LoadCustomersCommand
        public ICommand LoadCustomersCommand { get; }
        #endregion
        #region CreateSendingCommand
        ICommand createSendingCommand;
        public ICommand CreateSendingCommand => createSendingCommand ?? new MyActionCommand(OnCreateSendingCommandCommandExecuted);
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
            ObservableCollection<SendingTemplate> templates = new (Templates.Cast<SendingTemplate>());
            MessageService.DeleteTemplates(templates);
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
        public ICommand EditMessagesCommand => editMessagesCommand ??= new MyActionCommand(OnEditMessages);
        private void OnEditMessages(object p)
        {
            MessagesWindow window = new(new(new(new())));
            WindowMenager.OpenWindowCentered<MainWindow>(window);
        }
        #endregion
        #region Open Accaunts  
        private ICommand openAccauntsCommand;
        public ICommand OpenAccauntsCommand => openAccauntsCommand ??= new MyActionCommand(OnOpenAccauntsCommand);
        private void OnOpenAccauntsCommand(object p)
        {
            AccountsSettingsWindow window = new();
            WindowMenager.OpenWindowCentered<MainWindow>(window);
        }
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            StartSendingCommand = new MyActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
            CheckDeliveryCommand = new MyActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
            OpenContactsCommand = new MyActionCommand(OnOpenContactsCommandExecuted, CanOpenContactsCommandExecute);
            Templates = CollectionViewSource.GetDefaultView(MessageService.loadMessageTemplates() ?? new ObservableCollection<SendingTemplate>());
        }
    }
}