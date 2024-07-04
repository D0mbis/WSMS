using System.Windows;

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
    }
}
