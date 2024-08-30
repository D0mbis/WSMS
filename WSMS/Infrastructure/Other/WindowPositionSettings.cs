using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WSMS.Infrastructure.Other
{
    public static class WindowPositionSettings
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

        public static void SaveWindowPosition(Window window)
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
    }
}
