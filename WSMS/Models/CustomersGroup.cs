﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WSMS.Models
{
    public class CustomersGroup
    {
        public string Category { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public bool IsSelected { get; set; }
    }
}