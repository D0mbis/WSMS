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
        /* private static void SaveAllDiractions(Dictionary<string, Dictionary<string, List<Customer>>> mainDB)
         {
             Dictionary<string, List<SubDiraction>> result = mainDB.ToDictionary(
                     outerKey => outerKey.Key, // Ключ MainDiraction остаётся таким же
                     outerValue => outerValue.Value.Select(inner => new SubDiraction(inner.Key)).ToList()); // Преобразуем в List<SubDiraction>
             string allDiractionsPath = FolderPath + "\\all diractions.json";
             if (File.Exists(allDiractionsPath) && result.Count > 0)
             {
                 result = UpdateDiractionsDateTime(allDiractionsPath, result);
                 string allDiractionsJson = JsonConvert.SerializeObject(result, Formatting.Indented);
                 using StreamWriter stream = new(allDiractionsPath);
                 stream.Write(allDiractionsJson);
             }

         }*/
        /* public static ObservableCollection<MainDiraction> LoadAllDiractions()
         {
             ObservableCollection<MainDiraction> diractionsFullCollection = new();
             try
             {
                 string filePath = FolderPath + "\\all diractions.json";
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
                         diractionsFullCollection.Add(new MainDiraction(key, new ObservableCollection<SubDiraction>()));

                         foreach (var item in s[key])
                         {
                             foreach (var item2 in diractionsFullCollection)
                             {
                                 if (item2.Name == key)
                                 {
                                     item2.SubDiractions.Add(new SubDiraction(item));
                                 }
                             }
                         }
                     }
                 }
                 return diractionsFullCollection;
             }
             catch (Exception e)
             {
                 Logger.ShowMyReportMessageBox(e.Message, "CustomersService", "LoadAllDiractions");
                 return diractionsFullCollection;
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
                        if (!mainDB.ContainsKey(customer.MainDiraction))
                        {
                            mainDB.Add(customer.MainDiraction, new Dictionary<string, List<Customer>>() { { customer.MainDiraction, new List<Customer>() { customer } } });
                            continue;
                        }

                        if (!mainDB[customer.MainDiraction].ContainsKey(customer.SubDiraction))
                        {
                            mainDB[customer.MainDiraction].Add(customer.SubDiraction, new List<Customer>() { customer });
                        }
                        else
                        {
                            mainDB[customer.MainDiraction][customer.SubDiraction].Add(customer);
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
                MainDiraction = row[5]?.ToString() ?? "",
                SubDiraction = row[6]?.ToString() ?? "",
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
                if (!File.Exists(dbPath)) { GoogleSheetsAPI.PulldbCustomers(); }
                string jsonData;
                using StreamReader reader = new(dbPath);
                jsonData = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Customer>>>>(jsonData);
            }
            catch { return new Dictionary<string, Dictionary<string, List<Customer>>>(); }
        }

        public static void SaveNewDBCustomers(Dictionary<string, Dictionary<string, List<Customer>>> receivedBD)
        {
            string allDiractionsSortedJson = JsonConvert.SerializeObject(receivedBD, Formatting.Indented);
            if (!Directory.Exists(FolderPath)) { Directory.CreateDirectory(FolderPath); }
            using StreamWriter stream = new(FolderPath + "\\mainDB.json");
            stream.Write(allDiractionsSortedJson);
        }
        private static Dictionary<string, List<SubDiraction>> UpdateDiractionsDateTime(string allDiractionsPath, Dictionary<string, List<SubDiraction>> newDiractions)
        {
            string jsonData;
            using (FileStream fs = new(allDiractionsPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using StreamReader reader = new(fs);
                jsonData = reader.ReadToEnd();
            }
            File.Delete(allDiractionsPath);
            Dictionary<string, List<SubDiraction>> oldDiractions = JsonConvert.DeserializeObject<Dictionary<string, List<SubDiraction>>>(jsonData);
            foreach (var newMainDiraction in newDiractions)
            {
                foreach (var newSubDiraction in newMainDiraction.Value)
                {
                    foreach (var oldSubDiractionList in oldDiractions.Values)
                    {
                        foreach (var oldSubDiraction in oldSubDiractionList)
                        {
                            if (oldSubDiraction.Name == newSubDiraction.Name)
                            {
                                newSubDiraction.LastSending = oldSubDiraction.LastSending;
                            }
                        }
                    }
                }
            }
            return newDiractions;
        }
    }
}
