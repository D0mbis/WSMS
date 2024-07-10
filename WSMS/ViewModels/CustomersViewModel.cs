using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Services;
using WSMS.ViewModels.Base;
namespace WSMS.ViewModels
{
    internal class CustomersViewModel : ViewModel
    {
        private ObservableCollection<Customer> customers;
        private string searchText;
        private Customer selectedCustomer;
        public ICollectionView CustomersView { get; set; }
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

        #region Commands
        #region Import to .csv
        public ICommand ImportToCsv { get; }

        private bool CanImportToCsvExecute(object p) => true;

        private void OnImportToCsvExecuted(object p) => CustomersService.ImportToCSV();
        #endregion
        #region PullCustomersFromRemote
        public ICommand PullCustomersFromRemote { get; }

        private bool CanPullCustomersFromRemoteExecute(object p) => true;

        private void OnPullCustomersFromRemoteExecuted(object p)
        {
            GoogleSheetsAPI.PulldbCustomers();
            CustomersView.Refresh();
        }
        #endregion
        #region AddNewCredentials Command
        public ICommand AddNewCredentials { get; }

        private bool CanAddNewCredentialsExecute(object p) => true;

        private void OnAddNewCredentialsExecuted(object p)
        {
            GoogleSheetsAPI.AddNewCredentials();
        }
        #endregion
        #region PushValuesToRemoteExcel Command
        public ICommand PushValuesToRemoteExcel { get; }

        private bool CanPushValuesToRemoteExcelExecute(object p)
        {
            if (selectedCustomer != default)
            {
                if (selectedCustomer.IsSelected) return true;
            }
            return false;
        }

        private void OnPushValuesToRemoteExcelExecuted(object p)
        {
            GoogleSheetsAPI.PushValues(SelectedCustomer.ID, SelectedCustomer);
        }
        #endregion
        #endregion
        public CustomersViewModel()
        {
            CustomersView = CollectionViewSource.GetDefaultView(CustomersService.GetCustomersWithoutGroups());
            PullCustomersFromRemote = new ActionCommand(OnPullCustomersFromRemoteExecuted, CanPullCustomersFromRemoteExecute);
            AddNewCredentials = new ActionCommand(OnAddNewCredentialsExecuted, CanAddNewCredentialsExecute);
            PushValuesToRemoteExcel = new ActionCommand(OnPushValuesToRemoteExcelExecuted, CanPushValuesToRemoteExcelExecute);
            ImportToCsv = new ActionCommand(OnImportToCsvExecuted, CanImportToCsvExecute);
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
                               customer.MainCategory.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.SubCategory.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               customer.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
                CustomersView.Refresh();
            }
        }
    }
}