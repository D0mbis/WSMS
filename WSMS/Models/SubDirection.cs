using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using WSMS.Models.Base;

namespace WSMS.Models
{
    /// <summary>
    /// Sub categories for Message
    /// </summary>
    public class SubDirection : CheckableItemWithChildren<Customer>
    {
        public string Name { get; set; }
        public DateTime? LastSending { get; set; }
        public ObservableCollection<Customer> Customers
        {
            get => Children;
            set => Children = value;
        }

      
        public SubDirection(string name, ObservableCollection<Customer>? customers = default, DateTime? lastSending = default)
        {
            Name = name;
            LastSending = lastSending ?? new DateTime(2024, 11, 5).Date;
            Children = customers ?? new ObservableCollection<Customer>();
        }
    }
}