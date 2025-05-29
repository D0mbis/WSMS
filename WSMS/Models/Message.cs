using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class Message : CheckableItemWithChildren<MessageAllowDirections>
    {
        public string? Name { get; set; }
        private string? text;
        public string? Text
        {
            get => text; set => Set(ref text, Regex.Unescape(value ?? ""));
        }
        private string? imagePath;
        public string? ImagePath
        {
            get => imagePath;
            set => Set(ref imagePath, value);
        }
        private BitmapSource? image = default;
        [JsonIgnore]
        public BitmapSource? Image
        {
            get => image ?? new BitmapImage(new Uri("pack://application:,,,/data/messages/placeholder.png"));
            set
            {
                Set(ref image, value);
            }
        }
        [JsonIgnore]
        public ObservableCollection<MessageAllowDirections>? Directions
        {
            get => Children;
            set => Children = value;
        }
    }
}
