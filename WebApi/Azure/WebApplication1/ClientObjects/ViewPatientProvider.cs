using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.ClientObjects
{
    public class ViewPatientProvider
    {
        public int ProviderPatientId { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
    }
}