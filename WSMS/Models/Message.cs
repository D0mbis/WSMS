using System.Windows.Media.Imaging;
using WSMS.Services;

namespace WSMS.Models
{
    public class Message
    {
        public string[] Contacts { get; set; }  // to delete
        public string Text { get; set; }
        public BitmapSource? Image { get; set; }
        public string[] Categories { get; set; }

        public Message() { }    
        public Message (string messageText, BitmapSource image)
        {
            Text = messageText;
            Image = image;
        }
    }
}
