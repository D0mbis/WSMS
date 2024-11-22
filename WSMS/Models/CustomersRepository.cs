using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSMS.Services;

namespace WSMS.Models
{
    public class CustomersRepository
    {
        private static CustomersRepository instance;
        public Dictionary<string, Dictionary<string, List<Customer>>>? AllDataBaseDictionary { get; set; } = new(); 
        public ObservableCollection<MainDiractionFull>? AllDataBase { get; set; } = new();

        public static CustomersRepository Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomersRepository();
                return instance;
            }
        }

        private CustomersRepository() // need to check if an instance is always required?
        {
            AllDataBaseDictionary = CustomersService.GetMainDB();
        }

        public ObservableCollection<SubDiraction> GetSubDiractions()
        {
            ObservableCollection<SubDiraction> resault = new ObservableCollection<SubDiraction>();
            foreach (var diraction in AllDataBase)
            {
                foreach (var sub in diraction.SubDiractions)
                {
                    resault.Add(sub);
                }
            }
            return resault;
        }
        public ObservableCollection<Customer> GetCustomers()
        {
            if (AllDataBaseDictionary == null)
                return new ObservableCollection<Customer>();

            // Flatten the nested structure and collect all customers
            var customers = AllDataBaseDictionary
                .SelectMany(main => main.Value) // Flatten outer dictionary
                .SelectMany(sub => sub.Value)  // Flatten inner dictionary
                .ToList();                     // Collect all customers into a list

            // Convert to ObservableCollection
            var resault = new ObservableCollection<Customer>(customers);
            return resault;
        }
    }
}
