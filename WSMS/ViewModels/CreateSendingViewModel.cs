using System.Collections.ObjectModel;
using WSMS.Models;
using WSMS.Models.Base;

namespace WSMS.ViewModels
{
    public class CreateSendingViewModel : Model
    {
        private ObservableCollection<SubDiraction>? subDiractions;
        public ObservableCollection<SubDiraction>? SubDiractions { get => subDiractions; set => Set(ref subDiractions, value); }

        public CreateSendingViewModel()
        {
            SubDiractions = CustomersRepository.Instance.GetSubDiractions();
        }
    }
}
