using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
