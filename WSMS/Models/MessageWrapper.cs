using Newtonsoft.Json;
using System.ComponentModel;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class MessageWrapper : Model
    {
        [JsonIgnore]
        public string? OldTextMessage { get; set; } = string.Empty;
        [JsonIgnore]
        public string? OldImagePath { get; set; } = string.Empty;
        [JsonIgnore]
        public bool IsChanged { get; set; }
        private Message message;
        public Message Message
        {
            get => message;
            set
            {
                if (message != null)
                {
                    message.PropertyChanged -= Message_PropertyChanged;
                }

                message = value;

                if (message != null)
                {
                    message.PropertyChanged += Message_PropertyChanged;
                }
                if (OldTextMessage == null)
                {
                    OldTextMessage = message?.Text;
                }
                Set(ref message, value);
            }
        }

        public MessageWrapper(Message message)
        {
            Message = message;
            OldTextMessage = message.Text;
            OldImagePath = message.ImagePath;
            IsChanged = false;
        }

        private void Message_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Message.Text) || e.PropertyName == nameof(Message.ImagePath))
            {
                IsChanged = !string.Equals(OldTextMessage, Message.Text) || !string.Equals(OldImagePath, Message.ImagePath);
            }
        }
    }





}
