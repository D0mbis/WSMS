using System.Windows.Media.Imaging;

namespace WSMS.Models
{
    internal class Message
    {
        public string[] Contacts { get; set; }
        public string? Text { get; set; } = default;
        public BitmapSource? Image { get; set; } = default;
        public Message(string[] contacts, string? messageText = default, BitmapSource? image = default)
        {
            Contacts = contacts;
            Text = messageText;
            Image = image;
        }
    }
}
