using System.Collections.ObjectModel;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class SendingTemplate : Model
    {
        public string? Account { get; set; }
        public Message Message { get; set; }
        public ObservableCollection<LiteSubDirections>? SelectedCustomers { get; set; }
    }
}
