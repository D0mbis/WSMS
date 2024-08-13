using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using WSMS.Infrastructure.Commands.Base;
using WSMS.Models;
using WSMS.Services;
using WSMS.ViewModels;

namespace WSMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public List<CustomersGroup> CustomerGroups { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            MessageService.LoadMessages();
            //GoogleSheetsAPI.PulldbCustomers();
            //CustomersService.AllCustomers;

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
