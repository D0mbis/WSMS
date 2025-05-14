using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Infrastructure.Other;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Services;
using WSMS.Views;
using WSMS.Views.Windows;

namespace WSMS.ViewModels
{
    public class CreateSendingViewModel : CheckableItemWithChildren<SubDirection>
    {
        #region Properties of Customers
        private ObservableCollection<SubDirection> allSubDirections;
        public ObservableCollection<SubDirection>? AllSubDirections
        {
            get => allSubDirections;
            set
            {
                if (allSubDirections != null)
                {
                    foreach (var subDirection in allSubDirections)
                    {
                        subDirection.PropertyChanged -= Update;
                    }
                }
                Set(ref allSubDirections, value);

                if (allSubDirections != null)
                {
                    foreach (var subDirection in allSubDirections)
                    {
                        subDirection.PropertyChanged += Update;
                    }
                }
            }
        }
        private ICollectionView? subDirections;
        public ICollectionView? SubDirections
        {
            get => subDirections;
            set => Set(ref subDirections, value);
        }
        private string? filterDays;
        public string? FilterDays
        {
            get => filterDays;
            set
            {
                Set(ref filterDays, value);
                ApplyDateFilter();
            }
        }

        private ObservableCollection<SubDirection> selecledSubDirections;
        public ObservableCollection<SubDirection> SelectedSubDirections
        {
            get => selecledSubDirections;
            set => Set(ref selecledSubDirections, value);
        }
        private int selectedContactsCount;
        public int SelectedContactsCount
        {
            get => selectedContactsCount;
            set
            {
                Set(ref selectedContactsCount, value);
            }
        }
        #endregion

        #region Properties of Messages
        private ICollectionView messagesView;
        private MessageWrapper? selectedMessage;
        public ICollectionView MessagesView
        {
            get => messagesView;
            set => Set(ref messagesView, value);
        }
        public MessageWrapper SelectedMessage
        {
            get => selectedMessage ?? new(new());
            set => Set(ref selectedMessage, value);
        }



        #endregion

        #region Properties of Accounts
        private ObservableCollection<WhatsAppAccount>? whatsAppAccounts;
        private WhatsAppAccount? selectedWhatsAppAccount;

        public ObservableCollection<WhatsAppAccount> WhatsAppAccounts
        {
            get => whatsAppAccounts ?? new(new());
            set => Set(ref whatsAppAccounts, value);
        }
        public WhatsAppAccount SelectedWhatsAppAccount
        {
            get => selectedWhatsAppAccount;
            set => Set(ref selectedWhatsAppAccount, value);
        }


        #endregion

        #region Commands
        #region EditSeletedCustomers
        private ICommand editSeletedCustomers;
        public ICommand EditSeletedCustomers => editSeletedCustomers ??= new MyActionCommand(OnEditSeletedCustomersCommandExecuted);

        private void OnEditSeletedCustomersCommandExecuted(object p)
        {
            if (p is SubDirection selectedSubDirection)
            {
                SelectedDirectionCustomersWindow window = new(new(selectedSubDirection));
                window.ShowDialog();
            }
        }
        #endregion
        #region EditMessages
        private ICommand editMessagesCommand;
        public ICommand EditMessagesCommand => editMessagesCommand ??= new MyActionCommand(OnEditMessages);
        private void OnEditMessages(object p)
        {
            MessagesWindow window = new(new(SelectedMessage));
            bool? result = window.ShowDialog();
            if (result == false)
            {
                MessagesView = CollectionViewSource.GetDefaultView(MessageService.LoadMessages());
            }
        }
        #endregion
        #region EditAccaunts
        private ICommand editAccauntsCommand;
        public ICommand EditAccauntsCommand => editAccauntsCommand ??= new MyActionCommand(OnEditAccauntsCommand);
        private void OnEditAccauntsCommand(object p)
        {
            AccountsSettingsWindow window = new();
            bool? result = window.ShowDialog();
            if (result == false)
            {
                //MessagesView = CollectionViewSource.GetDefaultView(MessageService.LoadMessages());
            }
        }
        #endregion
        #region SaveTemplate
        private ICommand addTemplateCommand;
        private bool CanAddTemplateCommandCommandExecute(object p)
        {
            return SelectedSubDirections.Count != 0 &&
            SelectedWhatsAppAccount != null &&
            SelectedMessage.Message.Name != null;
        }
        public ICommand AddTemplateCommand => addTemplateCommand ??= new MyActionCommand(OnAddTemplateCommand, CanAddTemplateCommandCommandExecute);
        private void OnAddTemplateCommand(object p)
        {
            MessageService.AddTemplate(new SendingTemplate()
            {
                Account = SelectedWhatsAppAccount.Name,
                Message = SelectedMessage.Message,
                SelectedSubdirections = CustomersService.GetLiteSubDirections(SelectedSubDirections)
            });
            
        }
        #endregion
        #region CloseAppCommand
        ICommand closeWindowCommand;
        public ICommand CloseWindowCommand => closeWindowCommand ?? new MyActionCommand(OnCloseWindowCommandExecuted);
        private void OnCloseWindowCommandExecuted(object p)
        {
            WindowMenager.CloseWindow<CreateSendingWindow>();
        }
        #endregion
        #endregion

        public CreateSendingViewModel()
        {
            AllSubDirections = CustomersRepository.Instance.GetSubDirectionsFull();
            SubDirections = CollectionViewSource.GetDefaultView(AllSubDirections);
            SelectedSubDirections = new(AllSubDirections.Where(sd => sd.IsChecked));
            SelectedContactsCount = CustomersRepository.Instance.GetCheckedCustomersCount();
            MessagesView = CollectionViewSource.GetDefaultView(MessageService.LoadMessages());
            WhatsAppAccounts = WhatsAppAccountsService.GetAccounts(true);
        }

        private void Update(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubDirection.IsChecked))
            {
                SelectedSubDirections = new(AllSubDirections.Where(sd => sd.IsChecked));
                SelectedContactsCount = CustomersRepository.Instance.GetCheckedCustomersCount();
            }

        }
        private void ApplyDateFilter()
        {
            if (CollectionViewSource.GetDefaultView(SubDirections) is ICollectionView collectionView)
            {
                if (int.TryParse(FilterDays, out int days))
                {
                    collectionView.Filter = item =>
                    {
                        if (item is SubDirection subDirection && subDirection.LastSending.HasValue)
                        {
                            var dateDifference = (DateTime.Now - subDirection.LastSending.Value).Days;
                            return dateDifference >= days;
                        }
                        return false;
                    };
                }
                else
                {
                    collectionView.Filter = null;
                }
                collectionView.Refresh();
            }
        }

    }
}
