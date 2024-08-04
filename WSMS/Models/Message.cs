using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using WSMS.Services;

namespace WSMS.Models
{
    public class Message
    {
        public string? Name { get; set; }
        public string? Text { get; set; } = "Start test value";
        public string? ImagePath { get; set; }
        [JsonIgnore]
        public BitmapSource? Image { get; set; }
        public string[]? Categories { get; set; }

        public Message() { }    
        public Message (string name, string messageText, string imagePath, BitmapSource image, string[] categories)
        {
            Name = name;
            Text = messageText;
            ImagePath = imagePath;
            Image = image;
            Categories = categories;
        }
    }
}
