using OpenQA.Selenium;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WSMS.Models;
using WSMS.ViewModels;

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
            CuntactsListView.SelectedItem = null;
        }

        private void CuntactsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CuntactsListView.SelectedItem = null;
                return;
            }
        }
    }
}
