using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class PatientProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientProcedureId { get; set; }
        public int PatientId { get; set; }
        public int ProcedureCodeId{ get; set; }
        public virtual Patient Patient { get; set; }
        public virtual ProcedureCode ProcedureCode { get; set; }

    }
}
