using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using WSMS.Services;

namespace WSMS.Views
{
    /// <summary>
    /// Interaction logic for AccountsSettings.xaml
    /// </summary>
    public partial class AccountsSettings : Window
    {
        public class WhatsAppAccount
        {
            public string AccountId { get; set; }
            public string AccountName { get; set; }
        }
        public class CustomersCategories
        {
            public string name { get; set; }
            public string lastdate { get; set; }
            public bool IsChacked { get; set; }
        }

        // Коллекция CustomersCategories
        public ObservableCollection<CustomersCategories> Customers { get; set; }
        private ICollectionView CustomersView { get; set; }
        public ObservableCollection<WhatsAppAccount> Accounts { get; set; }
        public AccountsSettings()
        {
            InitializeComponent();
            Accounts = GetAccounts() ?? new ObservableCollection<WhatsAppAccount>();
            AccountListBox.ItemsSource = Accounts;
            Customers = new ObservableCollection<CustomersCategories>
            {
                new CustomersCategories { name = "Customer 1", lastdate = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"), IsChacked = false },
                new CustomersCategories { name = "Customer 2", lastdate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"), IsChacked = true },
                new CustomersCategories { name = "Customer 3", lastdate = DateTime.Now.AddDays(-25).ToString("yyyy-MM-dd"), IsChacked = false }
            };

            // Создание представления для коллекции
            CustomersView = CollectionViewSource.GetDefaultView(Customers);
            CustomersView.Filter = FilterCustomers;

            // Привязка представления к ListView
            CustomersListView.ItemsSource = CustomersView;
        }

        private ObservableCollection<WhatsAppAccount>? GetAccounts()
        {
            string profilesDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Users");
            string[] subdirectoryEntries = Directory.GetDirectories(profilesDirectory);
            ObservableCollection<WhatsAppAccount> accounts = new();
            // Получаем только имена подпапок
            foreach (string subdirectory in subdirectoryEntries)
            {
                accounts.Add(new WhatsAppAccount()
                {
                    AccountId = System.IO.Path.GetFileName(subdirectory),
                    AccountName = System.IO.Path.GetFileName(subdirectory)
                });
            }
            return accounts;
        }

        private void StartSessionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountListBox.SelectedItem as WhatsAppAccount;
            if (selectedAccount != null)
            {
                StartSession(selectedAccount);
                StatusTextBlock.Text = $"Started session for {selectedAccount.AccountName}";
            }
            else
            {
                MessageBox.Show("Please select an account to start the session.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Кнопка "Stop Session"
        private void StopSessionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountListBox.SelectedItem as WhatsAppAccount;
            if (selectedAccount != null)
            {
                StopSession(selectedAccount);
                StatusTextBlock.Text = $"Stopped session for {selectedAccount.AccountName}";
            }
            else
            {
                MessageBox.Show("Please select an account to stop the session.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Кнопка "Add Account"
        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string accountId = Guid.NewGuid().ToString();
            string accountName = $"Account {Accounts.Count + 1}";
            Accounts.Add(new WhatsAppAccount { AccountId = accountId, AccountName = accountName });
            StatusTextBlock.Text = $"Added {accountName}";
        }

        // Кнопка "Remove Account"
        private void RemoveAccountButton_Click(object sender, RoutedEventArgs e)
        {
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
        }

        // Метод для запуска сессии
        private void StartSession(WhatsAppAccount account)
        {
            WebService.OpenBrowser( account.AccountId);
        }

        // Метод для остановки сессии
        private void StopSession(WhatsAppAccount account)
        {
            // Здесь будет логика остановки сессии
        }

        private bool FilterCustomers(object item)
        {
            if (item is CustomersCategories customer)
            {
                if (int.TryParse(DaysTextBox.Text, out int days))
                {
                    if (DateTime.TryParseExact(customer.lastdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastDate))
                    {
                        return (DateTime.Now - lastDate).TotalDays > days;
                    }
                }
            }
            return true; // По умолчанию, если не введено число, отображать все элементы
        }

        // Кнопка "Apply Filter" для обновления фильтра
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            CustomersView.Refresh();
        }

        // Обработчик для числового ввода в TextBox
        private void DaysTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, e.Text.Length - 1);
        }
    }
}

