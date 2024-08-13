using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WSMS.Models;
using WSMS.Services;
using WSMS.Views.Windows;
using System;
using WSMS.Models.Base;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors.Core;
using WSMS.Infrastructure.Commands.Base;

namespace WSMS.ViewModels
{
    internal class MainWindowViewModel : Model
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
        private ObservableCollection<CustomersCategoryFull> customersCategories;
        public ObservableCollection<CustomersCategoryFull> CustomersCategories
        {
            get => customersCategories;
            set
            {
                if (customersCategories != value)
                {

                    Set(ref customersCategories, value); // (OnPropertyChanged)
                }
            }
        }
        private ObservableCollection<MessageWrapper> messages;
        private MessageWrapper selectedMessage;
        public MessageWrapper SelectedMessage
        {
            get => selectedMessage;
            set
            {
                Set(ref selectedMessage, value);
            }
        }
        private string selectAllButtonContent = "Select all";
        public string SelectAllButtonContent { get => selectAllButtonContent; set => Set(ref selectAllButtonContent, value); }

        ICollectionView messagesView;
        public ICollectionView MessagesView { get => messagesView; set { Set(ref messagesView, value); } }

        public ObservableCollection<MessageWrapper> Messages
        {
            get => messages;
            set
            {
                Set(ref messages, value);
            }
        }
        /*public string MessageText
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
        }*/

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
            if (CustomersCategories == null) { return true; }
            return false;
        }
        private void OnStartLoadCustomersCommandExecuted(object p)
        {
            /// Load customers categories in SaveMessageWindow
           // CustomerGroups = CustomersService.LoadAllCategories();
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
            if (SelectedMessage.IsChanged) return true;
            return false;
        }
        private void OnSaveMessageCommandExecuted(object p)
        {
            if (!SaveMessageWindow.IsOpen)
            {
                SaveMessageWindow saveMessageWindow = new();
                saveMessageWindow.Show();
            }

            /*if (MessageService.SaveMesage(Message))
                message.IsChanged = false;*/
        }
        #endregion
        #region Select all Command

        public ICommand SelectAllCommand { get; }

        private bool CanSelectAllCommandExecute(object p) => true;
        private void OnSelectAllCommandExecuted(object p)
        {
            bool flag;
            if (SelectAllButtonContent == "Select all")
            {
                flag = true;
                SelectAllButtonContent = "Unselect all";
            }
            else
            {
                SelectAllButtonContent = "Select all";
                flag = false;
            }
            CustomersCategories = MessageService.ChangeIsCheck(CustomersCategories, flag);
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
                        SelectedMessage.Message.Image = MessageService.GetImage(filePath);
                        SelectedMessage.Message.ImagePath = filePath;
                        //CommandManager.InvalidateRequerySuggested();
                        (SaveMessageCommand as MyActionCommand).RaiseCanExecuteChanged();
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
            SelectedMessage = new MessageWrapper(new Message());
            CustomersCategories = CustomersService.LoadAllCategories();
            Messages = new ObservableCollection<MessageWrapper>
            {
                new MessageWrapper(new Message()),
                new MessageWrapper(new Message())
            };
            MessagesView = CollectionViewSource.GetDefaultView(Messages); // paste GetMessages()
                                                                          // messages.OldTextMessage = messageText = messages.Text;
            StartBrowserCommand = new MyActionCommand(OnStartBrowserCommandExecuted, CanStartBrowserCommandExecute);
            StartSendingCommand = new MyActionCommand(OnStartSendingCommandExecuted, CanStartSendingCommandExecute);
            CheckDeliveryCommand = new MyActionCommand(OnStartCheckDeliveryCommandExecuted, CanStartCheckDeliveryCommandExecute);
            LoadCustomersCommand = new MyActionCommand(OnStartLoadCustomersCommandExecuted, CanLoadCustomersCommandExecute);
            OpenContactsCommand = new MyActionCommand(OnOpenContactsCommandExecuted, CanOpenContactsCommandExecute);
            SaveMessageCommand = new MyActionCommand(OnSaveMessageCommandExecuted, CanSaveMessageCommandExecute);
            ImageDropCommand = new MyActionCommand(OnImageDropCommandExecuted);
            SelectAllCommand = new MyActionCommand(OnSelectAllCommandExecuted, CanSelectAllCommandExecute);
        }


    }
}