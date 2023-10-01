using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
