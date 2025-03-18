using System.Text.Json.Serialization;
using WSMS.Models.Base;

namespace WSMS.Models
{
    public class SubDirectionName : Model, ICheckable
    {
        public string? SubDirection { get; set; }
        private bool isChecked;
        [JsonIgnore]
        public bool IsChecked { get => isChecked; set => Set(ref isChecked, value); }
    }
}
