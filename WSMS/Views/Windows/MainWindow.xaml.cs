using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Infrastructure.Other;
using WSMS.Models;
using WSMS.Services;
using WSMS.ViewModels;
using WSMS.Views.Windows;

namespace WSMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //GoogleSheetsAPI.PulldbCustomers();
            //CustomersService.AllCustomers;
            var window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(window => window.IsInitialized);
            WindowPositionSettings.RestoreWindowPosition(window);
        }
        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.ImageDropCommand.Execute(e);
            }
        }
    }
}
