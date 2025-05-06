using OpenQA.Selenium;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WSMS.Infrastructure.Other;
using WSMS.Models;
using WSMS.ViewModels;

namespace WSMS.Views.Windows
{
    public partial class CustomersWindow : Window
    {
        public CustomersWindow()
        {
            InitializeComponent();
        }

        private void ContactsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
    }
}
