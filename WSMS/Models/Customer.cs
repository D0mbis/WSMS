using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMS.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string PhoneNumber3 { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }


        public Customer() { }
        public Customer(string name, string phoneNumber1, string phoneNumber2, string phoneNumber3, string mainCategory, string subCategory)
        {
            Name = name;
            PhoneNumber1 = phoneNumber1;
            PhoneNumber2 = phoneNumber2;
            PhoneNumber3 = phoneNumber3;
            MainCategory = mainCategory;
            SubCategory = subCategory;
        }
    }
}
