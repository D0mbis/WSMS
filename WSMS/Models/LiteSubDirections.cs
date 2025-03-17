using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMS.Models
{
    public class LiteSubDirections
    {
        public string? Name { get; set; }
        public ObservableCollection<LiteCustomer>? Customers { get; set; }
    }
}
