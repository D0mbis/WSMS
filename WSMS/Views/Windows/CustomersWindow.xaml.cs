using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WSMS.Models;

namespace WSMS.Views.Windows
{
    /// <summary>
    /// Interaction logic for CustomersWindow.xaml
    /// </summary>
    public partial class CustomersWindow : Window
    {
        public static bool IsOpen = false;
        public CustomersWindow()
        {
            InitializeComponent();
            IsOpen = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsOpen = false;
        }

        private void CuntactsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is FrameworkElement element) || !(element.DataContext is Customer))
            {
                CuntactsListView.SelectedItem = null;
            }
        }
    }
}
