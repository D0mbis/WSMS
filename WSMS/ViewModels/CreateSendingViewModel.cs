using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSMS.Models;
using WSMS.Models.Base;

namespace WSMS.ViewModels
{
    class CreateSendingViewModel : Model
    {
        private List<SubDiraction>? subCategoriesList;
        public List<SubDiraction>? SubCategoriesList { get => subCategoriesList; set => Set(ref subCategoriesList, value); }

        CreateSendingViewModel()
        {
            SubCategoriesList = new List<SubDiraction>();
        }
    }
}
