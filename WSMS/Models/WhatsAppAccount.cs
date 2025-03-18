using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMS.Models
{
    public class WhatsAppAccount
    {
        public string? Name { get; set; }
        public int? AvailableForSending { get; set; } = 100;
    }
}
