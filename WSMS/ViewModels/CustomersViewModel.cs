using System;
using WSMS.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using WSMS.Models;
using WSMS.Services;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using System.Collections.Generic;
namespace WSMS.ViewModels
{
    //  TODO: add save updated values to excel
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
                    Set(ref selectedCustomer, value);
                    CustomersView.Refresh();
                }
            }
        }

        #region Commands
        #region PullCustomersFromRemote
        public ICommand PullCustomersFromRemote { get; }

        private bool CanPullCustomersFromRemoteExecute(object p) => true;

        private void OnPullCustomersFromRemoteExecuted(object p)
        {
            GoogleSheetsAPI.PulldbCustomers();
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

        private bool CanPushValuesToRemoteExcelExecute(object p) => true;

        private void OnPushValuesToRemoteExcelExecuted(object p)
        {
            GoogleSheetsAPI.PushValues(SelectedCustomer.ID, SelectedCustomer );
        }
        #endregion
        #endregion
        public CustomersViewModel()
        {
            CustomersView = CollectionViewSource.GetDefaultView(CustomersService.GetCustomersWithoutGroups());
            PullCustomersFromRemote = new ActionCommand(OnPullCustomersFromRemoteExecuted, CanPullCustomersFromRemoteExecute);
            AddNewCredentials = new ActionCommand(OnAddNewCredentialsExecuted, CanAddNewCredentialsExecute);
            PushValuesToRemoteExcel = new ActionCommand(OnPushValuesToRemoteExcelExecuted, CanPushValuesToRemoteExcelExecute);
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