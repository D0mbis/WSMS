using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using WSMS.Models;

namespace WSMS.Services
{
    public class CustomersService
    {
        public static List<CustomersGroup>? AllCustomersInGroups { get; private set; }
        private static ObservableCollection<Customer>? AllCustomers { get; set; }
        private static readonly string FolderPath = $"{Environment.CurrentDirectory}\\data";
        public static void LoadAllCustomersInGroups()
        {
            try
            {
                string dbPath = $"{Environment.CurrentDirectory}\\data\\dbCustomers.json";
                if (File.Exists(dbPath) && AllCustomersInGroups == default)
                {
                    AllCustomersInGroups = new();
                    var jsonData = File.ReadAllText(dbPath);
                    var items = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<Customer>>>(jsonData);
                    AllCustomersInGroups = new();
                    foreach (var item in items.Keys)
                    {
                        AllCustomersInGroups.Add(new CustomersGroup(item, items[item]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CustomersService.cs -> LoadAllCustomersInGroups ERROR!\n" + ex.Message);
            }
        }
        public static ObservableCollection<Customer> GetCustomersWithoutGroups()
        {
            if (AllCustomers == null)
            {
                LoadAllCustomersInGroups();
                AllCustomers = new ObservableCollection<Customer>();
                foreach (var entry in AllCustomersInGroups)
                {
                    foreach (Customer customer in entry.Customers)
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
                    if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); }
                    File.WriteAllText($"{FolderPath}\\dbCustomers.json", json);
                }
            }
            catch (Exception e)
            {
                Logger.ShowMyReportMessageBox(e.Message, "CustomersService", "Seve customers data error");
            }
        }

        internal static int GetRowIndex(string customerID, IList<IList<object>>? pullValues)
        {
            for (int i = 0; i < pullValues.Count; i++)
            {

                if (pullValues[i][0].ToString() == customerID)
                {
                    return i + 2;
                }
            }
            return -1;
        }

        internal static IList<object> ConvertToList(Customer customer)
        {
            IList<object> result = new List<object>();
            PropertyInfo[] properties = typeof(Customer).GetProperties();
            foreach (var item in properties)
            {
                if (item.Name != "IsSelected")
                    result.Add(item.GetValue(customer) ?? string.Empty);
            }
            return result;
        }
        public static void ImportToCSV()
        {
            try
            {
                if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); }
                if (AllCustomers.Count == 0) { GetCustomersWithoutGroups(); }
                using StreamWriter writer = new StreamWriter($"{FolderPath}\\dbCustomers.csv");
                // header
                writer.WriteLine("Notes\tName\tPhone 1 - Value\tPhone 2 - Value\tPhone 3 - Value\tGroup Membership\t " +
                    "Address 1 - Region\tOrganization 1 - Name");
                foreach (var customer in AllCustomers)
                {
                    var list = ConvertToList(customer);
                    foreach (string item in list)
                    {
                        writer.Write(item.ToString() + "\t");
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "CustomerService", "ImportToCSV Error");
            }
        }
    }
}
