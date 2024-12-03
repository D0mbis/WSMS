using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WSMS.Models.Base;
using WSMS.Services;

namespace WSMS.Models
{
    public class Message : Model
    {
        public string? Name { get; set; } = string.Empty;
        private string? text = string.Empty;
        public string? Text
        {
            get => text;
            set
            {
                Set(ref text, value);
            }
        }
        private string? imagePath = string.Empty;
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
                //ImagePath = value?.ToString();
                Set(ref image, value);
            }
        }
        public ObservableCollection<MainDirection>? Directions { get; set; }
    }
}
