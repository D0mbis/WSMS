using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WSMS.Models;

namespace WSMS.Views.Windows
{
    /// <summary>
    /// Interaction logic for SaveMessageWindow.xaml
    /// </summary>
    public partial class SaveMessageWindow : Window
    {
        public static bool IsOpen = false;
        public SaveMessageWindow()
        {
            InitializeComponent();
            IsOpen = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsOpen = false;
        }

        /// If need to add unselected Message
        /*private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element)
            {
                if ((element.DataContext is Customer))
                {
                    return;
                }
                while (element != null && !(element is Button))
                {
                    element = (FrameworkElement)VisualTreeHelper.GetParent(element);
                }
                if (element == null || element is Button button && button.Name != PushtoExcelDB.Name)
                {
                    CuntactsListView.SelectedItem = null;
                    return;
                }
            }
        }*/
    }
}
