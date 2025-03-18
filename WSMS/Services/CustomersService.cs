using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using WSMS.Models;

namespace WSMS.Services
{
    public class CustomersService
    {
        private static readonly string FolderPath = $"{Environment.CurrentDirectory}\\data";
        private static ObservableCollection<Customer>? AllCustomers { get; set; }

        public static Dictionary<string, Dictionary<string, List<Customer>>> GetMainDBFromExcelValues(IList<IList<object>> values)
        {
            try
            {
                Dictionary<string, Dictionary<string, List<Customer>>>? mainDB = new();
                if (values != default && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        var customer = ExcelRowToCustomer(row);
                        if (!mainDB.ContainsKey(customer.MainDirection))
                        {
                            mainDB.Add(customer.MainDirection, new Dictionary<string, List<Customer>>() { { customer.MainDirection, new List<Customer>() { customer } } });
                            continue;
                        }

                        if (!mainDB[customer.MainDirection].ContainsKey(customer.SubDirection))
                        {
                            mainDB[customer.MainDirection].Add(customer.SubDirection, new List<Customer>() { customer });
                        }
                        else
                        {
                            mainDB[customer.MainDirection][customer.SubDirection].Add(customer);
                        }

                    }
                    mainDB = mainDB
                             .OrderBy(outer => outer.Key) // Сортировка внешнего словаря по ключам (outer.Key)
                             .ToDictionary(
                                 outerKey => outerKey.Key, // Сохранение ключа внешнего словаря
                                 outerValue => outerValue.Value
                                     .OrderBy(inner => inner.Key) // Сортировка внутреннего словаря по ключам (inner.Key)
                                     .ToDictionary(
                                         innerKey => innerKey.Key, // Сохранение ключа внутреннего словаря
                                         innerValue => innerValue.Value // Сохранение списка Customer без изменений
                                     )
                             );
                }
                return mainDB;
            }
            catch (Exception e)
            {
                Logger.ShowMyReportMessageBox(e.Message, "CustomersService", "GetMainDBFromExcelValues data error");
                return new Dictionary<string, Dictionary<string, List<Customer>>>();
            }
        }

        private static Customer ExcelRowToCustomer(IList<object> row)
        {
            if (row.Count < 8) row.Add("");
            return new()
            {
                ID = row[0]?.ToString() ?? "",
                Name = row[1]?.ToString() ?? "",
                PhoneNumber1 = row[2]?.ToString() ?? "",
                PhoneNumber2 = row[3]?.ToString() ?? "",
                PhoneNumber3 = row[4]?.ToString() ?? "",
                MainDirection = row[5]?.ToString() ?? "",
                SubDirection = row[6]?.ToString() ?? "",
                Address = row[7]?.ToString() ?? ""
            };
        }
        public static IList<object> ConvertToList(Customer customer)
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
                AllCustomers ??= CustomersRepository.Instance.GetCustomers();
                using StreamWriter writer = new($"{FolderPath}\\mainDB.csv");
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
        public static Dictionary<string, Dictionary<string, List<Customer>>> GetMainDB()
        {
            try
            {
                string dbPath = FolderPath + "\\mainDB.json";
                if (File.Exists(dbPath))
                {
                    /* if (!File.Exists(dbPath)) { GoogleSheetsAPI.PulldbCustomers(); }*/ //TEMP!!
                    string jsonData;
                    using StreamReader reader = new(dbPath);
                    jsonData = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Customer>>>>(jsonData);
                }
                return new();
            }
            catch { return new Dictionary<string, Dictionary<string, List<Customer>>>(); }
        }

        public static ObservableCollection<LiteSubDirections> GetLiteSubDirections(ObservableCollection<SubDirection> subDirectionsInput)
        {
            ObservableCollection<LiteSubDirections> resault = new();
            foreach (SubDirection subdirection in subDirectionsInput)
            {
                ObservableCollection<LiteCustomer> customers = new(
                    subdirection.Customers.Select(c => new LiteCustomer { Name = c.Name })
                    );
                resault.Add(new LiteSubDirections() { Name = subdirection.Name, Customers = customers});
            }
            return resault;
        }


        public static void SaveNewDBCustomers(Dictionary<string, Dictionary<string, List<Customer>>> receivedBD)
        {
            string allDirectionsSortedJson = JsonConvert.SerializeObject(receivedBD, Formatting.Indented);
            if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); }
            using StreamWriter stream = new(FolderPath + "\\mainDB.json");
            stream.Write(allDirectionsSortedJson);
        }
        private static Dictionary<string, List<SubDirection>> UpdateDirectionsDateTime(string allDirectionsPath, Dictionary<string, List<SubDirection>> newDirections)
        {
            string jsonData;
            using (FileStream fs = new(allDirectionsPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using StreamReader reader = new(fs);
                jsonData = reader.ReadToEnd();
            }
            File.Delete(allDirectionsPath);
            Dictionary<string, List<SubDirection>> oldDirections = JsonConvert.DeserializeObject<Dictionary<string, List<SubDirection>>>(jsonData);
            foreach (var newMainDirection in newDirections)
            {
                foreach (var newSubDirection in newMainDirection.Value)
                {
                    foreach (var oldSubDirectionList in oldDirections.Values)
                    {
                        foreach (var oldSubDirection in oldSubDirectionList)
                        {
                            if (oldSubDirection.Name == newSubDirection.Name)
                            {
                                newSubDirection.LastSending = oldSubDirection.LastSending;
                            }
                        }
                    }
                }
            }
            return newDirections;
        }
    }
}
