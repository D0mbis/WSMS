using System.ComponentModel;
using System.Windows.Data;
using WSMS.Models;
using WSMS.Models.Base;

namespace WSMS.ViewModels
{
    public class SelectedDirectionCustomersViewModel : Model
    {
        private ICollectionView? customersView;
        public ICollectionView? CustomersView { get => customersView; set => Set(ref customersView, value); }

        public SelectedDirectionCustomersViewModel(SubDirection subDirection)
        {
            CustomersView = CollectionViewSource.GetDefaultView(subDirection.Customers);
        }
    }
}
