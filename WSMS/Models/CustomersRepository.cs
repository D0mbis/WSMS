using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WSMS.Services;

namespace WSMS.Models
{
    public class CustomersRepository
    {
        private static CustomersRepository? instance;
        public Dictionary<string, Dictionary<string, List<Customer>>>? AllDataBaseDictionary { get; set; } = new();
        public ObservableCollection<MainDiraction> AllDataBase { get; set; } = new();

        public static CustomersRepository Instance
        {
            get
            {
                instance ??= new CustomersRepository();
                return instance;
            }
        }

        private CustomersRepository()
        {
            AllDataBaseDictionary = CustomersService.GetMainDB();
            AllDataBase = ConvertToMainDiractions();
        }
        public ObservableCollection<MainDiraction> ConvertToMainDiractions()
        {
            if (AllDataBaseDictionary == null)
                return new ObservableCollection<MainDiraction>();

            var mainDiractions = new ObservableCollection<MainDiraction>();

            foreach (var mainEntry in AllDataBaseDictionary)
            {
                string mainDirectionName = mainEntry.Key;
                var subDiractions = new ObservableCollection<SubDiraction>();
                foreach (var subEntry in mainEntry.Value)
                {
                    string subDirectionName = subEntry.Key;
                    var customers = new ObservableCollection<Customer>(subEntry.Value);
                    var subDiraction = new SubDiraction(subDirectionName, customers);
                    subDiractions.Add(subDiraction);
                }
                var mainDiraction = new MainDiraction(mainDirectionName, subDiractions);
                mainDiractions.Add(mainDiraction);
            }
            return mainDiractions;
        }
        public ObservableCollection<SubDiraction> GetSubDiractions()
        {
            ObservableCollection<SubDiraction> resault = new();
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
