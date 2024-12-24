using WSMS.Models.Base;

namespace WSMS.Models
{
    public class SubDirectionsNames : Model, ICheckable
    {
        public string? SubDirection { get; set; }
        private bool isChecked;
        public bool IsChecked { get => isChecked; set => Set(ref isChecked, value); }
    }
}
