using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class MainDirection : CheckableItemWithChildren<SubDirection>
    {
        [JsonProperty(Order = 1)]
        public string Name { get; set; }
        [JsonProperty(Order = 2)]
        public ObservableCollection<SubDirection>? SubDirections
        {
            get => Children;
            set => Children = value;
        }
        public MainDirection(string name, ObservableCollection<SubDirection> subDirections)
        {
            Name = name;
            SubDirections = subDirections;
            IsChecked = false;
        }
    }
}