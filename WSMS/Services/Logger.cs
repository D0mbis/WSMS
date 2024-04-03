using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WSMS.Services
{
    public static class Logger
    {
        private static readonly string dateNow = DateTime.Now.ToString("dd-MM-yy (HH.mm.ss)");
        public static void SaveSendingLogs(Dictionary<string, List<string>> resultSending)
        {
            try
            {
                string LogsFilesFolderPath = $"{Environment.CurrentDirectory}\\Logs";
                if (!Directory.Exists(LogsFilesFolderPath)) { Directory.CreateDirectory(LogsFilesFolderPath); }
                string LogsFilePath = $"{LogsFilesFolderPath}\\{dateNow}.txt";
                File.Create(LogsFilePath).Close();
                if (resultSending["Successful sent"].Count > 0 || resultSending["Not sent"].Count > 0)
                {
                    using (StreamWriter stream = new(LogsFilePath, true))
                    {
                        if (resultSending["Successful sent"].Count > 0)
                        {
                            stream.WriteLine($"Total was successfully sent {resultSending["Successful sent"].Count} messages for:\n"
                                + string.Join("\n", resultSending["Successful sent"]) + "\n");
                        }
                        if (resultSending["Not sent"].Count > 0)
                        {
                            stream.WriteLine($"Not sent {resultSending["Not sent"].Count} messages:\n" +
                                string.Join("\n", resultSending["Not sent"]) + "\n");
                        }
                        stream.WriteLine($"The message text was:\n{string.Join("\n", resultSending["Message Text"])}\n");
                        for (int i = 0; i < 10; i++) stream.Write("_");
                        stream.WriteLine($"\nErrors:\n{WebService.Errors}");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Logs error:\n{ex.Message}"); }
        }
        public static void SaveNotDeliveryReport(string[] reportsArray)
        {
            try
            {
                string reportsFolderPath = $"{Environment.CurrentDirectory}\\Reports";
                if (!Directory.Exists(reportsFolderPath)) { Directory.CreateDirectory(reportsFolderPath); }

                File.Create($"{reportsFolderPath}\\Were not delivered on {dateNow}.txt").Close();

                using (StreamWriter stream = new($"{reportsFolderPath}\\Were not delivered on {dateNow}.txt", true))
                {
                    if (reportsArray != Array.Empty<string>())
                    {
                        stream.WriteLine($"Were not delivered contacts:\n{string.Join("\n", reportsArray)}");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
