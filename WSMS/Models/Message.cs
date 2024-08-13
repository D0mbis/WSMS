using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using WSMS.Models.Base;
using WSMS.Services;

namespace WSMS.Models
{
    public class Message : Model
    {
        public string? Name { get; set; }
        private string? text = "Start test value";
        public string? Text
        {
            get => text;
            set
            {
                Set(ref text, value);
            }
        }
        private string? imagePath;
        public string? ImagePath
        {
            get => imagePath;
            set => Set(ref imagePath, value);
        }
        private BitmapSource? image;
        public BitmapSource? Image
        {
            get => image;
            set
            {
                Set(ref image, value);
            }
        }
        [JsonIgnore]
        public string[]? Categories { get; set; }

        public Message() { }
        public Message(BitmapSource image, string name = default, string messageText = default, string imagePath = default, string[] categories = default)
        {
            Image = image;
            Name = name;
            Text = messageText ?? string.Empty;
            ImagePath = imagePath ?? string.Empty;
            Categories = categories;
        }
    }
}
