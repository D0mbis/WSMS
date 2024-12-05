using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Services;
using WSMS.Views.Windows;

namespace WSMS.ViewModels
{
    public class CreateSendingViewModel : CheckableItemWithChildren<SubDirection>
    {
        #region Properties
        private ObservableCollection<SubDirection> allSubDirections;
        public ObservableCollection<SubDirection> AllSubDirections
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

        #region Commands
        public ICommand EditSeletedCustomers { get; }
        private bool CanEditSeletedCustomersCommandExecute(object p) => true;

        private void OnEditSeletedCustomersCommandExecuted(object p)
        {
            if (p is SubDirection selectedSubDirection)
            {
                SelectedDirectionCustomersWindow window = new(new(selectedSubDirection));
                window.ShowDialog();
            }
        }
        #endregion

        public CreateSendingViewModel()
        {
            AllSubDirections = CustomersRepository.Instance.GetSubDirections();
            SubDirections = CollectionViewSource.GetDefaultView(AllSubDirections);
            SelectedSubDirections = new(AllSubDirections.Where(sd => sd.IsChecked));
            EditSeletedCustomers = new MyActionCommand(OnEditSeletedCustomersCommandExecuted, CanEditSeletedCustomersCommandExecute);
            SelectedContactsCount = CustomersRepository.Instance.GetCheckedCustomersCount();
        }

        private void Update(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubDirection.IsChecked))
            {
                // Обновляем выбранные SubDirections
                SelectedSubDirections = new(AllSubDirections.Where(sd => sd.IsChecked));

                // Обновляем количество выбранных клиентов
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
                    collectionView.Filter = null; // Сброс фильтра
                }
                collectionView.Refresh();
            }
        }
    }
}
