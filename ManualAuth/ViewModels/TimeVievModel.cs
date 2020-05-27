using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManualAuth.ViewModels
{
    public class TimeVievModel
    {
        private bool[] days = new bool[7];
        public bool[] Days { get { return days; } set { days = value; } }

        private bool all;
        public bool All { get { return all; } set { all = value; } }

                      
    }
}
