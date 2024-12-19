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
        public int GetCheckedCustomersCount()
        {
            return AllDataBase
                .SelectMany(main => main.SubDirections)
                .Where(sub => sub.IsChecked)
                .Sum(sub => sub.Customers?.Count(c => c.IsChecked) ?? 0);
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
                var mainDirection = new MainDirection(mainDirectionName, subDirections);
                foreach (var subEntry in mainEntry.Value)
                {
                    string subDirectionName = subEntry.Key;
                    var customers = new ObservableCollection<Customer>(subEntry.Value);

                    var subDirection = new SubDirection(subDirectionName, customers);
                    subDirections.Add(subDirection);
                }
                mainDirection.SubDirections = subDirections;
                mainDirections.Add(mainDirection);
            }
            return mainDirections;
        }
        public ObservableCollection<SubDirection> GetSubDirectionsFull()
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
        public ObservableCollection<MessageAllowDirections> GetSubDirections()
        {
            ObservableCollection<MessageAllowDirections> resault = new();
            foreach (var direction in AllDataBase)
            {
                ObservableCollection<SubDirectionsNames> temp = new();
                foreach (var sub in direction.SubDirections)
                {
                    temp.Add(new() { SubDirection = sub.Name });
                }
                resault.Add(new() {MainDirection = direction.Name, SubDirections = temp });
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
