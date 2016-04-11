using Azure.DataObjects;
using System;
using System.Collections.Generic;

namespace Azure.ClientObjects
{
    public class ViewPatient
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MedicalStatus { get; set; }
        public string Diagnosis { get; set; }
        public DateTime AdmitDate { get; set; }
        public virtual ICollection<PatientProcedure> PatientProcedures { get; set; }
        public virtual ICollection<PatientChatLog> PatientChatLogs { get; set; }
        public virtual ICollection<PatientProvider> PatientProviders { get; set; }
        public virtual ICollection<PatientToDo> PatientToDos { get; set; }
    }
}