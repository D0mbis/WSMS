using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WSMS.Models;

namespace WSMS.Services
{
    public class GoogleSheetsAPI
    {
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static readonly string ApplicationName = "BorMarketCustomers";
        static readonly string SpreadsheetId = "1PjdTKdxkMQQ5kZDJnnuNrUVOhwdm2JztKUSQFsAL8Bs"; // Replace with your Google Sheets ID
        static readonly string SheetName = "CustomersForSanding"; // Replace with your sheet name if different
        static UserCredential Credential;

        private static bool GetCredential()
        {
            try
            {
                if (Credential == default)
                {
                    string folderPath = $"{Environment.CurrentDirectory}\\Services\\GoogleSheetsAPI";
                    using (var stream = new FileStream($"{Environment.CurrentDirectory}\\Services\\GoogleSheetsAPI\\credentials.json", FileMode.Open, FileAccess.Read))
                    {
                        Credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(folderPath, true)).Result;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Message += $"\nError from GoogleSheetsAPI.cs||GetCredential: {e.Message}\n";
                Logger.SaveReport("GoogleSheetsAPI.txt");
                return false;
            }
        }

        private static IList<IList<object>> GetValues()
        {
            try
            {
                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = Credential,
                    ApplicationName = ApplicationName,
                });

                // Define request parameters.
                string range = $"{SheetName}!A2:F"; // Adjust range as needed
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(SpreadsheetId, range);
                return request.Execute().Values;
            }
            catch (Exception e)
            {
                Logger.Message += $"\nError from GoogleSheetsAPI.cs||GetValues: {e.Message}\n";
                Logger.SaveReport("GoogleSheetsAPI.txt");
                return default;
            }
        }
        public static void PulldbCustomers()
        {
            IList<IList<object>> values = default;
            if (GetCredential()) values = GetValues();
            try
            {
                // Fetch the data.
                List<Customer> list = new();
                var dbCustomers = new Dictionary<string, List<Customer>>();
                if (values != default && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        Customer customer = new Customer();
                        customer.Name = row[0]?.ToString() ?? "";
                        customer.PhoneNumber1 = row[1]?.ToString() ?? "";
                        customer.PhoneNumber2 = row[2]?.ToString() ?? "";
                        customer.PhoneNumber3 = row[3]?.ToString() ?? "";
                        customer.MainCategory = row[4]?.ToString() ?? "";
                        customer.SubCategory = row[5]?.ToString() ?? "";
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
                Logger.Message += $"\nError from PulldbCustomers.cs: {e.Message}\n";
                Logger.SaveReport("GoogleSheetsAPI.txt");
                var result = MessageBox.Show("Update dbCustomers error.\nOpen Reports folder?", "Update error", 
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
