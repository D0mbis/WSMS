using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WSMS.Views.Windows;

namespace WSMS.Infrastructure.Other
{
    public static class WindowMenager
    {
        public static void RestoreWindowPosition(Window window)
        {
            // Restore window position and size
            if (Properties.Settings.Default.WindowTop != 0)
                window.Top = Properties.Settings.Default.WindowTop;
            if (Properties.Settings.Default.WindowLeft != 0)
                window.Left = Properties.Settings.Default.WindowLeft;
            if (Properties.Settings.Default.WindowHeight != 0)
                window.Height = Properties.Settings.Default.WindowHeight;
            if (Properties.Settings.Default.WindowWidth != 0)
                window.Width = Properties.Settings.Default.WindowWidth;

            // Restore window state
            if (Properties.Settings.Default.WindowState != null)
            {
                WindowState state;
                if (Enum.TryParse(Properties.Settings.Default.WindowState, out state))
                {
                    window.WindowState = state;
                }
            }
        }
        /// <summary>
        /// Open a new window centered relative to the parent window
        /// </summary>
        /// <typeparam name="TParent">parentWindow</typeparam>
        /// <param name="сhildWindow">сhildWindow</param>
        public static void OpenWindowCentered<TParent>(Window сhildWindow) where TParent : Window
        {
            var parentWindow = Application.Current.Windows.OfType<TParent>().FirstOrDefault(w => w.IsVisible);
            if (parentWindow == null) return;

            сhildWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            сhildWindow.Left = parentWindow.Left + (parentWindow.Width - сhildWindow.Width) / 2;
            сhildWindow.Top = parentWindow.Top + (parentWindow.Height - сhildWindow.Height) / 2;

            сhildWindow.Show();
        }

        static void SaveWindowPosition(Window window)
        {
            // Save window position, size, and state
            Properties.Settings.Default.WindowTop = window.Top;
            Properties.Settings.Default.WindowLeft = window.Left;
            Properties.Settings.Default.WindowHeight = window.Height;
            Properties.Settings.Default.WindowWidth = window.Width;
            Properties.Settings.Default.WindowState = window.WindowState.ToString(); // Save as string

            // Save the settings
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Save the current position of the selected window
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="savePosition">If you need to save please specify true</param>
        public static void CloseWindow<T>(bool savePosition = false) where T : Window
        {
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault(w => w.IsVisible);
            if (window != null)
            {
                if (savePosition)
                {
                    SaveWindowPosition(window);
                }
                window.Hide();
                window?.Close();
            }
        }
    }
}
