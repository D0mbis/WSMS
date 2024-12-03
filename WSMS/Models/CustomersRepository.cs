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
        public ObservableCollection<MainDirection> AllDataBase { get; set; } = new();

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
            AllDataBase = ConvertToMainDirections();
        }
        public ObservableCollection<MainDirection> ConvertToMainDirections()
        {
            if (AllDataBaseDictionary == null)
                return new ObservableCollection<MainDirection>();

            var mainDirections = new ObservableCollection<MainDirection>();

            foreach (var mainEntry in AllDataBaseDictionary)
            {
                string mainDirectionName = mainEntry.Key;
                var subDirections = new ObservableCollection<SubDirection>();
                foreach (var subEntry in mainEntry.Value)
                {
                    string subDirectionName = subEntry.Key;
                    var customers = new ObservableCollection<Customer>(subEntry.Value);
                    var subDirection = new SubDirection(subDirectionName, customers);
                    subDirections.Add(subDirection);
                }
                var mainDirection = new MainDirection(mainDirectionName, subDirections);
                mainDirections.Add(mainDirection);
            }
            return mainDirections;
        }
        public ObservableCollection<SubDirection> GetSubDirections()
        {
            ObservableCollection<SubDirection> resault = new();
            foreach (var direction in AllDataBase)
            {
                foreach (var sub in direction.SubDirections)
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
