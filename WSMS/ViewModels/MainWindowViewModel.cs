using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Services;
using WSMS.ViewModels.Base;

namespace WSMS.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title
        private string _Title = "WSMS";
        /// <summary Header MainWindow </summary>
		public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion

        #region Comands
        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object parameter) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion
        #region UpdateChromeDriverComand
        public ICommand UpdateChromeDriverComand { get; }

        private bool CanUpdateChromeDriverComandExecute(object parameter) => true;

        private void OnUpdateChromeDriverComandExecuted(object p)
        {
            using (WebService driver = new()) 
            { 
                driver.UpdateChromeDriver();
            };
        }
        #endregion
        #endregion
        public MainWindowViewModel()
        {
            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            UpdateChromeDriverComand = new ActionCommand(OnUpdateChromeDriverComandExecuted, CanUpdateChromeDriverComandExecute);
        }
    }
}
