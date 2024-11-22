using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMS.Models
{
    public class Customer
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber1 { get; set; }
        public string? PhoneNumber2 { get; set; }
        public string? PhoneNumber3 { get; set; }
        public string? MainDiraction { get; set; }
        public string? SubDiraction { get; set; } 
        public string? Address { get; set; }
        public bool IsSelected { get; set; } = false;

        public Customer() { }
        public Customer(string id, string name, string phoneNumber1, string phoneNumber2, string phoneNumber3, 
            string mainDiraction, string subDiraction, string address)
        {
            ID = id;
            Name = name;
            PhoneNumber1 = phoneNumber1;
            PhoneNumber2 = phoneNumber2;
            PhoneNumber3 = phoneNumber3;
            MainDiraction = mainDiraction;
            SubDiraction = subDiraction;
            Address = address;
        }
    }
}
