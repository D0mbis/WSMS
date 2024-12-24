using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WSMS.Models;
using WSMS.Services;
using WSMS.ViewModels;

namespace WSMS.Views.Windows
{
    /// <summary>
    /// Interaction logic for SaveMessageWindow.xaml
    /// </summary>
    public partial class SaveMessageWindow : Window
    {
        public static bool IsOpened;
        public SaveMessageWindow(MessageWrapper messageWrapper, VMUpdateService vMUpdateService)
        {
            DataContext = new SaveMessageViewModel(messageWrapper, vMUpdateService);
            InitializeComponent();
        }
    }
}
