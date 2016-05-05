using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    public class DataInfo
    {
        public IEnumerable<PatientDashboard> patients { get; set; }
        public IEnumerable<ProviderDashboard> providers { get; set; }
        public IEnumerable<Item> procedures { get; set; }
        public IEnumerable<Item> diagnoses { get; set; }
    }
}