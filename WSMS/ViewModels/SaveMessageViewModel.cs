using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Services;
using WSMS.Views.Windows;

namespace WSMS.ViewModels
{
    public class SaveMessageViewModel : Model
    {
        private readonly VMUpdateService? VMUpdateService;
        private MessageWrapper? message;
        public MessageWrapper Message
        {
            get => message ?? new(new());
            set
            {
                Set(ref message, value);
            }
        }

        private string selectAllButtonContent = "Unselect all";
        public string SelectAllButtonContent { get => selectAllButtonContent; set => Set(ref selectAllButtonContent, value); }

        private ObservableCollection<MainDiraction>? customersDiractions;
        public ObservableCollection<MainDiraction> CustomersDiractions
        {
            get => customersDiractions ?? new();
            set
            {
                if (customersDiractions != value)
                {

                    Set(ref customersDiractions, value);
                }
            }
        }

        #region SaveMessage Command
        public ICommand SaveMessageCommand { get; }
        private bool CanSaveMessageCommandExecute(object p)
        {
            if (Message.Message.Name != string.Empty)
            { return true; }
            else return false;
        }
        private void OnSaveMessageCommandExecuted(object p)
        {
            Message.Message.Diractions = MessageService.RemoveUnselectedDiractions(CustomersDiractions);
            MessageService.UpdateMessages(Message);
            var window = Application.Current.Windows.OfType<SaveMessageWindow>().FirstOrDefault(window => window.IsVisible);

            window?.Close();
            VMUpdateService.UpdateData();
            //Message = new MessageWrapper(new Message());
        }
        #endregion

        #region Select all Command

        public ICommand SelectAllCommand { get; }

        // private bool CanSelectAllCommandExecute(object p) => true;
        private void OnSelectAllCommandExecuted(object p)
        {
            bool flag;
            if (SelectAllButtonContent == "Select all")
            {
                flag = true;
                SelectAllButtonContent = "Unselect all";
            }
            else
            {
                SelectAllButtonContent = "Select all";
                flag = false;
            }
            CustomersDiractions = MessageService.ChangeIsCheck(CustomersDiractions, flag);
        }
        #endregion

        public SaveMessageViewModel(MessageWrapper message, VMUpdateService dataService)
        {
            VMUpdateService = dataService;
            CustomersDiractions = CustomersRepository.Instance.AllDataBase ?? new();
            Message = message;
            SaveMessageCommand = new MyActionCommand(OnSaveMessageCommandExecuted, CanSaveMessageCommandExecute);
            SelectAllCommand = new MyActionCommand(OnSelectAllCommandExecuted);

        }

    }
}
