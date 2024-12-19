using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace WSMS.Models.Base
{
    public class CheckableItemWithChildren<TChild> : Model, ICheckable
     where TChild : Model
    {
        private bool isChecked;
        [JsonIgnore]
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (Set(ref isChecked, value))
                {
                    // Устанавливаем состояние для всех дочерних элементов
                    if (Children != null)
                    {
                        foreach (var child in Children)
                        {
                            if (child is CheckableItemWithChildren<TChild> childWithChildren)
                                childWithChildren.IsChecked = value;
                            else if (child is ICheckable checkableChild)
                                checkableChild.IsChecked = value;
                        }
                    }
                }
            }
        }

        private ObservableCollection<TChild> children;
        [JsonIgnore]
        public ObservableCollection<TChild> Children
        {
            get => children;
            set
            {
                if (children != value)
                {
                    if (children != null)
                        children.CollectionChanged -= OnChildrenChanged;

                    Set(ref children, value);

                    if (children != null)
                    {
                        children.CollectionChanged += OnChildrenChanged;

                        foreach (var child in children)
                            child.PropertyChanged += Child_PropertyChanged;
                    }
                }
            }
        }

        public CheckableItemWithChildren()
        {
            Children = new ObservableCollection<TChild>();
        }

        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TChild child in e.NewItems)
                    child.PropertyChanged += Child_PropertyChanged;
            }

            if (e.OldItems != null)
            {
                foreach (TChild child in e.OldItems)
                    child.PropertyChanged -= Child_PropertyChanged;
            }

            UpdateIsChecked();
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChecked))
            {
                UpdateIsChecked();
            }
        }

        public void UpdateIsChecked()
        {
            if (Children.All(c => (c as ICheckable)?.IsChecked == true))
            {
                isChecked = true;
            }
            else if (Children.All(c => (c as ICheckable)?.IsChecked == false))
            {
                isChecked = false;
            }
            else
            {
                isChecked = true; // или false для частично выбранного состояния
            }

            OnPropertyChanged(nameof(IsChecked));
        }
    }
}
