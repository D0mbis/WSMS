using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class MainDirection : Model
    {
        [JsonProperty(Order = 1)]
        public string Name { get; set; }
        private ObservableCollection<SubDirection>? subDirections;
        [JsonProperty(Order = 2)]
        public ObservableCollection<SubDirection>? SubDirections
        {
            get => subDirections;
            set
            {
                if (subDirections != value)
                {
                    if (subDirections != null)
                        subDirections.CollectionChanged -= OnSubDirectionsChanged;

                    Set(ref subDirections, value);

                    if (subDirections != null) // need check why need to chek for null now?
                        subDirections.CollectionChanged += OnSubDirectionsChanged;

                    foreach (var subDirection in SubDirections)
                    {
                        subDirection.PropertyChanged += SubDirection_PropertyChanged;
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
                    if (subDirections != null)
                        foreach (var item in subDirections)
                        {
                            item.IsChecked = value;
                        }
                }
            }
        }
        public MainDirection(string name, ObservableCollection<SubDirection> subDirections)
        {
            Name = name;
            SubDirections = subDirections;
            IsChecked = false;
        }

        private void OnSubDirectionsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubDirection subDirection in e.NewItems)
                {
                    subDirection.PropertyChanged += SubDirection_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (SubDirection subDirection in e.OldItems)
                {
                    subDirection.PropertyChanged -= SubDirection_PropertyChanged;
                }
            }

            UpdateIsChecked();
        }

        private void SubDirection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubDirection.IsChecked))
            {
                UpdateIsChecked();
            }
        }

        private void UpdateIsChecked()
        {
            if (SubDirections.All(sd => sd.IsChecked))
            {
                isChecked = true;
            }
            else if (SubDirections.All(sd => !sd.IsChecked))
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