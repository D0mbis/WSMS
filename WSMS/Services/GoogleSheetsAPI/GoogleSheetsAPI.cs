using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        static UserCredential AutorizeToken;
        static IList<IList<object>> PullValues;
        private readonly static string FolderPath = $"{Environment.CurrentDirectory}\\Services\\GoogleSheetsAPI";

        public static void PushValues(string customerID = default, Customer customer = default)
        {
            if (GetAccess())
            {
                if (PullValues == null) { PulldbCustomers(); }
                try
                {
                    int rowIndex = PullValues?.Select((row, index) => new { row, index }).FirstOrDefault(x => x.row[0]?.ToString() == customerID)?.index + 2 ?? -1;
                    // Define request parameters.
                    string range = $"{SheetName}!A{rowIndex}"; // Adjust range as needed
                    var valueRange = new ValueRange();
                    valueRange.Values = new List<IList<object>> { CustomersService.ConvertToList(customer) };

                    // Отправка данных в таблицу
                    var updateRequest = Service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                    updateRequest.Execute();
                    // need to update old customer if exist in the repository HERE!!!
                }
                catch (Exception e)
                {
                    Logger.ShowMyReportMessageBox(e.Message, "GoogleSheetsAPI", "PushValues");
                }
            }
        }
        /// <summary>
        /// Pull all customers db from remote Excel and save in "data" folder.
        /// </summary>
        public static void PulldbCustomers()
        {
            if (GetAccess())
                try
                {
                    // Define request parameters.
                    string range = $"{SheetName}!A2:H"; // Adjust range as needed
                    SpreadsheetsResource.ValuesResource.GetRequest request =
                            Service.Spreadsheets.Values.Get(SpreadsheetId, range);
                    PullValues = request.Execute().Values;
                    CustomersService.GetMainDBFromExcelValues(PullValues);
                }
                catch (Exception e)
                {
                    Logger.ShowMyReportMessageBox(e.Message, "GoogleSheetsAPI", "PulldbCustomers");
                }
        }
        private static bool GetAccess()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            GetToken(cts);
            string credentialsPath = $"{FolderPath}\\credentials.json";
            string tokenPath = $"{FolderPath}\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-user";
            try
            {
                if (!File.Exists(tokenPath))
                {
                    var result = MessageBox.Show("Please log in in the browser window that opens. " +
                        "Click \"Ok\" when you complete authorization or \"Cancel\" to cancel", "Authorization"
                        , MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        if (File.Exists(tokenPath))
                        {
                            if (AutorizeToken.Token.IsExpired(AutorizeToken.Flow.Clock))
                            {
                                var resault = MessageBox.Show("Credentials was revoked\n Do you want to update them?", "Credentials error",
                                              MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                                if (resault == MessageBoxResult.Yes)
                                {
                                    Clipboard.SetText("https://console.cloud.google.com/apis/credentials?orgonly=true&project=bormarketcustomers&supportedpurview=organizationId");
                                    MessageBox.Show("Link was copied to clipboard, please insert to your browser search.");
                                    cts.Cancel();
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Authorization token was created.");
                                cts.Cancel();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Authorization token was not created, try again.");
                            cts.Cancel();
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Authorization token was not created, try again.");
                        cts.Cancel();
                        return false;
                    }
                }
                Service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = AutorizeToken,
                    ApplicationName = ApplicationName,
                });
                cts.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Logger.ShowMyReportMessageBox(e.Message, "GoogleSheetsAPI", "GetCredential");
                cts.Dispose();
                return false;
            }
        }
        private static async Task GetToken(CancellationTokenSource cts)
        {
            try
            {
                // Загружаем секреты клиента
                using (var stream = new FileStream($"{FolderPath}\\credentials.json", FileMode.Open, FileAccess.Read))
                {
                    AutorizeToken = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                         GoogleClientSecrets.FromStream(stream).Secrets,
                         Scopes,
                         "user",
                         cts.Token,
                         new FileDataStore(FolderPath, true));
                }
            }
            catch (AggregateException ex)
            {
                // Обработка ошибок, связанных с отменой авторизации или закрытием браузера
                if (ex.InnerException is TokenResponseException e)
                {
                    // Обработка конкретной ошибки, связанной с токеном
                    Logger.ShowMyReportMessageBox(e.Message, "GoogleSheetsAPI", "GetToken");
                }
                else if (ex.InnerException is TaskCanceledException)
                {
                    // Пользователь закрыл браузер или прервал процесс
                    Logger.ShowMyReportMessageBox("TaskCanceledException", "GoogleSheetsAPI", "GetToken: Autorization was canceled.");
                }
                else
                {
                    // Общая обработка других исключений
                    Logger.ShowMyReportMessageBox(ex.Message, "GoogleSheetsAPI", "catch (AggregateException ex)");
                }
            }
            catch (Exception ex)
            {
                // Общая обработка любых других непредвиденных ошибок
                Logger.ShowMyReportMessageBox(ex.Message, "GoogleSheetsAPI", "GetToken");
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
