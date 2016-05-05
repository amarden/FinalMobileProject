using Azure.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    public class PatientDashboard
    {
        public string PatientId { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public IEnumerable<PatientProcedure> procedures { get; set; }
        public string diagnosis { get; set; }
        public int providerCount { get; set; }
        public int imageCount { get; set; }
        public int chatCount { get; set; }
        public string MedicalStatus { get; set; }
    }
}