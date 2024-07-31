using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using WSMS.Models;
using WSMS.Services;

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
            //GoogleSheetsAPI.PulldbCustomers();
            //CustomersService.AllCustomers;

        }
        private void Image_Drop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            //DropLabel.Visibility = Visibility.Collapsed;
            // ImageBlock.Source = MessageService.GetImage(file[0]);
        }
    }
}
