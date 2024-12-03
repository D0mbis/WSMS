using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WSMS.Models;
using WSMS.ViewModels;

namespace WSMS.Views.Windows
{
    /// <summary>
    /// Interaction logic for SelectedDirectionCustomersWindow.xaml
    /// </summary>
    public partial class SelectedDirectionCustomersWindow : Window
    {
        public SelectedDirectionCustomersWindow(SelectedDirectionCustomersViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
