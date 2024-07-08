using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using WSMS.Models;

namespace WSMS.Services
{
    public class GoogleSheetsAPI
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static SheetsService Service;
        static readonly string ApplicationName = "BorMarketCustomers";
        static readonly string SpreadsheetId = "1PjdTKdxkMQQ5kZDJnnuNrUVOhwdm2JztKUSQFsAL8Bs"; // Replace with your Google Sheets ID
        static readonly string SheetName = "БД клиентов"; // Replace with your sheet name if different
        static UserCredential Credential;
        static IList<IList<object>> PullValues;
        private readonly static string FolderPath = $"{Environment.CurrentDirectory}\\Services\\GoogleSheetsAPI";

        private static bool GetCredentials()
        {
            string credentialsPath = $"{FolderPath}\\credentials.json";
            try
            {
                if (Credential == default)
                {
                    if (!File.Exists(credentialsPath)) { using FileStream fileStream = new(credentialsPath, FileMode.Create); }
                    //string folderPath = $"{Environment.CurrentDirectory}\\Services\\GoogleSheetsAPI";
                    using (var stream = new FileStream($"{FolderPath}\\credentials.json", FileMode.Open, FileAccess.Read))
                    {
                        Credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(FolderPath, true)).Result;
                    }
                }
                Service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = Credential,
                    ApplicationName = ApplicationName,
                });
                return true;
            }
            catch (Exception e)
            {
                Logger.Message += $"\nError from GoogleSheetsAPI.cs||GetCredential: {e.Message}\n";
                Logger.SaveReport("GoogleSheetsAPI.txt");
                var resault = MessageBox.Show("Credentials was revoked\n Do you want to update them?", "Credentials error",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (resault == MessageBoxResult.Yes)
                {
                    Clipboard.SetText("https://console.cloud.google.com/apis/credentials?orgonly=true&project=bormarketcustomers&supportedpurview=organizationId");
                    MessageBox.Show("Link was copied to clipboard, please insert to your browser search.");
                }
                return false;
            }
        }
        public static void PulldbCustomers()
        {
            if (GetCredentials())
                try
                {
                    // Define request parameters.
                    string range = $"{SheetName}!A2:H"; // Adjust range as needed
                    SpreadsheetsResource.ValuesResource.GetRequest request =
                            Service.Spreadsheets.Values.Get(SpreadsheetId, range);
                    PullValues = request.Execute().Values;
                    CustomersService.SaveCustomerData(PullValues);
                }
                catch (Exception e)
                {
                    Logger.Message += $"Error from GoogleSheetsAPI.cs||PulldbCustomers: {e.Message}\n";
                    Logger.SaveReport("GoogleSheetsAPI.txt");
                }
        }
        public static void PushValues(string customerID = default, Customer customer = default)
        {
            if (GetCredentials())
            {
                if (PullValues == null) { PulldbCustomers(); }
                try
                {
                    
                    int rowIndex = CustomersService.GetRowIndex(customerID, PullValues);
                    // Define request parameters.
                    string range = $"{SheetName}!A{rowIndex}"; // Adjust range as needed
                    var valueRange = new ValueRange();
                    valueRange.Values = new List<IList<object>> { CustomersService.ConvertToRow(customer) };

                    // Отправка данных в таблицу
                    var updateRequest = Service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                    updateRequest.Execute();
                }
                catch (Exception e)
                {
                    Logger.Message += $"Error from GoogleSheetsAPI.cs||PushValues: {e.Message}\n";
                    Logger.SaveReport("GoogleSheetsAPI.txt");
                }
            }
        }
        public static void AddNewCredentials()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Select credentials .json file";
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFilePath = openFileDialog.FileName;
                string destinationFilePath = Path.Combine(FolderPath, "credentials.json");
                try
                {
                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                    }
                    File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                    MessageBox.Show("Credential was added successfully!", "Successfull", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"Credential was not added: {ioEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
