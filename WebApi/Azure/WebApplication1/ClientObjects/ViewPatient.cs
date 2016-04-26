using Azure.DataObjects;
using System;
using System.Collections.Generic;

namespace Azure.ClientObjects
{
    public class ViewPatient
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string MedicalStatus { get; set; }
        public string Diagnosis { get; set; }
        public DateTime AdmitDate { get; set; }
        public int NumProvidersAssigned{ get; set; }
    }
}