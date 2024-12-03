using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Services;
namespace WSMS.ViewModels
{
    public class CustomersViewModel : Model
    {
        private readonly CustomersRepository Repository = CustomersRepository.Instance;
        private ObservableCollection<Customer>? customers;
        private string? searchText;
        private Customer? selectedCustomer;
        private ICollectionView? customersView;
        public ObservableCollection<Customer> Customers
        {
            get => customers;
            set
            {
                Set(ref customers, value);
            }
        }
        public string SearchText
        {
            get => searchText;
            set
            {
                Set(ref searchText, value);
                ApplyFilter();
            }
        }
        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                if (selectedCustomer != value)
                {
                    if (selectedCustomer != null)
                    {
                        selectedCustomer.IsSelected = false;
                    }
                    selectedCustomer = value;
                    if (selectedCustomer != null)
                    {
                        selectedCustomer.IsSelected = true;
                    }
                    CustomersView.Refresh();
                }
            }
        }
        public ICollectionView CustomersView { get => customersView; set { Set(ref customersView, value); } }

        #region Commands
        #region Import to .csv
        public ICommand ImportToCsv { get; }

        private bool CanImportToCsvExecute(object p) => true;

        private void OnImportToCsvExecuted(object p) => CustomersService.ImportToCSV();
        #endregion
        #region AddNewCredentials Command
        public ICommand AddNewCredentials { get; }

        private bool CanAddNewCredentialsExecute(object p) => true;

        private void OnAddNewCredentialsExecuted(object p)
        {
            GoogleSheetsAPI.AddNewCredentials();
        }
        #endregion
        #region PullCustomersFromRemote Command
        public ICommand PullCustomersFromRemote { get; }

        private bool CanPullCustomersFromRemoteExecute(object p) => true;

        private void OnPullCustomersFromRemoteExecuted(object p)
        {
            GoogleSheetsAPI.PulldbCustomers();
            CustomersView = CollectionViewSource.GetDefaultView(Repository.GetCustomers());
        }
        #endregion
        #region PushValuesToRemoteExcel Command
        public ICommand PushSelectedCustomerToRemoteExcel { get; }

        private bool CanPushSelectedCustomerToRemoteExcelExecute(object p)
        {
            if (selectedCustomer != default)
            {
                if (selectedCustomer.IsSelected) return true;
            }
            return false;
        }

        private void OnPushValuesToRemoteExcelExecuted(object p)
        {
            GoogleSheetsAPI.PushCustomerUpdate(SelectedCustomer.ID, SelectedCustomer);
        }
        #endregion
        #endregion
        public CustomersViewModel()
        {
            CustomersView = CollectionViewSource.GetDefaultView(Repository.GetCustomers());
            PullCustomersFromRemote = new MyActionCommand(OnPullCustomersFromRemoteExecuted, CanPullCustomersFromRemoteExecute);
            AddNewCredentials = new MyActionCommand(OnAddNewCredentialsExecuted, CanAddNewCredentialsExecute);
            PushSelectedCustomerToRemoteExcel = new MyActionCommand(OnPushValuesToRemoteExcelExecuted, CanPushSelectedCustomerToRemoteExcelExecute);
            ImportToCsv = new MyActionCommand(OnImportToCsvExecuted, CanImportToCsvExecute);
        }

        private void ApplyFilter()
        {
            if (CustomersView != null)
            {
                CustomersView.Filter = item =>
                {
                    if (item is Customer customer)
                    {
                        return string.IsNullOrEmpty(SearchText) ||
                               customer.ID.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.PhoneNumber1.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.PhoneNumber2.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.PhoneNumber3.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.MainDirection.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.SubDirection.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase); 
                    }
                    return false;
                };
                CustomersView.Refresh();
            }
        }
    }
}