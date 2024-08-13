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
        /* private string? newTextMessage;*/
        /* public string? NewTextMessage
         {

             get => newTextMessage;
             set
             {
                 if (value == string.Empty)
                 {
                     IsChanged = false;
                 }
                 else if (newTextMessage == string.Empty)
                 {
                     newTextMessage = value;
                     if (newTextMessage != value)
                     {
                         IsChanged = true;
                     }
                 }
                 else if (OldTextMessage != value)
                 {
                     IsChanged = true;
                     newTextMessage = value;
                 }
                 else
                 {
                     IsChanged = false;
                 }
                 Set(ref newTextMessage, value);
             }
         }*/

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

                OldTextMessage = message.Text; // Инициализация OldTextMessage при установке Message
                Set(ref message, value);
            }
        }

        public MessageWrapper(Message message, string oldTextMessage = default, string newTextMessage = default, bool isChanged = false)
        {
            Message = message;
            OldTextMessage = Message.Text;
            OldImagePath = "D:\\Notes\\Работа Вова\\Discount\\35.png";//Message.ImagePath;
            IsChanged = isChanged;
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
