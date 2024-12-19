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
        private MessageWrapper? messageWrapper;
        public MessageWrapper MessageWrapper
        {
            get => messageWrapper ?? new(new());
            set
            {
                Set(ref messageWrapper, value);
            }
        }

        private string selectAllButtonContent = "Unselect all";
        public string SelectAllButtonContent { get => selectAllButtonContent; set => Set(ref selectAllButtonContent, value); }

        #region SaveMessage Command
        public ICommand SaveMessageCommand { get; }
        private bool CanSaveMessageCommandExecute(object p)
        {
            if (MessageWrapper.Message.Name != string.Empty)
            { return true; }
            else return false;
        }
        private void OnSaveMessageCommandExecuted(object p)
        {
            MessageService.EditMessages(MessageWrapper);
            var window = Application.Current.Windows.OfType<SaveMessageWindow>().FirstOrDefault(window => window.IsVisible);

            window?.Close();
            VMUpdateService?.UpdateData();
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
            MessageWrapper.Message.Directions = MessageService.ChangeIsCheck(MessageWrapper.Message.Directions, flag);
        }
        #endregion

        public SaveMessageViewModel(MessageWrapper messageWrapper, VMUpdateService dataService)
        {
            VMUpdateService = dataService;
            MessageWrapper = messageWrapper;
            messageWrapper.Message.Directions = CustomersRepository.Instance.GetSubDirections() ?? new();
            SaveMessageCommand = new MyActionCommand(OnSaveMessageCommandExecuted, CanSaveMessageCommandExecute);
            SelectAllCommand = new MyActionCommand(OnSelectAllCommandExecuted);

        }

    }
}
