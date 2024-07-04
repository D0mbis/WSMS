using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WSMS.Models;

namespace WSMS.Services
{
    public class CustomersService
    {
        private static Dictionary<string, ObservableCollection<Customer>> AllCustomersInGroups;
        private static ObservableCollection<Customer> AllCustomers { get; set; }
        public static List<CustomersGroup> LoadCustomersGroup()
        {
            try
            {
                string dbPath = $"{Environment.CurrentDirectory}\\data\\dbCustomers.json";
                if (File.Exists(dbPath) && AllCustomersInGroups == default)
                {
                    var jsonData = File.ReadAllText(dbPath);
                    AllCustomersInGroups = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<Customer>>>(jsonData);
                    var customerGroups = new List<CustomersGroup>();

                    foreach (var entry in AllCustomersInGroups)
                    {
                        customerGroups.Add(new CustomersGroup
                        {
                            Category = entry.Key,
                            Customers = entry.Value
                        });
                    }
                    return customerGroups;
                }
                //MessageBox.Show("CustomersService.cs -> LoadCustomersGroup ERROR!\n");

                return default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("CustomersService.cs -> LoadCustomersGroup ERROR!\n" + ex.Message);
                return default;
            }
        }
        public static ObservableCollection<Customer> LoadAllCustomers()
        {
            if (AllCustomers == null)
            {
                LoadCustomersGroup();
                AllCustomers = new ObservableCollection<Customer>();
                foreach (var entry in AllCustomersInGroups)
                {
                    foreach (Customer customer in entry.Value)
                    {
                        AllCustomers.Add(customer);
                    }
                }

                return AllCustomers;
            }
            else { return AllCustomers; }
        }
        public static void SaveCustomerData(IList<IList<object>> values)
        {
            try
            {
                var dbCustomers = new Dictionary<string, List<Customer>>();
                // Fetch the data.
                if (values != default && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        if (row.Count < 7)
                        {
                            MessageBox.Show(row[0].ToString());
                        }
                        Customer customer = new()
                        {
                            ID = row[0]?.ToString() ?? "",
                            Name = row[1]?.ToString() ?? "",
                            PhoneNumber1 = row[2]?.ToString() ?? "",
                            PhoneNumber2 = row[3]?.ToString() ?? "",
                            PhoneNumber3 = row[4]?.ToString() ?? "",
                            MainCategory = row[5]?.ToString() ?? "",
                            SubCategory = row[6]?.ToString() ?? ""
                        };
                        row.Add("");
                        customer.Address = row[7]?.ToString() ?? "";

                        if (dbCustomers.ContainsKey(customer.MainCategory))
                            dbCustomers[customer.MainCategory].Add(customer);
                        else dbCustomers.Add(customer.MainCategory, new List<Customer>() { customer });
                        foreach (var regionCategory in dbCustomers.Keys)
                        {
                            dbCustomers[regionCategory].Sort((c1, c2) => c1.SubCategory.CompareTo(c2.SubCategory));
                        }
                    }
                    string json = JsonConvert.SerializeObject(dbCustomers, Formatting.Indented);
                    string folderPath = $"{Environment.CurrentDirectory}\\data";
                    if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }
                    File.WriteAllText($"{folderPath}\\dbCustomers.json", json);
                }
            }
            catch (Exception e)
            {
                Logger.Message += $"\nError from CustomersService.cs: {e.Message}\n";
                Logger.SaveReport("CustomersService.txt");
                var result = MessageBox.Show("Seve customers data error\nDetails in CustomersService.txt.\nOpen Reports folder?", "Update error",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start("explorer.exe", $"{Environment.CurrentDirectory}\\Reports");
                }
            }
        }
    }
}
