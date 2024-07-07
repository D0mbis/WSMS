using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WSMS.Models
{
    public class CustomersGroup
    {
        public string Category { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public bool IsSelected { get; set; }
        public CustomersGroup() { }
        public CustomersGroup(string category, ObservableCollection<Customer> customers)
        {
            Category = category;
            Customers = customers;
        }
    }
}
