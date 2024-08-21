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
        public SubCategory(string name)
        {
            IsChecked = true;
            Name = name;
        }
    }
}