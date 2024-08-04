using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace WSMS.Models
{
    public class ExpandMessage : Message
    {
        [JsonIgnore]
        public string? OldTextMessage { get; set; } = string.Empty;
        [JsonIgnore]
        public string? NewTextMessage { get; set; }
        [JsonIgnore]
        public bool IsChanged { get; set; } 

        public ExpandMessage(string oldTextMessage = default, string newTextMessage = default, bool isChanged = false)
        {
            OldTextMessage = oldTextMessage;
            NewTextMessage = newTextMessage;    
            IsChanged = isChanged;
        }
    }

    

   

}
