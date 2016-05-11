using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.ClientObjects;

namespace Client.ClientObjects
{
    public class PatientScreenData
    {
        public User User { get; set; }
        public PatientDetail Patient { get; set; }
    }
}
