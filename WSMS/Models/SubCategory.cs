using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using WSMS.Models.Base;

namespace WSMS.Models
{
    /// <summary>
    /// Sub categories for Message
    /// </summary>
    public class SubCategory : Model
    {
        public string Name { get; set; }
        public DateTime? LastSending { get; set; }
        
        private bool isChecked;
        //private string name;

        [JsonIgnore]
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    Set(ref isChecked, value);
                }
            }
        }
        public SubCategory(string name, DateTime? lastSending = default)
        {
            IsChecked = true;
            Name = name;
            LastSending = lastSending ?? new DateTime(2024, 11, 5).Date;
        }
    }
}