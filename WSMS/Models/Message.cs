using System.Windows.Media.Imaging;
using WSMS.Services;

namespace WSMS.Models
{
    internal class Message
    {
        public string[] Contacts { get; set; }
        public string? Text { get; set; } = default;
        public static BitmapSource? Image { get; set; } = default;
        public Message(string[] contacts, string? messageText = default)
        {
            Contacts = contacts;
            Text = messageText;
        }
    }
}
