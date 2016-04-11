using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.ClientObjects
{
    public class ToDoItem
    {
        public int PatientToDoId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string Note { get; set; }
        public int ProcedureCodeId { get; set; }
        public DateTime Created { get; set; }
        public bool Complete { get; set; }
    }
}