using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.DataObjects
{
    public class PatientToDo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientToDoId { get; set; }
        public int PatientId { get; set; }
        public string note { get; set; }
        public int procedureCodeId { get; set; }
        public DateTime Created { get; set; }
        public bool Complete { get; set; }
        public DateTime CompleteDate { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
