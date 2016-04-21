﻿using System;
using System.Collections.Generic;

namespace Client.ClientObjects
{
    public class PatientDetail
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MedicalStatus { get; set; }
        public string Diagnosis { get; set; }
        public DateTime AdmitDate { get; set; }
        public virtual ICollection<Biometric> Biometrics { get; set; }
        public int ChatActivityNumber { get; set; }
        public int ProcedureNumber { get; set; }
        public int ProviderNumber { get; set; }
        public int ImageNumber { get; set; }
    }
}