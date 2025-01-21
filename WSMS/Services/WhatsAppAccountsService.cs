using System;
using System.Collections.ObjectModel;
using System.IO;
using WSMS.Models;

namespace WSMS.Services
{
    public class WhatsAppAccountsService
    {
        public static ObservableCollection<WhatsAppAccount>? GetAccounts()
        {
            string profilesDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Users");
            string[] subdirectoryEntries = Directory.GetDirectories(profilesDirectory);
            ObservableCollection<WhatsAppAccount> accounts = new();
            // Получаем только имена подпапок
            foreach (string subdirectory in subdirectoryEntries)
            {
                accounts.Add(new WhatsAppAccount() { Id = Path.GetFileName(subdirectory), Name = Path.GetFileName(subdirectory) });
            }
            accounts.Add(new WhatsAppAccount() { Id ="EditAccaunts", Name = "Редактировать.." });
            return accounts;
        }

        public static void UpdateAccounts(ObservableCollection<WhatsAppAccount> accounts, bool delete = false)
        {
            if (!delete)
            {
                string accountId = Guid.NewGuid().ToString();
                //string accountName = $"Account {Accounts.Count + 1}";
                // Accounts.Add(new WhatsAppAccount { AccountId = accountId, AccountName = accountName });
                // StatusTextBlock.Text = $"Added {accountName}";
            }
            else {
                /*

                     var selectedAccount = AccountListBox.SelectedItem as WhatsAppAccount;
                if (selectedAccount != null)
                {
                    Accounts.Remove(selectedAccount);
                    StatusTextBlock.Text = $"Removed {selectedAccount.AccountName}";
                }
                else
                {
                    MessageBox.Show("Please select an account to remove.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                    */
            }
        }
    }
}
