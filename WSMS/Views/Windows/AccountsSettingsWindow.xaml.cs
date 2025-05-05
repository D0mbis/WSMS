using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using WSMS.Services;
using WSMS.ViewModels;

namespace WSMS.Views
{
    /// <summary>
    /// Interaction logic for AccountsSettings.xaml
    /// </summary>
    public partial class AccountsSettingsWindow : Window
    {
        public AccountsSettingsWindow()
        {
            InitializeComponent();
            //DataContext = new WhatsAppAccountsSettingsViewModel();
        }
    }
}

