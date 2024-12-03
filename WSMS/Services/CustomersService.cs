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


        /*private static bool LoadAllCustomersInGroups()
        {
            try
            {
                string dbPath = FolderPath + "\\mainDB.json";
                if (!File.Exists(dbPath))
                {
                    //GoogleSheetsAPI.PulldbCustomers();
                    MessageBox.Show("File \"mainDB.json\" not found, please pull from Excel db.");
                    return false;
                }
                else if (AllCustomersInGroups == default)
                {
                    AllCustomersInGroups = new();
                    string jsonData;
                    using (StreamReader reader = new(dbPath))
                    {
                        jsonData = reader.ReadToEnd();
                    }

                    var items = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<Customer>>>(jsonData);
                    AllCustomersInGroups = new();
                    foreach (var item in items.Keys)
                    {
                        AllCustomersInGroups.Add(new CustomersGroup(item, items[item]));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "CustomersService", "LoadCustomersInGroups method");
                return false;
            }
        }  // отредактировать после pull from excel*/
        /*public static ObservableCollection<Customer> GetCustomersWithoutGroups()
        {
            try
            {

                if (LoadAllCustomersInGroups())
                {
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
                else
                {
                    return AllCustomers;
                }
            }
            catch (Exception ex)
            {
                Logger.ShowMyReportMessageBox(ex.Message, "CustomersService", "GetCustomersWithoutGroups");
                AllCustomers = null;
                return AllCustomers;
            }
        }*/
        /// <summary>
        /// Returns the entire database as a dictionary.
        /// </summary>
        /// <param name="values">Values from Excel table</param>
        /// <returns></returns>
        /* private static void SaveAllDirections(Dictionary<string, Dictionary<string, List<Customer>>> mainDB)
         {
             Dictionary<string, List<SubDirection>> result = mainDB.ToDictionary(
                     outerKey => outerKey.Key, // Ключ MainDirection остаётся таким же
                     outerValue => outerValue.Value.Select(inner => new SubDirection(inner.Key)).ToList()); // Преобразуем в List<SubDirection>
             string allDirectionsPath = FolderPath + "\\all directions.json";
             if (File.Exists(allDirectionsPath) && result.Count > 0)
             {
                 result = UpdateDirectionsDateTime(allDirectionsPath, result);
                 string allDirectionsJson = JsonConvert.SerializeObject(result, Formatting.Indented);
                 using StreamWriter stream = new(allDirectionsPath);
                 stream.Write(allDirectionsJson);
             }

         }*/
        /* public static ObservableCollection<MainDirection> LoadAllDirections()
         {
             ObservableCollection<MainDirection> directionsFullCollection = new();
             try
             {
                 string filePath = FolderPath + "\\all directions.json";
                 if (!File.Exists(filePath)) { GoogleSheetsAPI.PulldbCustomers(); }
                 string data;
                 using (StreamReader reader = new(filePath))
                 {
                     data = reader.ReadToEnd();
                 }
                 if (data != string.Empty)
                 {
                     var s = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(data);
                     foreach (var key in s.Keys)
                     {
                         directionsFullCollection.Add(new MainDirection(key, new ObservableCollection<SubDirection>()));

                         foreach (var item in s[key])
                         {
                             foreach (var item2 in directionsFullCollection)
                             {
                                 if (item2.Name == key)
                                 {
                                     item2.SubDirections.Add(new SubDirection(item));
                                 }
                             }
                         }
                     }
                 }
                 return directionsFullCollection;
             }
             catch (Exception e)
             {
                 Logger.ShowMyReportMessageBox(e.Message, "CustomersService", "LoadAllDirections");
                 return directionsFullCollection;
             }
         }*/
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
