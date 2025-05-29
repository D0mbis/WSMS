using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Infrastructure.Other;
using WSMS.Models;
using WSMS.Models.Base;
using WSMS.Services;
using WSMS.Views.Windows;

namespace WSMS.ViewModels
{
    public class MessagesViewModel : Model
    {
        private readonly VMUpdateService vmUpdateService;
        private MessageWrapper? selectedMessage;
        public MessageWrapper SelectedMessage
        {
            get => selectedMessage;
            set
            {
                Set(ref selectedMessage, value);
            }
        }

        ICollectionView? messagesView;
        public ICollectionView MessagesView
        {
            get => messagesView; set
            {
                Set(ref messagesView, value);
            }
        }

        #region OpenSaveMessageWindow Command
        public ICommand OpenSaveMessageWindowCommand { get; }
        private bool CanOpenSaveMessageWindowExecute(object p)
        {
            var window = Application.Current.Windows.OfType<SaveMessageWindow>().FirstOrDefault(window => window.IsVisible);
            if (SelectedMessage != null)
            {
                if (SelectedMessage.IsChanged && window == null) { return true; }
            }
            return false;
        }
        private void OnOpenSaveMessageWindowExecuted(object p)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(window => window.IsVisible);
            var saveMessageWindow = new SaveMessageWindow(SelectedMessage, vmUpdateService)
            {
                Left = mainWindow.Left + mainWindow.Width / 2,
                Top = mainWindow.Top + 50
            };

            saveMessageWindow.Show();
            //  SelectedMessage = new MessageWrapper(new Message()); // unnecessary?
        }
        #endregion
        #region DeleteMessage Command
        public ICommand DeleteMessageCommand { get; }
        private bool CanDeleteMessageCommandExecute(object p)
        {
            if (SelectedMessage != null)
            {
                if (SelectedMessage.Message.Name != string.Empty) return true;
            }
            return false;
        }
        private void OnDeleteMessageCommandExecuted(object p)
        {
            MessagesService.EditMessages(SelectedMessage, true);
            MessagesView = CollectionViewSource.GetDefaultView(MessagesService.LoadMessages());
            SelectedMessage = new MessageWrapper(new Message());
        }
        #endregion
        #region ImageDrop Command
        public ICommand ImageDropCommand { get; }
        private void OnImageDropCommandExecuted(object p)
        {
            if (p is DragEventArgs e && e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    var filePath = files[0];
                    try
                    {
                        SelectedMessage.Message.Image = MessagesService.GetImage(filePath);
                        SelectedMessage.Message.ImagePath = filePath;
                        //CommandManager.InvalidateRequerySuggested();
                        (OpenSaveMessageWindowCommand as MyActionCommand).RaiseCanExecuteChanged();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image: {ex.Message}");
                    }
                }
            }
        }
        #endregion
        #region CloseWindow
        public ICommand closeWindowCommand;
        public ICommand CloseWindowCommand => closeWindowCommand ?? new MyActionCommand(OnCloseWindowCommandExecuted);

        private void OnCloseWindowCommandExecuted(object p)
        {
            WindowMenager.CloseWindow<MessagesWindow>(true);
        }
        #endregion

        private void OnUpdateData()
        {
            MessagesView = CollectionViewSource.GetDefaultView(MessagesService.LoadMessages());
            SelectedMessage = new MessageWrapper(new Message());
        }

        public MessagesViewModel(MessageWrapper selectedMessage)
        {
            SelectedMessage = selectedMessage;
            vmUpdateService = new VMUpdateService();
            vmUpdateService.DataUpdated += OnUpdateData;
            MessagesView = CollectionViewSource.GetDefaultView(MessagesService.LoadMessages());
            OpenSaveMessageWindowCommand = new MyActionCommand(OnOpenSaveMessageWindowExecuted, CanOpenSaveMessageWindowExecute);
            DeleteMessageCommand = new MyActionCommand(OnDeleteMessageCommandExecuted, CanDeleteMessageCommandExecute);
            ImageDropCommand = new MyActionCommand(OnImageDropCommandExecuted);
        }
    }
}
