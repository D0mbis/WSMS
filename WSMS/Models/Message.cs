using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WSMS.Models.Base;
using WSMS.Services;

namespace WSMS.Models
{
    public class Message : CheckableItemWithChildren<MessageAllowDirections>
    {
        public string? Name { get; set; }
        private string? text;
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
        public ObservableCollection<MessageAllowDirections>? Directions
        {
            get => Children;
            set => Children = value;
        }
    }
}
