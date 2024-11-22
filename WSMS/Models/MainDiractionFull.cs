using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class MainDiractionFull : Model
    {
        [JsonProperty(Order = 1)]
        public string MainDiraction { get; set; }
        private ObservableCollection<SubDiraction> subDiractions;
        [JsonProperty(Order = 2)]
        public ObservableCollection<SubDiraction> SubDiractions
        {
            get => subDiractions;
            set
            {
                if (subDiractions != value)
                {
                    if (subDiractions != null)
                        subDiractions.CollectionChanged -= OnSubDiractionsChanged;

                    Set(ref subDiractions, value);

                    if (subDiractions != null) // need check why need to chek for null now?
                        subDiractions.CollectionChanged += OnSubDiractionsChanged;

                    foreach (var subDiraction in SubDiractions)
                    {
                        subDiraction.PropertyChanged += SubDiraction_PropertyChanged;
                    }
                }
            }
        }
        private bool isChecked;
        [JsonIgnore]
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    Set(ref isChecked, value);
                    if(subDiractions != null)
                    foreach (var item in subDiractions)
                    {
                        item.IsChecked = value;
                    }
                }
            }
        }
        public MainDiractionFull(string mainDiraction, ObservableCollection<SubDiraction> subDiractions)
        {
            MainDiraction = mainDiraction;
            SubDiractions = subDiractions;
            IsChecked = true;
        }

        private void OnSubDiractionsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubDiraction subDiraction in e.NewItems)
                {
                    subDiraction.PropertyChanged += SubDiraction_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (SubDiraction subCategory in e.OldItems)
                {
                    subCategory.PropertyChanged -= SubDiraction_PropertyChanged;
                }
            }

            UpdateIsChecked();
        }

        private void SubDiraction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubDiraction.IsChecked))
            {
                UpdateIsChecked();
            }
        }

        private void UpdateIsChecked()
        {
            if (SubDiractions.All(sd => sd.IsChecked))
            {
                isChecked = true;
            }
            else if (SubDiractions.All(sd => !sd.IsChecked))
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