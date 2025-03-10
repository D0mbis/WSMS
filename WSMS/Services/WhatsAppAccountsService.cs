using System;
using System.Collections.ObjectModel;
using System.IO;
using WSMS.Models;

namespace WSMS.Services
{
    public class WhatsAppAccountsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="available">Available to add last account from button EditAccaunts</param>
        /// <returns></returns>
        public static ObservableCollection<WhatsAppAccount>? GetAccounts(bool available = false)
        {
            string profilesDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Users");
            string[] subdirectoryEntries = Directory.GetDirectories(profilesDirectory);
            ObservableCollection<WhatsAppAccount> accounts = new();
            // Получаем только имена подпапок
            foreach (string subdirectory in subdirectoryEntries)
            {
                accounts.Add(new WhatsAppAccount() { Name = Path.GetFileName(subdirectory) });
            }
            if (available)
                accounts.Add(new WhatsAppAccount() { Name = "Редактировать..  🖉" });
            return accounts;
        }

        public static void UpdateAccounts(ObservableCollection<WhatsAppAccount> accounts, WhatsAppAccount whatsAppAccount, string userInput = default)
        {
            if (userInput != default)
            {
                accounts.Add(new WhatsAppAccount { Name = userInput });
                string profileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users", userInput);
                if (!Directory.Exists(profileDirectory)) { Directory.CreateDirectory(profileDirectory); }
            }
            else
            {
                accounts.Remove(whatsAppAccount);
                string profileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users", whatsAppAccount.Name);
                if (Directory.Exists(profileDirectory)) { Directory.Delete(profileDirectory, true); }
            }
        }
    }
}
