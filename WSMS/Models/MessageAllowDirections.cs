using System.Collections.ObjectModel;
using WSMS.Models.Base;

namespace WSMS.Models
{
    /// <summary>
    /// All Allow diractions for Message (without customers)
    /// </summary>
    public class MessageAllowDirections: CheckableItemWithChildren<SubDirectionsNames>
    {
        public string? MainDirection { get; set; }
        public ObservableCollection<SubDirectionsNames>? SubDirections
        {
            get => Children;
            set => Children = value;
        }
    }
}
