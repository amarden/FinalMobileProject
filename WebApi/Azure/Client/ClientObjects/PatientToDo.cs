using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientObjects
{
    public class PatientToDo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientToDoId { get; set; }
        public int PatientId { get; set; }
        public int? ProviderId { get; set; }
        public string Note { get; set; }
        public int ProcedureCodeId { get; set; }
        public DateTime Created { get; set; }
        public bool Complete { get; set; }
        public DateTime CompleteDate { get; set; }
    }
}
