using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Infrastructure.Other;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Views.Windows;

namespace WSMS.ViewModels
{
    public class SelectedDirectionCustomersViewModel : Model
    {

        // работает с Message, осталось разобра
        private ICollectionView? customersView;
        public ICollectionView? CustomersView { get => customersView; set => Set(ref customersView, value); }
        #region CloseAppCommand
        ICommand closeWindowCommand;
        public ICommand CloseWindowCommand => closeWindowCommand ?? new MyActionCommand(OnCloseWindowCommandExecuted);
        private void OnCloseWindowCommandExecuted(object p)
        {
            WindowMenager.CloseWindow<SelectedDirectionCustomersWindow>();
        }
        #endregion

        public SelectedDirectionCustomersViewModel(SubDirection subDirection)
        {
            CustomersView = CollectionViewSource.GetDefaultView(subDirection.Customers);
        }
    }
}
