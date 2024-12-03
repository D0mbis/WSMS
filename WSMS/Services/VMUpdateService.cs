using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMS.Services
{
    public class VMUpdateService
    {
        public event Action? DataUpdated;

        public void UpdateData()
        {
            DataUpdated?.Invoke();
        }
    }
}
