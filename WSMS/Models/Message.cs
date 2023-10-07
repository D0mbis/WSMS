using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WSMS.Models
{
    internal class Message 
    {
        public Message(string? messageText = default, BitmapSource? image = default)
        {
            Text = messageText;
            Image = image;
        }

        public string? Text { get; set; } = default;
        public BitmapSource? Image { get; set; } = default;
    }
}
