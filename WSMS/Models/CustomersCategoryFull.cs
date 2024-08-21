using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class CustomersCategoryFull : Model
    {
        [JsonProperty(Order = 1)]
        public string MainCategory { get; set; }
        private ObservableCollection<SubCategory> subCategories;
        private bool isChecked;
        [JsonProperty(Order = 2)]
        public ObservableCollection<SubCategory> SubCategories
        {
            get => subCategories;
            set
            {
                if (subCategories != value)
                {
                    if (subCategories != null)
                        subCategories.CollectionChanged -= OnSubCategoriesChanged;

                    Set(ref subCategories, value);

                    if (subCategories != null)
                        subCategories.CollectionChanged += OnSubCategoriesChanged;

                    foreach (var subCategory in SubCategories)
                    {
                        subCategory.PropertyChanged += SubCategory_PropertyChanged;
                    }
                }
            }
        }
        [JsonIgnore]
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    Set(ref isChecked, value);
                    foreach (var item in subCategories)
                    {
                        item.IsChecked = value;
                    }
                }
            }
        }
        public CustomersCategoryFull(string mainCategory, ObservableCollection<SubCategory> subCategories)
        {
            MainCategory = mainCategory;
            SubCategories = subCategories;
            IsChecked = true;
        }

        private void OnSubCategoriesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubCategory subCategory in e.NewItems)
                {
                    subCategory.PropertyChanged += SubCategory_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (SubCategory subCategory in e.OldItems)
                {
                    subCategory.PropertyChanged -= SubCategory_PropertyChanged;
                }
            }

            UpdateIsChecked();
        }

        private void SubCategory_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubCategory.IsChecked))
            {
                UpdateIsChecked();
            }
        }

        private void UpdateIsChecked()
        {
            if (SubCategories.All(sc => sc.IsChecked))
            {
                isChecked = true;
            }
            else if (SubCategories.All(sc => !sc.IsChecked))
            {
                isChecked = false;
            }
            else
            {
                isChecked = true;
            }

            OnPropertyChanged(nameof(IsChecked));
        }
    }
}