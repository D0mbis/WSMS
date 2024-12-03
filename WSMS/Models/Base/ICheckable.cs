using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMS.Models.Base
{
    public interface ICheckable
    {
        bool IsChecked { get; set; }
    }
}
