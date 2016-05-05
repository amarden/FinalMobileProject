using Azure.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    public class ProviderDashboard
    {
        public int ProviderId { get; set; }
        public string Role { get; set; }
        public IEnumerable<PatientSimple> patients { get; set; }
        public IEnumerable<PatientProcedure> procedures { get; set; }
    }
}