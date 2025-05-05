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
using WSMS.Infrastructure.Other;
using WSMS.ViewModels;

namespace WSMS.Views.Windows
{
    public partial class MessagesWindow : Window
    {
        public MessagesWindow( MessagesViewModel vm)
        {
            DataContext = vm;   
            InitializeComponent();
        }
        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is MessagesViewModel viewModel)
            {
                viewModel.ImageDropCommand.Execute(e);
            }
        }
    }
}
