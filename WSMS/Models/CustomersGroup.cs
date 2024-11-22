using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WSMS.Models
{
    public class CustomersGroup
    {
        public string SubDiraction { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public bool IsSelected { get; set; }
        public CustomersGroup(string category, ObservableCollection<Customer> customers)
        {
            SubDiraction = category;
            Customers = customers;
        }
    }
}
