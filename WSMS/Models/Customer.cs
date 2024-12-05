using Newtonsoft.Json;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class Customer : Model, ICheckable
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber1 { get; set; }
        public string? PhoneNumber2 { get; set; }
        public string? PhoneNumber3 { get; set; }
        public string? MainDirection { get; set; }
        public string? SubDirection { get; set; }
        public string? Address { get; set; }
        [JsonIgnore]
        public bool IsSelected { get; set; }
        private bool isChecked;
        public bool IsChecked { get => isChecked; set => Set(ref isChecked, value); }
    }
}
