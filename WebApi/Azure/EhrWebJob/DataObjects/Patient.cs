using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class Patient
    {
        public Patient()
        {
            Biometrics = new HashSet<Biometric>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }
        public int DiagnosisCodeId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MedicalStatus { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime? AssignDate{ get; set; }
        public DateTime? DischargeDate { get; set; }
        public virtual ICollection<Biometric> Biometrics { get; set; }

    }
}
