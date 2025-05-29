using System.Windows;

namespace WSMS.Services
{
    public static class CustomMessageBox
    {
        public static MessageBoxResult ShowTopMost(
            string message,
            string caption = "",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None)
        {
            // Создаём невидимое окно-владелец с TopMost
            Window topmostWindow = new Window
            {
                Topmost = true,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Width = 0,
                Height = 0,
                Left = -10000,
                Top = -10000
            };
            topmostWindow.Show();
            topmostWindow.Activate();

            // Показываем MessageBox с этим окном как owner
            MessageBoxResult result = MessageBox.Show(topmostWindow, message, caption, buttons, icon);

            topmostWindow.Close();
            return result;
        }
    }
}