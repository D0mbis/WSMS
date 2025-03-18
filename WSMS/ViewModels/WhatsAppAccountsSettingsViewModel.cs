using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Services;
using WSMS.Views.Windows;

namespace WSMS.ViewModels
{

    public class WhatsAppAccountsSettingsViewModel : Model
    {
        private string statusTextBlock;
        private ObservableCollection<WhatsAppAccount> accounts;
        private WhatsAppAccount selectedAccount;
        private string userInput = string.Empty;

        public string StatusTextBlock
        {
            get => statusTextBlock;
            set => Set(ref statusTextBlock, value);
        }
        public ObservableCollection<WhatsAppAccount> Accounts
        {
            get => accounts; set => Set(ref accounts, value);
        }
        public WhatsAppAccount SelectedAccount
        {
            get => selectedAccount;
            set => Set(ref selectedAccount, value);
        }
        public string UserInput { get => userInput; set => Set(ref userInput, value); }

        #region Commands
        #region AddNewAccountCommand
        private ICommand addNewAccountCommand;
        public ICommand AddNewAccountCommand => addNewAccountCommand ??= new MyActionCommand(OnAddNewAccountCommandExecuted, CanAddNewAccountCommandExecuted);
        private bool CanAddNewAccountCommandExecuted(object p) => UserInput != string.Empty;
        private void OnAddNewAccountCommandExecuted(object p)
        {
            WhatsAppAccountsService.UpdateAccounts(Accounts, SelectedAccount, UserInput);
            StatusTextBlock = $"Added {UserInput}";
            UserInput = string.Empty;
        }
        #endregion
        #region DeleteAccountCommand
        private ICommand deleteAccountCommand;
        public ICommand DeleteAccountCommand => deleteAccountCommand ??= new MyActionCommand(OnDeleteAccountCommandCommandExecuted);

        private void OnDeleteAccountCommandCommandExecuted(object p)
        {
            WhatsAppAccountsService.UpdateAccounts(Accounts, SelectedAccount);
        }
        #endregion
        #region StartSessionCommand
        private ICommand startSessionCommand;
        public ICommand StartSessionCommand => startSessionCommand ??= new MyActionCommand(OnStartSessionCommandCommandExecuted);
        private void OnStartSessionCommandCommandExecuted(object p)
        {
            if (SelectedAccount != null)
            {
                WebService.OpenBrowser(SelectedAccount.Name);
                StatusTextBlock = $"Started session for {selectedAccount.Name}";
            }
            else
            {
                MessageBox.Show("Please select an account to start the session.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion
        #region StopSessionCommand
        private ICommand stopSessionCommand;
        public ICommand StopSessionCommand => stopSessionCommand ??= new MyActionCommand(OnStopSessionCommandCommandExecuted);

        private void OnStopSessionCommandCommandExecuted(object p)
        {
            if (SelectedAccount != null)
            {
                WebService.CloseBrowser(SelectedAccount.Name);
                StatusTextBlock = $"Stopped session for {selectedAccount.Name}";
            }
            else
            {
                MessageBox.Show("Please select an account to stop the session.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion
        #endregion
        public WhatsAppAccountsSettingsViewModel()
        {
            StatusTextBlock = "Ready.";
            Accounts = WhatsAppAccountsService.GetAccounts() ?? new ObservableCollection<WhatsAppAccount>();
        }
    }
}
